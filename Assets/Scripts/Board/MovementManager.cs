using UnityEngine;

public class MovementManager : ManagerLocatable
{
    private BoardManager _boardManager;
    private BlockManager _blockManager;
    private GrinderManager _grinderManager;
    private BlockActor _selectedBlock;
    private Vector3 _dragStartWorld;
    private Vector3 _blockStartWorld;
    private Vector3 _lastResolvedWorld;
    private bool _isDragging;
    
    private Camera _camera;
    private const float BlockCollisionTolerance = 0.1f;
    private const float SweepAxisStepSize = 0.4f;
    
    public LayerMask interactableLayerMask;
    public string blockActorTag = "BlockActor";

    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
        _blockManager = Locator.GetLocatable<BlockManager>();
        _grinderManager = Locator.GetLocatable<GrinderManager>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TrySelectBlock();
        if (Input.GetMouseButton(0) && _isDragging) DragBlock();
        if (Input.GetMouseButtonUp(0) && _isDragging) ReleaseBlock();
    }

    private void TrySelectBlock()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, interactableLayerMask)) return;

        if (!hit.collider.CompareTag(blockActorTag)) return;
        var block = hit.collider.GetComponentInParent<BlockActor>();
        if (block == null) return;

        _selectedBlock = block;
        _dragStartWorld = GetMouseWorldPosition();
        _blockStartWorld = block.transform.position;
        _lastResolvedWorld = block.transform.position;
        _isDragging = true;

        _boardManager.UnregisterBlock(_selectedBlock.Data);
    }

    private void DragBlock()
    {
        var mouseWorld = GetMouseWorldPosition();
        var mouseDelta = mouseWorld - _dragStartWorld;

        var constraint = _selectedBlock.Data.Constraint;
        if (constraint == AxisConstraint.Horizontal) mouseDelta.z = 0f;
        if (constraint == AxisConstraint.Vertical) mouseDelta.x = 0f;

        var desiredWorld = _blockStartWorld + new Vector3(mouseDelta.x, 0f, mouseDelta.z);
        _lastResolvedWorld = SweepToPosition(_lastResolvedWorld, desiredWorld);

        _selectedBlock.transform.position = new Vector3(
            _lastResolvedWorld.x,
            _selectedBlock.transform.position.y,
            _lastResolvedWorld.z
        );
        
        var snappedGrid = _boardManager.WorldToGrid(_lastResolvedWorld);
        var shape = _blockManager.GetBlockRotatedShape(_selectedBlock.Data);

        if (!_boardManager.IsShapeAtEdge(shape, snappedGrid)) return;

        var originalPos = _selectedBlock.Data.Position;
        _selectedBlock.Data.Position = snappedGrid;

        var exitDirection = _grinderManager.TryGrindBlock(_selectedBlock.Data);
        if (exitDirection.HasValue)
        {
            var blockToRemove = _selectedBlock;
            _selectedBlock = null;
            _isDragging = false;
            blockToRemove.PlayExitAnimation(shape, exitDirection.Value, () => _blockManager.RemoveBlock(blockToRemove));
            return;
        }

        _selectedBlock.Data.Position = originalPos;
    }

    private void ReleaseBlock()
    {
        var snappedGrid = _boardManager.WorldToGrid(_selectedBlock.transform.position);
        _selectedBlock.Data.Position = snappedGrid;
        _boardManager.RegisterBlock(_selectedBlock.Data);
        _selectedBlock.SnapToGrid();
        _selectedBlock = null;
        _isDragging = false;
    }

    private Vector3 SweepToPosition(Vector3 from, Vector3 to)
    {
        var shape = _blockManager.GetBlockRotatedShape(_selectedBlock.Data);
        var fromGrid = _boardManager.WorldToGridFloat(from);
        var toGrid = _boardManager.WorldToGridFloat(to);

        var resolvedX = SweepAxis(fromGrid.x, toGrid.x - fromGrid.x, fromGrid.y, shape, true);
        var resolvedY = SweepAxis(fromGrid.y, toGrid.y - fromGrid.y, resolvedX, shape, false);

        return _boardManager.GridToWorld(resolvedX, resolvedY);
    }

    private float SweepAxis(float start, float delta, float otherAxis, Vector2Int[] shape, bool isXAxis)
    {
        if (Mathf.Abs(delta) < 0.001f) return start;

        var sign = Mathf.Sign(delta);
        var target = start + delta;
        var current = start;
        var steps = Mathf.CeilToInt(Mathf.Abs(delta) / SweepAxisStepSize);

        for (var i = 1; i <= steps; i++)
        {
            var next = i == steps ? target : start + sign * SweepAxisStepSize * i;

            if (CheckCollisionAtPosition(next, otherAxis, shape, isXAxis, sign))
                return current;

            current = next;
        }

        return current;
    }

    private bool CheckCollisionAtPosition(float axisValue, float otherAxis, Vector2Int[] shape, bool isXAxis, float direction)
    {
        foreach (var offset in shape)
        {
            var xValue = isXAxis ? axisValue : otherAxis;
            var yValue = isXAxis ? otherAxis : axisValue;
            var cellX = xValue + offset.x;
            var cellY = yValue + offset.y;

            if (cellX < -0f || cellX > _boardManager.Width - 1f) return true;
            if (cellY < -0f || cellY > _boardManager.Height - 1f) return true;

            int checkMain, checkOther1, checkOther2;
            
            if (isXAxis)
            {
                checkMain = direction > 0
                    ? Mathf.CeilToInt(cellX - BlockCollisionTolerance)
                    : Mathf.FloorToInt(cellX + BlockCollisionTolerance);
                
                checkOther1 = Mathf.FloorToInt(cellY + BlockCollisionTolerance);
                checkOther2 = Mathf.CeilToInt(cellY - BlockCollisionTolerance);

                if (_boardManager.IsInBounds(checkMain, checkOther1) && _boardManager.IsCellOccupied(checkMain, checkOther1))
                    return true;
                
                if (checkOther2 != checkOther1 && _boardManager.IsInBounds(checkMain, checkOther2) && _boardManager.IsCellOccupied(checkMain, checkOther2))
                    return true;
            }
            
            else
            {
                checkMain = direction > 0
                    ? Mathf.CeilToInt(cellY - BlockCollisionTolerance)
                    : Mathf.FloorToInt(cellY + BlockCollisionTolerance);
                
                checkOther1 = Mathf.FloorToInt(cellX + BlockCollisionTolerance);
                checkOther2 = Mathf.CeilToInt(cellX - BlockCollisionTolerance);

                if (_boardManager.IsInBounds(checkOther1, checkMain) && _boardManager.IsCellOccupied(checkOther1, checkMain))
                    return true;
                
                if (checkOther2 != checkOther1 && _boardManager.IsInBounds(checkOther2, checkMain) && _boardManager.IsCellOccupied(checkOther2, checkMain))
                    return true;
            }
        }

        return false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, Vector3.zero);
        plane.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }
}