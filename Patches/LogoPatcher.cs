using HarmonyLib;
using UnityEngine;

namespace IsamClient.Patches
{
    [HarmonyPatch(typeof(SpriteRenderer), nameof(SpriteRenderer.OnEnable))]
    public static class LogoPatcher
    {
        private const float DefaultPPU = 100f;
        private static readonly Vector2 DefaultPivot = new Vector2(0.5f, 0.5f);

        [HarmonyPostfix]
        public static void Postfix(SpriteRenderer __instance)
        {
            try
            {
                if (__instance.sprite == null)
                    return;

                var spriteName = __instance.sprite.name;
                string targetFile = null;

                if (spriteName == "title_logo")
                    targetFile = "title_logo.png";
                else if (spriteName == "InnerslothLogo")
                    targetFile = "InnerslothLogo.png";

                if (targetFile == null)
                    return;

                var replacement = Utils.AssetLoader.LoadSpriteFromDisk(targetFile, DefaultPPU, DefaultPivot);
                if (replacement != null)
                    __instance.sprite = replacement;
            }
            catch
            {
            }
        }
    }
}
