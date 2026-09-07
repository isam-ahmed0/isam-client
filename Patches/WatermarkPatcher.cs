using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace IsamClient.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class WatermarkPatcher
    {
        private const string WatermarkText = "isam-client v1.0.0";

        [HarmonyPostfix]
        public static void Postfix()
        {
            try
            {
                var go = new GameObject("IsamClientWatermark");
                Object.DontDestroyOnLoad(go);

                var canvas = go.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;

                go.AddComponent<CanvasScaler>();
                go.AddComponent<GraphicRaycaster>();

                var textGo = new GameObject("WatermarkText");
                textGo.transform.SetParent(go.transform, false);

                var textRect = textGo.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;

                var text = textGo.AddComponent<TMPro.TextMeshProUGUI>();
                text.text = WatermarkText;
                text.fontSize = 20f;
                text.color = new Color(1f, 1f, 1f, 0.85f);
                text.alignment = TMPro.TextAlignmentOptions.TopLeft;
                text.enableAutoSizing = false;
                text.overflowMode = TMPro.TextOverflowModes.Overflow;
                text.raycastTarget = false;
            }
            catch
            {
            }
        }
    }
}
