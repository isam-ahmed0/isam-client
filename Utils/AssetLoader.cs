using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;

namespace IsamClient.Utils
{
    public static class AssetLoader
    {
        private const string GitHubBaseUrl = "https://raw.githubusercontent.com/isam-ahmed0/isam-client-assets/main/";
        private const string DataFolder = "isam-client_data";
        private const string AssetsSubfolder = "assets";

        private static readonly HttpClient _httpClient = new HttpClient();
        private static Dictionary<string, string> _mapping;
        private static bool _initialized;

        public static string GetAssetsPath()
        {
            return Path.Combine(BepInEx.Paths.GameRootPath, DataFolder, AssetsSubfolder);
        }

        public static Dictionary<string, string> GetMapping()
        {
            if (!_initialized)
                RefreshMapping();
            return _mapping;
        }

        public static void RefreshMapping()
        {
            _mapping = new Dictionary<string, string>();
            var assetsPath = GetAssetsPath();
            Directory.CreateDirectory(assetsPath);

            var pngs = Directory.GetFiles(assetsPath, "*.png");
            foreach (var png in pngs)
            {
                var fileName = Path.GetFileName(png);
                var spriteName = Path.GetFileNameWithoutExtension(fileName);
                if (!_mapping.ContainsKey(spriteName))
                    _mapping[spriteName] = fileName;
            }

            _initialized = true;
        }

        public static void DownloadAllFromGitHub()
        {
            var assetsPath = GetAssetsPath();
            Directory.CreateDirectory(assetsPath);

            string[] knownFiles = { "logoImage.png" };

            foreach (var fileName in knownFiles)
            {
                var localFile = Path.Combine(assetsPath, fileName);
                if (File.Exists(localFile))
                    continue;

                try
                {
                    var url = GitHubBaseUrl + fileName;
                    var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        File.WriteAllBytes(localFile, bytes);
                    }
                }
                catch
                {
                }
            }

            RefreshMapping();
        }

        public static Sprite LoadSpriteFromDisk(string fileName, float pixelsPerUnit = 100f, Vector2? pivot = null)
        {
            var spritePivot = pivot ?? new Vector2(0.5f, 0.5f);

            try
            {
                var assetsPath = GetAssetsPath();
                var localFile = Path.Combine(assetsPath, fileName);
                if (!File.Exists(localFile))
                    return null;

                var bytes = File.ReadAllBytes(localFile);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(texture, bytes);
                texture.filterMode = FilterMode.Point;

                var rect = new Rect(0, 0, texture.width, texture.height);
                return Sprite.Create(texture, rect, spritePivot, pixelsPerUnit);
            }
            catch
            {
                return null;
            }
        }
    }
}
