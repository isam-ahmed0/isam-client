using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using UnityEngine;

namespace IsamClient.Utils
{
    public static class AssetLoader
    {
        private const string GitHubRepoUrl = "https://github.com/isam-ahmed0/isam-client-assets.git";
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

            if (!Directory.Exists(assetsPath))
                return;

            var pngs = Directory.GetFiles(assetsPath, "*.png", SearchOption.AllDirectories);
            foreach (var png in pngs)
            {
                var fileName = Path.GetFileName(png);
                var spriteName = Path.GetFileNameWithoutExtension(fileName);
                if (!_mapping.ContainsKey(spriteName))
                    _mapping[spriteName] = fileName;
            }

            _initialized = true;
        }

        public static void CloneAssetsRepo()
        {
            var assetsPath = GetAssetsPath();

            if (Directory.Exists(assetsPath) && Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories).Length > 0)
            {
                RefreshMapping();
                return;
            }

            var dataPath = Path.Combine(BepInEx.Paths.GameRootPath, DataFolder);
            var tempClone = Path.Combine(dataPath, "temp_clone");

            try
            {
                Directory.CreateDirectory(dataPath);

                if (Directory.Exists(tempClone))
                    Directory.Delete(tempClone, true);

                var psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"clone {GitHubRepoUrl} \"{tempClone}\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                var process = Process.Start(psi);
                process.WaitForExit(30000);

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    UnityEngine.Debug.LogError($"[isam-client] git clone failed: {error}");
                    return;
                }

                if (Directory.Exists(assetsPath))
                    Directory.Delete(assetsPath, true);

                Directory.Move(tempClone, assetsPath);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[isam-client] Clone error: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempClone))
                        Directory.Delete(tempClone, true);
                }
                catch { }
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

                var fileBytes = File.ReadAllBytes(localFile);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(texture, fileBytes);
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
