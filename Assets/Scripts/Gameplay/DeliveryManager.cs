using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMate.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class DeliveryManager : MonoBehaviour
    {
        [SerializeField]
        private int _requiredDeliveries = 3;

        public event Action<int> OnItemAmountChanged;
        public event Action OnAllItemsDelivered;

        private List<Deliverable> _deliveredItems = new List<Deliverable>();

        public int RequiredDeliveries => _requiredDeliveries;
        public int CurrentDeliveries => _deliveredItems.Count;

        public void DeliverItem(Deliverable deliverable)
        {
            if (_deliveredItems.Contains(deliverable))
                return;

            _deliveredItems.Add(deliverable);

            if (_deliveredItems.Count >= _requiredDeliveries)
                OnAllItemsDelivered?.Invoke();
            else
                OnItemAmountChanged?.Invoke(_deliveredItems.Count);
        }

        public void RetrieveItem(Deliverable deliverable)
        {
            if (_deliveredItems.Count > 0)
            {
                _deliveredItems.Remove(deliverable);
                OnItemAmountChanged?.Invoke(_deliveredItems.Count);
            }
        }
    }
}
