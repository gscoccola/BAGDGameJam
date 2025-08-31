using System.Collections.Generic;
using UnityEngine;

namespace PixelMate.Collectibles
{
    [DisallowMultipleComponent]
    public sealed class CollectiblesManager : Singleton<CollectiblesManager>
    {
        [SerializeField]
        private CustomAudioClip _collectSound;

        private const string Key = "COLLECTIBLES_A2";
        private readonly HashSet<string> _collected = new();

        public int CollectedCount => _collected.Count;
        public bool IsCollected(string id) => _collected.Contains(id);

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            Load();

            Debug.Log($"GetString in Awake: {PlayerPrefs.GetString(Key, "")}");
        }

#if UNITY_EDITOR
        private void Update()
        {
            // For testing purposes only: clear all collected items when pressing '*'
            if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
            {
                Debug.Log("Clearing collectibles for testing");
                _collected.Clear();
                PlayerPrefs.SetString(Key, "");
                PlayerPrefs.Save();
            }
        }
#endif

        // The following 3 methods are included for safety reasons in WebGl, to ensure data is saved properly
        private void OnApplicationPause(bool pause)
        {
            if (pause)
                PlayerPrefs.Save();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
                PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

        #endregion

        public bool RegisterCollected(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if (!_collected.Add(id))
                return false; // already had it

            AudioSystem.Instance?.PlaySound(_collectSound);

            Save();
            return true;
        }

        private void Load()
        {
            _collected.Clear();
            string raw = PlayerPrefs.GetString(Key, "");

            if (string.IsNullOrEmpty(raw))
                return;

            Debug.Log($"Raw collected IDs: '{raw}'");

            foreach (var id in raw.Split(','))
            {
                Debug.Log($"Loaded collected ID: '{id}'");

                if (!string.IsNullOrWhiteSpace(id))
                    _collected.Add(id.Trim());
            }
        }

        private void Save()
        {
            string raw = string.Join(",", _collected);
            PlayerPrefs.SetString(Key, raw);
            PlayerPrefs.Save();

            Debug.Log($"Saved collected raw IDs: '{raw}'. GetString: {PlayerPrefs.GetString(Key, "")}");
        }
    }
}
