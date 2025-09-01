using PixelMate.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

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

        [Header("Text localizations")]
        [SerializeField]
        private string _countTextES         = "Ovejas: {0}/{1}";
        [SerializeField]
        private string _allDelipveredTextES = "Cierra el corral!";
        [SerializeField]
        private string _countTextEN         = "Sheeps: {0}/{1}";
        [SerializeField]
        private string _allDeliveredTextEN  = "Close the Farmyard!";

        private void Awake()
        {
            _worldCanvas.worldCamera = Camera.main;
        }

        private void Start()
        {
            _signText.SetText(CheckCountLocale());
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
            _deliveryManager.OnAllItemsDelivered -= HandleAllItemsDelivered;
        }

        private void HandleNumberOfItemsChange(int current)
        {
            _signText.SetText(CheckCountLocale());
        }

        private void HandleAllItemsDelivered()
        {
            _signText.SetText(CheckAllItemsLocale());
        }

        private string CheckCountLocale()
        {
            string content = (LocalizationSettings.SelectedLocale.LocaleName == "Spanish (es)") ? _countTextES : _countTextEN;
            return string.Format(content, _deliveryManager.CurrentDeliveries, _deliveryManager.RequiredDeliveries);
        }

        private string CheckAllItemsLocale()
        {
            return (LocalizationSettings.SelectedLocale.LocaleName == "Spanish (es)") ? _allDelipveredTextES : _allDeliveredTextEN;
        }
    }
}
