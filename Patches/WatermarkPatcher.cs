using System;
using UnityEngine;
using UnityEngine.UI;

namespace IsamClient.Patches
{
    public class WatermarkRenderer : MonoBehaviour
    {
        private const string WatermarkText = "isam-client v1.0.0";

        private bool _created;
        private float _timer;
        private const float CheckInterval = 1f;

        public WatermarkRenderer(IntPtr ptr) : base(ptr) { }

        private void Update()
        {
            if (_created)
                return;

            _timer += Time.deltaTime;
            if (_timer < CheckInterval)
                return;
            _timer = 0f;

            try
            {
                var menu = FindObjectOfType<MainMenuManager>();
                if (menu == null)
                    return;

                CreateWatermark();
                _created = true;
            }
            catch
            {
            }
        }

        private void CreateWatermark()
        {
            var go = new GameObject("IsamClientWatermark");
            UnityEngine.Object.DontDestroyOnLoad(go);

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
    }
}
