using UnityEngine;

namespace MagicPairs.UI
{
    /// <summary>
    /// Ensures ScreenSpaceCamera gradient canvas keeps its camera reference.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class GradientSync : MonoBehaviour
    {
        private Canvas _canvas;

        private void Awake() => _canvas = GetComponent<Canvas>();

        private void LateUpdate()
        {
            if (_canvas.worldCamera == null)
                _canvas.worldCamera = Camera.main;
        }
    }
}
