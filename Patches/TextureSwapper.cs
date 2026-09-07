using System;
using System.Collections.Generic;
using UnityEngine;

namespace IsamClient.Patches
{
    public class TextureSwapper : MonoBehaviour
    {
        private Dictionary<string, string> _mapping;
        private float _timer;
        private const float ScanInterval = 0.5f;
        private bool _downloaded;
        private bool _loggedMapping;

        public TextureSwapper(IntPtr ptr) : base(ptr) { }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < ScanInterval)
                return;
            _timer = 0f;

            try
            {
                if (!_downloaded)
                {
                    Debug.Log("[isam-client] Cloning assets repo...");
                    Utils.AssetLoader.CloneAssetsRepo();
                    _downloaded = true;
                    Debug.Log("[isam-client] Assets repo cloned");
                }

                if (_mapping == null || _mapping.Count == 0)
                    _mapping = Utils.AssetLoader.GetMapping();

                if (_mapping == null || _mapping.Count == 0)
                {
                    if (!_loggedMapping)
                    {
                        Debug.Log("[isam-client] No replacement textures found in assets folder");
                        _loggedMapping = true;
                    }
                    return;
                }

                if (!_loggedMapping)
                {
                    Debug.Log($"[isam-client] Found {_mapping.Count} replacement texture(s): {string.Join(", ", _mapping.Keys)}");
                    _loggedMapping = true;
                }

                var renderers = FindObjectsOfType<SpriteRenderer>();
                foreach (var sr in renderers)
                {
                    if (sr == null || sr.sprite == null)
                        continue;

                    var spriteName = sr.sprite.name;
                    if (!_mapping.TryGetValue(spriteName, out var fileName))
                        continue;

                    var replacement = Utils.AssetLoader.LoadSpriteFromDisk(fileName);
                    if (replacement != null)
                    {
                        sr.sprite = replacement;
                        Debug.Log($"[isam-client] Replaced sprite: {spriteName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[isam-client] TextureSwapper error: {ex.Message}");
            }
        }
    }
}
