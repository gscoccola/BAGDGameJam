using UnityEngine;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Grabber : MonoBehaviour, IGrabber
    {
        [SerializeField]
        private Transform _holdPoint;

        [Header("Raycast")]
        [SerializeField]
        private Vector3 _rayOrigin = new Vector3(0.5f, 0.5f, 0);
        [SerializeField]
        private float _grabDistance = 3.0f;
        [SerializeField]
        private float _sphereRadius = 0.25f;
        [SerializeField]
        private LayerMask _grabMask = ~0;

        private Camera _camera;
        private IGrabbable _heldItem;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            PlayerControls.Instance.OnClickAttack += Grab;
            PlayerControls.Instance.OnReleaseAttack += Release;
        }

        private void OnDisable()
        {
            if (PlayerControls.Instance != null)
            {
                PlayerControls.Instance.OnClickAttack -= Grab;
                PlayerControls.Instance.OnReleaseAttack -= Release;
            }
        }

        public void Grab()
        {
            if (_heldItem != null)
                return;

            Ray ray = _camera.ViewportPointToRay(_rayOrigin);

            if (Physics.SphereCast(ray, _sphereRadius, out var hit, _grabDistance, _grabMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.TryGetComponent<IGrabbable>(out var grabbable))
                {
                    grabbable.BeginGrab(_holdPoint);
                    _heldItem = grabbable;
                }
            }
        }

        public void Release()
        {
            if (_heldItem == null)
                return;

            _heldItem.EndGrab();
            _heldItem = null;
        }
    }
}
