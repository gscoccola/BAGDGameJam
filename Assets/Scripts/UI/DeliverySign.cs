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
        private Canvas _worldCanvas;
        [SerializeField]
        private TMP_Text _signText;
        [SerializeField]
        private string _countText = "Sheeps: {0}/{1}";
        [SerializeField]
        private string _allDeliveredText = "All sheeps are in the Farmyard!";

        private void Awake()
        {
            _worldCanvas.worldCamera = Camera.main;
        }

        private void Start()
        {
            string content = string.Format(_countText, _deliveryManager.CurrentDeliveries, _deliveryManager.RequiredDeliveries);
            _signText.SetText(content);
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

        private void HandleNumberOfItemsChange(int current)
        {
            string content = string.Format(_countText, current, _deliveryManager.RequiredDeliveries);
            _signText.SetText(content);
        }

        private void HandleAllItemsDelivered()
        {
            _signText.SetText(_allDeliveredText);
        }
    }
}
