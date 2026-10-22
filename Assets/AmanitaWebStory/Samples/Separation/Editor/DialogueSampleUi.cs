using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AmanitaWebStory.Editor
{
    internal static class DialogueSampleUi
    {
        private static readonly Color Ink = new Color(0.08f, 0.12f, 0.19f);
        private static readonly Color Paper = new Color(0.93f, 0.94f, 0.90f);
        private static readonly Color Accent = new Color(0.28f, 0.76f, 0.65f);

        #region 预制体

        public static GameObject CreateView(string Path, TMP_FontAsset Font, bool Radio)
        {
            var Root = new GameObject(Radio ? "通讯界面" : "对白界面");
            try
            {
                var View = Root.AddComponent<UguiDialogueView>();
                var Canvas = CanvasObject(Root.transform, "Canvas");
                var Surface = Rect(Canvas.transform, "Surface", Vector2.zero, Vector2.one);
                var Backdrop = Panel(Surface, "StoryBackground", new Vector2(0, 0), new Vector2(1, 1), Color.white);
                var Card = Panel(Surface, "Card", Radio ? new Vector2(0.53f, 0.07f) : new Vector2(0.07f, 0.07f),
                    Radio ? new Vector2(0.95f, 0.77f) : new Vector2(0.93f, 0.57f), Radio ? Paper : Ink);
                var ContentColor = Radio ? Ink : Paper;
                Label(Card.transform, "Caption", Radio ? "通讯频道 / 城门" : "城门 · 对话", Font,
                    new Vector2(0.06f, 0.87f), new Vector2(0.94f, 0.96f), 20, Radio ? Ink : Accent);
                var Speaker = Label(Card.transform, "Speaker", "", Font, new Vector2(0.06f, 0.76f), new Vector2(0.76f, 0.86f), 26, ContentColor);
                var Line = Label(Card.transform, "Line", "", Font, new Vector2(0.06f, 0.43f), new Vector2(0.80f, 0.74f), 27, ContentColor);
                Line.alignment = TextAlignmentOptions.TopLeft;
                Line.overflowMode = TextOverflowModes.Overflow;
                var Portrait = Panel(Card.transform, "Portrait", new Vector2(0.82f, 0.49f), new Vector2(0.96f, 0.86f), Color.white);
                Portrait.preserveAspect = true;
                var Continue = Button(Card.transform, "Continue", "继续  /  点击补全文字", Font,
                    new Vector2(0.44f, 0.06f), new Vector2(0.94f, 0.18f), Accent, Ink);
                var ScrollRoot = Rect(Card.transform, "Options", new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.40f));
                var Scroll = ScrollRoot.gameObject.AddComponent<ScrollRect>();
                var Viewport = Panel(ScrollRoot, "Viewport", Vector2.zero, Vector2.one, Color.white);
                Viewport.raycastTarget = true;
                Viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
                var Options = Rect(Viewport.transform, "Content", new Vector2(0, 1), new Vector2(1, 1));
                Options.pivot = new Vector2(0.5f, 1);
                Options.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                Scroll.viewport = Viewport.rectTransform;
                Scroll.content = Options;
                Scroll.horizontal = false;
                Scroll.movementType = ScrollRect.MovementType.Clamped;
                Scroll.scrollSensitivity = 24;
                var Layout = Options.gameObject.AddComponent<VerticalLayoutGroup>();
                Layout.spacing = 10;
                Layout.childControlHeight = true;
                Layout.childControlWidth = true;
                Layout.childForceExpandHeight = false;
                Layout.childForceExpandWidth = true;
                var Template = Button(Root.transform, "OptionTemplate", "选项", Font, Vector2.zero, Vector2.one, Accent, Ink);
                var Element = Template.gameObject.AddComponent<LayoutElement>();
                Element.minHeight = 48;
                Element.preferredHeight = 56;
                var Option = Template.gameObject.AddComponent<DialogueOptionButton>();
                Set(Option, "button", Template);
                Set(Option, "label", Template.GetComponentInChildren<TMP_Text>());
                Template.gameObject.SetActive(false);
                var Voice = Root.AddComponent<AudioSource>();
                var Sound = Root.AddComponent<AudioSource>();
                Voice.playOnAwake = Sound.playOnAwake = false;
                Set(View, "surface", Surface.gameObject);
                Set(View, "speakerText", Speaker); Set(View, "lineText", Line);
                Set(View, "portrait", Portrait); Set(View, "background", Backdrop);
                Set(View, "continueButton", Continue); Set(View, "optionRoot", Options);
                Set(View, "optionPanel", ScrollRoot.gameObject);
                Set(View, "optionPrefab", Option); Set(View, "voiceSource", Voice); Set(View, "soundSource", Sound);
                Surface.gameObject.SetActive(false);
                return PrefabUtility.SaveAsPrefabAsset(Root, Path);
            }
            finally { Object.DestroyImmediate(Root); }
        }

        #endregion

        #region 控件

        public static GameObject CanvasObject(Transform Parent, string Name)
        {
            var Root = new GameObject(Name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Root.transform.SetParent(Parent, false);
            Root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var Scaler = Root.GetComponent<CanvasScaler>();
            Scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            Scaler.referenceResolution = new Vector2(1280, 720);
            Scaler.matchWidthOrHeight = 0.5f;
            return Root;
        }

        public static RectTransform Rect(Transform Parent, string Name, Vector2 Min, Vector2 Max)
        {
            var Rect = new GameObject(Name, typeof(RectTransform)).GetComponent<RectTransform>();
            Rect.SetParent(Parent, false);
            Rect.anchorMin = Min; Rect.anchorMax = Max;
            Rect.offsetMin = Rect.offsetMax = Vector2.zero;
            return Rect;
        }

        public static UnityEngine.UI.Image Panel(Transform Parent, string Name, Vector2 Min, Vector2 Max, Color Color)
        {
            var Image = Rect(Parent, Name, Min, Max).gameObject.AddComponent<UnityEngine.UI.Image>();
            Image.color = Color;
            Image.raycastTarget = false;
            return Image;
        }

        public static TMP_Text Label(Transform Parent, string Name, string Text, TMP_FontAsset Font, Vector2 Min, Vector2 Max, float Size, Color Color)
        {
            var Label = Rect(Parent, Name, Min, Max).gameObject.AddComponent<TextMeshProUGUI>();
            Label.font = Font; Label.text = Text; Label.fontSize = Size; Label.color = Color;
            Label.alignment = TextAlignmentOptions.MidlineLeft;
            Label.raycastTarget = false;
            return Label;
        }

        public static UnityEngine.UI.Button Button(Transform Parent, string Name, string Text, TMP_FontAsset Font,
            Vector2 Min, Vector2 Max, Color Fill, Color InkColor)
        {
            var Image = Panel(Parent, Name, Min, Max, Fill);
            Image.raycastTarget = true;
            var Button = Image.gameObject.AddComponent<UnityEngine.UI.Button>();
            Button.targetGraphic = Image;
            var TextLabel = Label(Image.transform, "Label", Text, Font, new Vector2(0.04f, 0), new Vector2(0.96f, 1), 22, InkColor);
            TextLabel.alignment = TextAlignmentOptions.Center;
            return Button;
        }

        public static void Set(object Target, string Field, object Value) { StoryTemplateBuilder.Set(Target, Field, Value); }

        #endregion
    }
}
