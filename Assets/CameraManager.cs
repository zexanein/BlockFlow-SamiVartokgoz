using UnityEngine;

public class CameraManager : ManagerLocatable
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform board;
    [SerializeField] private RectTransform fitArea;

    [SerializeField] private float maxDistance = 500f;
    [SerializeField] private float minDistancePhone = 25f;
    [SerializeField] private float minDistanceTablet = 20f;
    [SerializeField] private int binarySearchIterations = 30;
    [SerializeField] private float horizontalPaddingPixels = 40f;
    [SerializeField] private float verticalPaddingPixels = 20f;

    public void FitCamera(int boardWidth, int boardHeight, float cellSize = 1f)
    {
        if (cam == null || board == null || fitArea == null) return;
        
        var aspect = (float)Screen.width / Screen.height;
        var aspectFactor = Mathf.InverseLerp(0.46f, 0.75f, aspect);
        var minDistance = Mathf.Lerp(minDistancePhone, minDistanceTablet, aspectFactor);

        var anchorMin = fitArea.anchorMin;
        var anchorMax = fitArea.anchorMax;
        var offsetMin = fitArea.offsetMin;
        var offsetMax = fitArea.offsetMax;

        float screenW = Screen.width;
        float screenH = Screen.height;

        var pixelMinX = anchorMin.x * screenW + offsetMin.x + horizontalPaddingPixels;
        var pixelMaxX = anchorMax.x * screenW + offsetMax.x - horizontalPaddingPixels;
        var pixelMinY = anchorMin.y * screenH + offsetMin.y + verticalPaddingPixels;
        var pixelMaxY = anchorMax.y * screenH + offsetMax.y - verticalPaddingPixels;

        var targetMinX = pixelMinX / screenW;
        var targetMaxX = pixelMaxX / screenW;
        var targetMinY = pixelMinY / screenH;
        var targetMaxY = pixelMaxY / screenH;

        var offsetDir = -cam.transform.forward;
        var corners = GetBoardCorners(boardWidth, boardHeight, cellSize);

        var lo = minDistance;
        var hi = maxDistance;

        for (var i = 0; i < binarySearchIterations; i++)
        {
            var mid = (lo + hi) * 0.5f;
            cam.transform.position = board.position + offsetDir * mid;

            if (CornersFitInViewport(corners, targetMinX, targetMaxX, targetMinY, targetMaxY))
                hi = mid;
            else lo = mid;
        }
        
        cam.transform.position = board.position + offsetDir * hi;
        CenterBoardInRect(targetMinX, targetMaxX, targetMinY, targetMaxY);
    }

    private void CenterBoardInRect(float minX, float maxX, float minY, float maxY)
    {
        var targetCenterX = (minX + maxX) * 0.5f;
        var targetCenterY = (minY + maxY) * 0.5f;

        var targetRay = cam.ViewportPointToRay(new Vector3(targetCenterX, targetCenterY, 0f));
        var boardPlane = new Plane(Vector3.up, board.position);

        if (!boardPlane.Raycast(targetRay, out float enter)) return;
        var targetWorldPoint = targetRay.GetPoint(enter);
        var delta = board.position - targetWorldPoint;
        cam.transform.position += delta;
    }

    private bool CornersFitInViewport(Vector3[] corners, float minX, float maxX, float minY, float maxY)
    {
        foreach (var corner in corners)
        {
            var vp = cam.WorldToViewportPoint(corner);
            if (vp.z < 0f) return false;
            if (vp.x < minX || vp.x > maxX) return false;
            if (vp.y < minY || vp.y > maxY) return false;
        }
        return true;
    }

    private Vector3[] GetBoardCorners(int boardWidth, int boardHeight, float cellSize)
    {
        var hx = boardWidth * cellSize * 0.5f;
        var hz = boardHeight * cellSize * 0.5f;
        var c = board.position;
        var rot = board.rotation;

        return new[]
        {
            c + rot * new Vector3(-hx, 0f, -hz),
            c + rot * new Vector3( hx, 0f, -hz),
            c + rot * new Vector3(-hx, 0f,  hz),
            c + rot * new Vector3( hx, 0f,  hz),
        };
    }
}