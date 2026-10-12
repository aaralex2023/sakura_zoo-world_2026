using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZooWorld.Animals;

namespace ZooWorld.UI
{
    public sealed class DeathCounterView : MonoBehaviour
    {
        [InfoBox("Creates a UI panel with one death counter per configured food-chain role and displays count updates.")]
        [SerializeField, ReadOnly] private bool _about;

        private readonly Dictionary<FoodChainRoleId, TMP_Text> _labels = new();
        private bool _isInitialized;

        public void Initialize(IReadOnlyList<FoodChainRoleId> roles, TMP_FontAsset font)
        {
            if (_isInitialized)
                throw new InvalidOperationException($"'{name}' was initialized more than once.");
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            GameObject panel = new("Death Counter Panel", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            panel.transform.SetParent(transform, false);

            RectTransform panelRect = (RectTransform)panel.transform;
            panelRect.anchorMin = Vector2.one;
            panelRect.anchorMax = Vector2.one;
            panelRect.pivot = Vector2.one;
            panelRect.anchoredPosition = new Vector2(-24f, -24f);
            panelRect.sizeDelta = new Vector2(280f, 0f);

            Image background = panel.GetComponent<Image>();
            background.color = new Color(0.04f, 0.07f, 0.06f, 0.82f);

            VerticalLayoutGroup layout = panel.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 4f;
            layout.childAlignment = TextAnchor.UpperRight;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = panel.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (FoodChainRoleId role in roles)
            {
                GameObject row = new(role.DisplayName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(LayoutElement));
                row.transform.SetParent(panel.transform, false);

                TMP_Text label = row.GetComponent<TMP_Text>();
                label.font = font;
                label.fontSize = 26f;
                label.fontStyle = FontStyles.Bold;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Right;
                label.text = $"{role.DisplayName}: 0";

                LayoutElement rowLayout = row.GetComponent<LayoutElement>();
                rowLayout.minHeight = 36f;
                rowLayout.preferredHeight = 36f;
                _labels.Add(role, label);
            }

            _isInitialized = true;
        }

        public void SetCount(FoodChainRoleId role, string text)
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"'{name}' has not been initialized.");
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (_labels.TryGetValue(role, out TMP_Text label))
                label.text = text;
        }
    }
}
