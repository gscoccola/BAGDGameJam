using PixelMate.Gameplay;
using TMPro;
using UnityEngine;

namespace PixelMate.UI
{
    public sealed class DeliverySign : MonoBehaviour
    {
        [SerializeField]
        private DeliveryManager _deliveryManager;
        [SerializeField]
        private TMP_Text _signText;
        [SerializeField]
        private string _countText = "Sheeps in Farmyard: {0}";
        [SerializeField]
        private string _allDeliveredText = "All sheeps are in the Farmyard!";

        private void Awake()
        {
            _signText.SetText(_countText, 0);
        }

        private void OnEnable()
        {
            _deliveryManager.OnItemAmountChanged -= HandleNumberOfItemsChange;
            _deliveryManager.OnItemAmountChanged += HandleNumberOfItemsChange;

            _deliveryManager.OnAllItemsDelivered -= HandleAllItemsDelivered;
            _deliveryManager.OnAllItemsDelivered += HandleAllItemsDelivered;
        }

        private void OnDisable()
        {
            _deliveryManager.OnItemAmountChanged -= HandleNumberOfItemsChange;
        }

        private void HandleNumberOfItemsChange(int obj)
        {
            _signText.SetText(_countText, obj);
        }

        private void HandleAllItemsDelivered()
        {
            _signText.SetText(_allDeliveredText);
        }
    }
}
