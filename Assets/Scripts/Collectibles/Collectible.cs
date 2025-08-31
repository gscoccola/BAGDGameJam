using UnityEngine;

namespace PixelMate.Collectibles
{
    [DisallowMultipleComponent]
    public sealed class Collectible : MonoBehaviour
    {
        [SerializeField]
        private CollectibleData _collectibleData;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private bool _disableOnCollected = true;

        public CollectibleData CollectibleData => _collectibleData;

        private void Awake()
        {
            _spriteRenderer.sprite = _collectibleData.CollectibleImage;
        }

        private void Start()
        {
            if (CollectiblesManager.Instance != null && CollectiblesManager.Instance.IsCollected(CollectibleData.CollectibleID))
            {
                if (_disableOnCollected)
                    gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Colliding with {other.name}");

            if (!other.CompareTag("Player"))
                return;

            Collect();
        }

        public void Collect()
        {
            if (CollectiblesManager.Instance != null && CollectiblesManager.Instance.RegisterCollected(CollectibleData.CollectibleID))
            {
                if (_disableOnCollected)
                    gameObject.SetActive(false);
            }
        }
    }
}
