using PixelMate.Collectibles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Grabber : MonoBehaviour, IGrabber
    {
        private GameObject pointedGameObject;
        public LayerMask defualtAndSheepsMask = ~0;
        
        
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

        [Header("Audio")]
        [SerializeField]
        private CustomAudioClip[] _grabSounds;
        [SerializeField]
        private CustomAudioClip _dropSound;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField]
        private bool _showDebugRay = true;
        [SerializeField]
        private float _rayDuration = 2.5f;
#endif

        private bool _canGrab = true;
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

        private void Update()
        {  
            Ray ray = _camera.ViewportPointToRay(_rayOrigin);
            if (Physics.SphereCast(ray, _sphereRadius, out var hitInteractives, _grabDistance, defualtAndSheepsMask, QueryTriggerInteraction.Collide))
            {
                if (pointedGameObject == hitInteractives.collider.gameObject) return;
                // Picking up collectibles
                if (hitInteractives.collider.TryGetComponent<Collectible>(out var collectible))
                {
                    EventManager.Instance.Trigger(Constants.OnWantInteract);
                    pointedGameObject = hitInteractives.collider.gameObject;
                    return;
                }

                // Opening/closing the famryard door
                var parentTransform = hitInteractives.collider.transform.parent;
                if (parentTransform != null && parentTransform.TryGetComponent<SheepDoor>(out var door))
                {
                    EventManager.Instance.Trigger(Constants.OnWantInteract);
                    pointedGameObject = hitInteractives.collider.gameObject;
                    return;
                }

                //Oveja
                if (hitInteractives.collider.TryGetComponent<IGrabbable>(out var grabbable))
                {
                    EventManager.Instance.Trigger(Constants.OnWantInteract);
                    pointedGameObject = hitInteractives.collider.gameObject;
                }
                
                
                EventManager.Instance.Trigger(Constants.OnCancelWantInteract);
            }
            else
            {
                EventManager.Instance.Trigger(Constants.OnCancelWantInteract);
                pointedGameObject = null;
            }
        }


        public void Grab()
        {
            if (!_canGrab)
                return;

            Ray ray = _camera.ViewportPointToRay(_rayOrigin);

#if UNITY_EDITOR
            if (_showDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * _grabDistance, Color.cyan, _rayDuration);
            }
#endif

            if (Physics.SphereCast(ray, _sphereRadius, out var hitInteractives, _grabDistance, _defaultMask, QueryTriggerInteraction.Collide))
            {
                Debug.Log($"Player ray has hit {hitInteractives.collider.name}");

                // Picking up collectibles
                if (hitInteractives.collider.TryGetComponent<Collectible>(out var collectible))
                {
                    collectible.Collect();
                    return;
                }

                // Opening/closing the famryard door
                var parentTransform = hitInteractives.collider.transform.parent;
                if (parentTransform != null && parentTransform.TryGetComponent<SheepDoor>(out var door))
                {
                    door.MoveDoor();
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
                    AudioSystem.Instance?.PlaySound(_grabSounds[Random.Range(0, _grabSounds.Length)]);
                    EventManager.Instance.Trigger(Constants.OnPlayerKeepSheepForSounds, true);
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
            AudioSystem.Instance?.PlaySound(_dropSound);
            EventManager.Instance.Trigger(Constants.OnPlayerKeepSheepForSounds, false);
        }

        public void AllowGrabberState(bool state) => _canGrab = state;
    }
}
