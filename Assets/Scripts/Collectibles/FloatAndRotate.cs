using UnityEngine;

namespace PixelMate.Collectibles
{
    [DisallowMultipleComponent]
    public sealed class FloatAndRotate : MonoBehaviour
    {
        [Header("Floating")]
        [SerializeField]
        private float _amplitude = 0.35f; // height of the bob
        [SerializeField]
        private float _frequency = 2f;    // speed of the bob

        [Header("Rotation")]
        [SerializeField]
        private float _rotationSpeed = 120f; // degrees per second

        private Transform _transform;
        private Vector3 _startPos;

        private void Awake()
        {
            _transform = transform;
            _startPos = transform.localPosition;
        }

        private void Update()
        {
            // Vertical bobbing using sine wave
            float offsetY = Mathf.Sin(Time.time * _frequency) * _amplitude;
            _transform.localPosition = _startPos + new Vector3(0, offsetY, 0);

            // Constant spin around Y
            _transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
