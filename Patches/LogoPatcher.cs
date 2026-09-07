using System;
using System.Collections.Generic;
using UnityEngine;

namespace IsamClient.Patches
{
    public class LogoSwapper : MonoBehaviour
    {
        private static readonly Dictionary<string, string> SpriteMappings = new()
        {
            { "title_logo", "title_logo.png" },
            { "InnerslothLogo", "InnerslothLogo.png" },
        };

        private float _timer;
        private const float ScanInterval = 0.5f;

        public LogoSwapper(IntPtr ptr) : base(ptr) { }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < ScanInterval)
                return;
            _timer = 0f;

            try
            {
                var renderers = FindObjectsOfType<SpriteRenderer>();
                foreach (var sr in renderers)
                {
                    if (sr == null || sr.sprite == null)
                        continue;

                    var spriteName = sr.sprite.name;
                    if (!SpriteMappings.TryGetValue(spriteName, out var fileName))
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
