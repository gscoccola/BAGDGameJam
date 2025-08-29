using UnityEngine;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public sealed class Grabbable : MonoBehaviour, IGrabbable
    {
        [SerializeField]
        private Rigidbody rb;

        public bool IsHeld { get; private set; }

        public void BeginGrab(Transform holdPoint)
        {
            if (IsHeld)
                return;

            IsHeld = true;
            transform.SetParent(holdPoint, true);
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        public void EndGrab()
        {
            if (!IsHeld)
                return;

            IsHeld = false;
            transform.SetParent(null, true);
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}
