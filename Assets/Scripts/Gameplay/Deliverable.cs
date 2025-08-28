using UnityEngine;

namespace Pixelmate.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Deliverable : MonoBehaviour
    {
        private bool _isDelivered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("DeliveryZone") && !_isDelivered)
            {
                DeliveryManager.Instance.DeliverItem(this);
                _isDelivered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("DeliveryZone") && _isDelivered)
            {
                DeliveryManager.Instance.RetrieveItem(this);
                _isDelivered = false;
            }
        }
    }
}
