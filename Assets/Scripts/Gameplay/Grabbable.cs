using UnityEngine;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public sealed class Grabbable : MonoBehaviour, IGrabbable
    {
        [SerializeField]
        private Rigidbody _rb;

        public bool IsHeld { get; private set; }

        public void BeginGrab(Transform holdPoint)
        {
            if (IsHeld)
                return;

            IsHeld = true;

            transform.SetParent(holdPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            _rb.useGravity = false;
            _rb.isKinematic = true;
        }

        public void EndGrab()
        {
            if (!IsHeld)
                return;

            IsHeld = false;
            transform.SetParent(null, true);
            _rb.useGravity = true;
            _rb.isKinematic = false;
        }
    }
}
