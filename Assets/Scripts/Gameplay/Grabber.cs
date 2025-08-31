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
        [SerializeField]
        private LayerMask _defaultMask = ~0;

        [SerializeField]
        private GameObject _leftArm;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField]
        private bool _showDebugRay = true;
        [SerializeField]
        private float _rayDuration = 2.5f;
#endif

        private Camera _camera;
        private IGrabbable _heldItem;

        private void Awake()
        {
            _camera = Camera.main;
            _leftArm.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            PlayerControls.Instance.OnClickAttack -= Grab;
            PlayerControls.Instance.OnClickAttack += Grab;
        }

        private void OnDisable()
        {
            if (PlayerControls.Instance != null)
            {
                PlayerControls.Instance.OnClickAttack -= Grab;
            }
        }

        public void Grab()
        {
            Ray ray = _camera.ViewportPointToRay(_rayOrigin);

#if UNITY_EDITOR
            if (_showDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * _grabDistance, Color.cyan, _rayDuration);
            }
#endif

            // Opening/closing the famryard door
            if (Physics.SphereCast(ray, _sphereRadius, out var hitInteractives, _grabDistance, _defaultMask, QueryTriggerInteraction.Collide))
            {
                Debug.Log($"Player has hit {hitInteractives.collider.name}");

                SheepDoor door = hitInteractives.collider.GetComponentInParent<SheepDoor>();

                if (door)
                {
                    Debug.Log($"Player tries to open the door {door.name}");
                    door.OpenDoor();
                    return;
                }
            }

            // Grabbing and releasing the sheeps
            if (_heldItem != null)
            {
                Release();
                return;
            }

            if (Physics.SphereCast(ray, _sphereRadius, out var hit, _grabDistance, _grabMask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.TryGetComponent<IGrabbable>(out var grabbable))
                {
                    grabbable.BeginGrab();
                    _heldItem = grabbable;
                    _leftArm.gameObject.SetActive(true);
                }
            }
        }

        public void Release()
        {
            if (_heldItem == null)
                return;

            _heldItem.EndGrab(transform.position, transform.rotation);
            _heldItem = null;
            _leftArm.gameObject.SetActive(false);
        }
    }
}
