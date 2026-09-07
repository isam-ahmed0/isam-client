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
                    Utils.AssetLoader.DownloadAllFromGitHub();
                    _downloaded = true;
                }

                if (_mapping == null || _mapping.Count == 0)
                    _mapping = Utils.AssetLoader.GetMapping();

                if (_mapping == null || _mapping.Count == 0)
                    return;

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
                        sr.sprite = replacement;
                }
            }
            catch
            {
            }
        }
    }
}
