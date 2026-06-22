using UnityEngine;

namespace MagicPairs.UI
{
    /// <summary>
    /// Keeps the ScreenSpaceCamera gradient canvas synced with the main camera.
    /// Ensures planeDistance stays between cards (z=0) and background (z=5).
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class GradientSync : MonoBehaviour
    {
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void LateUpdate()
        {
            if (_canvas.worldCamera == null)
                _canvas.worldCamera = Camera.main;

            // Keep plane distance so gradient renders behind cards but in front of background
            var cam = _canvas.worldCamera;
            if (cam != null)
                _canvas.planeDistance = cam.orthographicSize * 2.4f;
        }
    }
}
