using UnityEngine;

namespace PixelMate.Collectibles
{
    [CreateAssetMenu(fileName = "New Collectible Data", menuName = "PixelMate/Collectible Data", order = 0)]
    public sealed class CollectibleData : ScriptableObject
    {
        [SerializeField]
        private string _collectibleID;
        [SerializeField]
        private Sprite _collectibleImage;
        [SerializeField]
        private string _collectibleName;
        [SerializeField, TextArea()]
        private string _collectibleDescription;

        public string CollectibleID => _collectibleID;
        public Sprite CollectibleImage => _collectibleImage;
        public string CollectibleName => _collectibleName;
        public string CollectibleDescription => _collectibleDescription;
    }
}
