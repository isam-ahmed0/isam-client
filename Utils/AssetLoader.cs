using System;
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

        public static string GetAssetsPath()
        {
            return Path.Combine(BepInEx.Paths.GameRootPath, DataFolder, AssetsSubfolder);
        }

        public static Sprite LoadSpriteFromDisk(string fileName, float pixelsPerUnit = 100f, Vector2? pivot = null)
        {
            var spritePivot = pivot ?? new Vector2(0.5f, 0.5f);

            try
            {
                var bytes = LoadAssetBytes(fileName);
                if (bytes == null)
                    return null;

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

        private static byte[] LoadAssetBytes(string fileName)
        {
            var assetsPath = GetAssetsPath();
            var localFile = Path.Combine(assetsPath, fileName);

            // 1. Already downloaded or user-placed — use it
            if (File.Exists(localFile))
                return File.ReadAllBytes(localFile);

            // 2. Download from GitHub and save to assets/
            try
            {
                Directory.CreateDirectory(assetsPath);
                var url = GitHubBaseUrl + fileName;
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    File.WriteAllBytes(localFile, bytes);
                    return bytes;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
