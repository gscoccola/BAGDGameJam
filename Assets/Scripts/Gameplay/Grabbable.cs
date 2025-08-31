using UnityEngine;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public sealed class Grabbable : MonoBehaviour, IGrabbable
    {
        [SerializeField]
        private bool _willWarnEnemy = true;

        private Deliverable _deliverable;

        public bool IsHeld { get; private set; }

        private void Awake()
        {
            _deliverable = GetComponent<Deliverable>();
        }

        public void BeginGrab()
        {
            if (IsHeld)
                return;

            if (_willWarnEnemy)
            {
                EventManager.Instance.Trigger(Constants.OnSheepPickup);
            }

            IsHeld = true;
            gameObject.SetActive(false);
        }

        public void EndGrab(Vector3 releasePoint, Quaternion rotation)
        {
            if (!IsHeld)
                return;

            gameObject.SetActive(true);
            IsHeld = false;
            transform.SetPositionAndRotation(releasePoint, rotation);
            _deliverable.HandleDelivery();
        }
    }
}
