using UnityEngine;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Deliverable : MonoBehaviour
    {
        [Header("Collision Detection")]
        [SerializeField]
        private float _detectionRadius = 1f;
        [SerializeField]
        private LayerMask _deliveryManagerLayers = 0;

        [Header("Models")]
        [SerializeField]
        private GameObject _aliveModel;
        [SerializeField]
        private GameObject _deathModel;

        private bool _isDelivered = false;
        private DeliveryManager _currentDeliveryManager;

        private void Start()
        {
            // Initial delivery check in case starting in a delivery zone
            HandleDelivery();
        }

        public void HandleDelivery()
        {
            // Find all delivery managers within range
            var colliders = Physics.OverlapSphere(transform.position, _detectionRadius, _deliveryManagerLayers, QueryTriggerInteraction.Collide);

            DeliveryManager closestManager = null;
            float closestDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<DeliveryManager>(out var manager))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestManager = manager;
                    }

                    break;
                }
            }

            // Handle delivery state changes
            if (closestManager != null)
            {
                // If we found a manager and we're not delivered, or it's a different manager
                if (!_isDelivered || _currentDeliveryManager != closestManager)
                {
                    // Remove from previous manager if we had one
                    if (_isDelivered && _currentDeliveryManager != null)
                    {
                        _currentDeliveryManager.RetrieveItem(this);
                    }

                    // Deliver to the closest manager
                    closestManager.DeliverItem(this);
                    _isDelivered = true;
                    _currentDeliveryManager = closestManager;

                    Debug.Log($"{name} delivered to {closestManager.name}");
                }
            }
            else
            {
                // No delivery manager in range, remove from current if delivered
                if (_isDelivered && _currentDeliveryManager != null)
                {
                    _currentDeliveryManager.RetrieveItem(this);
                    _isDelivered = false;
                    _currentDeliveryManager = null;

                    Debug.Log($"{name} retrieved from delivery zone");
                }
            }
        }

        public void KillSheep()
        {
            _aliveModel.SetActive(false);
            _deathModel.SetActive(true);
            GetComponent<SHeepSoundsOnSheep>().enabled = false;
        }
    }
}
