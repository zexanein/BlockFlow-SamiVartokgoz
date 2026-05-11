using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : ManagerLocatable
{
    [SerializeField] private BlockActor blockActorPrefab;
    [SerializeField] private BlockShapeRegistry blockShapeRegistry;

    private BoardManager _boardManager;
    private AudioManager _audioManager;
    private ParticleManager _particleManager;

    private readonly Dictionary<int, Vector2Int[]> _rotatedShapeCache = new();
    private readonly Dictionary<int, Vector2Int[]> _cellPositionsBuffer = new();
    
    private readonly List<BlockActor> _activeActors = new();
    private readonly List<BlockActor> _icedBlockActors = new();
    public int ActiveBlockCount => _activeActors.Count;
    
    public event Action OnBlockCleared;

    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
        _audioManager = Locator.GetLocatable<AudioManager>();
        _particleManager = Locator.GetLocatable<ParticleManager>();
    }

    public void SpawnBlocks(List<BlockData> blocks)
    {
        foreach (var blockData in blocks)
        {
            var baseShape = blockShapeRegistry.Get(blockData.ShapeType).BaseShape;
            var rotatedShape = StaticMethods.RotateShape(baseShape, (int)blockData.Direction);
            _rotatedShapeCache[blockData.BlockID] = rotatedShape;
            _cellPositionsBuffer[blockData.BlockID] = new Vector2Int[rotatedShape.Length];

            var blockActor = Instantiate(blockActorPrefab, transform);
            blockActor.Initialize(blockData, _boardManager, this, blockShapeRegistry);
            GenerateBoxCollidersForBlock(blockActor, baseShape, _boardManager.CellSize);
            _activeActors.Add(blockActor);
            if (blockData.IceEffectDuration > 0) _icedBlockActors.Add(blockActor);
        }
    }

    private void GenerateBoxCollidersForBlock(BlockActor actor, Vector2Int[] shape, float cellSize)
    {
        var visited = new HashSet<int>();

        for (var i = 0; i < shape.Length; i++)
        {
            if (visited.Contains(i)) continue;

            var start = shape[i];
            var count = 1;
            visited.Add(i);

            while (true)
            {
                var next = new Vector2Int(start.x + count, start.y);
                var found = false;

                for (var j = 0; j < shape.Length; j++)
                {
                    if (visited.Contains(j) || shape[j] != next) continue;
                    
                    visited.Add(j);
                    count++;
                    found = true;
                    break;
                }

                if (!found) break;
            }

            var box = actor.gameObject.AddComponent<BoxCollider>();
            box.center = new Vector3(
                (start.x + (count - 1) * 0.5f) * cellSize,
                0.5f,
                start.y * cellSize
            );
            
            box.size = new Vector3(
                count * cellSize,
                cellSize,
                cellSize
            );
            
            actor.RegisterCollider(box);
        }
    }
    
    public Vector2Int[] GetBlockRotatedShape(BlockData blockData)
    {
        return _rotatedShapeCache[blockData.BlockID];
    }

    public Vector2Int[] GetBlockCellGridPositions(BlockData blockData)
    {
        var shape = _rotatedShapeCache[blockData.BlockID];
        var buffer = _cellPositionsBuffer[blockData.BlockID];

        for (var i = 0; i < shape.Length; i++)
            buffer[i] = blockData.Position + shape[i];

        return buffer;
    }

    public void RemoveBlock(BlockActor blockActor)
    {
        _rotatedShapeCache.Remove(blockActor.Data.BlockID);
        _cellPositionsBuffer.Remove(blockActor.Data.BlockID);
        _activeActors.Remove(blockActor);
        Destroy(blockActor.gameObject);
        
        foreach (var icedBlockActor in _icedBlockActors)
        {
            icedBlockActor.Data.IceEffectDuration--;
            _particleManager.PlayIceRevealParticle(icedBlockActor.transform.position.WithY(2f));
            
            if (icedBlockActor.Data.IceEffectDuration <= 0)
            {
                _audioManager.PlaySfx("IceReveal");
            }
        }
        
        OnBlockCleared?.Invoke();
        
    }

    public void Clear()
    {
        foreach (var block in _activeActors)
            Destroy(block);
        _activeActors.Clear();
    }
}