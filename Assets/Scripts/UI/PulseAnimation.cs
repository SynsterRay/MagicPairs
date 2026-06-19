using UnityEngine;

namespace MagicPairs.UI
{
    public class PulseAnimation : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float minScale = 0.92f;
        [SerializeField] private float maxScale = 1.08f;

        private Vector3 _baseScale;

        private void OnEnable()
        {
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
            float s = Mathf.Lerp(minScale, maxScale, t);
            transform.localScale = _baseScale * s;
        }

        private void OnDisable()
        {
            transform.localScale = _baseScale;
        }
    }
}
