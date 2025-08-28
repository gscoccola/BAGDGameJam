using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixelmate.Gameplay
{
    public class DeliveryManager : Singleton<DeliveryManager>
    {
        [SerializeField]
        private int _requiredDeliveries = 3;

        public event Action<int> OnItemDelivered;
        public event Action<int> OnItemRetrieved;
        public event Action OnAllItemsDelivered;

        private List<Deliverable> _deliveredItems = new List<Deliverable>();

        public void DeliverItem(Deliverable deliverable)
        {
            if (_deliveredItems.Contains(deliverable))
                return;

            _deliveredItems.Add(deliverable);
            OnItemDelivered?.Invoke(_deliveredItems.Count);

            if (_deliveredItems.Count >= _requiredDeliveries)
                OnAllItemsDelivered?.Invoke();
        }

        public void RetrieveItem(Deliverable deliverable)
        {
            if (_deliveredItems.Count > 0)
            {
                _deliveredItems.Remove(deliverable);
                OnItemRetrieved?.Invoke(_deliveredItems.Count);
            }
        }
    }
}
