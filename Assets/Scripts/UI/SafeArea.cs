using System;
using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Update()
    {
        if (_lastSafeArea != Screen.safeArea)
            ApplySafeArea();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var safeArea = Screen.safeArea;
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var rect = new Rect(
            safeArea.x / screenWidth,
            safeArea.y / screenHeight,
            safeArea.width / screenWidth,
            safeArea.height / screenHeight
        );

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0), new Vector3(rect.width, rect.height, 0));
    }

    private void ApplySafeArea()
    {
        var safeArea = Screen.safeArea;
        _lastSafeArea = safeArea;

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}