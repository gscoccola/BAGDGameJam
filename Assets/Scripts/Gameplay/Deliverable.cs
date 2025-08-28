using UnityEngine;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Deliverable : MonoBehaviour
    {
        private bool _isDelivered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<DeliveryManager>(out var manager) && !_isDelivered)
            {
                manager.DeliverItem(this);
                _isDelivered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<DeliveryManager>(out var manager) && _isDelivered)
            {
                manager.RetrieveItem(this);
                _isDelivered = false;
            }
        }
    }
}
