using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 화면 위에 올리는 것들(UGUI)을 만드는 도구.
    ///
    /// 23차시의 "한 발 쏘기" 버튼부터 26차시의 난이도 버튼 세 개까지 여기서 만듭니다.
    /// </summary>
    public static class BuildKitUi
    {
        const string KoreanFontPath =
            "Assets/99. External Assets/Font/Noto_Sans_KR/static/NotoSansKR-Regular SDF.asset";

        /// <summary>
        /// 화면 판(Canvas)을 찾거나 만듭니다. 안내데스크(EventSystem)도 같이 챙깁니다.
        ///
        /// <paramref name="scaleWithScreen"/> — <b>화면 크기에 맞춰 늘어나게 할지.</b>
        /// <list type="bullet">
        /// <item><b>갈래 B · C (22차시 이후)는 <c>true</c></b>. 이미 배운 내용이라 처음부터 맞춰둡니다</item>
        /// <item><b>갈래 A 의 14차시는 <c>false</c></b> — <b>17차시가 이걸 바꾸는 실습</b>이라서요.
        ///       미리 맞춰두면 <c>17_시작</c> 과 <c>17_완성</c> 이 똑같아져
        ///       <b>그 차시가 무엇을 했는지 안 보입니다.</b></item>
        /// </list>
        /// </summary>
        public static Canvas Root(string name = "Canvas", bool scaleWithScreen = true)
        {
            Canvas existing = Object.FindFirstObjectByType<Canvas>();
            if (existing != null) return existing;

            GameObject go = BuildKit.Empty(name);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.AddComponent<CanvasScaler>();
            if (scaleWithScreen) ScaleWithScreen(scaler);

            go.AddComponent<GraphicRaycaster>();
            EnsureEventSystem();

            EditorUtility.SetDirty(go);
            return canvas;
        }

        /// <summary>
        /// 차시별 <b>화면 묶음</b>을 찾거나 만듭니다.
        ///
        /// <b>왜 필요한가</b> — 갈래 A는 아홉 차시가 한 판 위에 화면 요소를 쌓습니다.
        /// 21차시쯤 되면 <c>Canvas</c> 자식이 <b>열일곱 개</b>가 되어 무엇이 무엇인지 안 보입니다.
        ///
        /// 차시마다 자기 그릇에 담아두면 <b>안 쓰는 차시 것을 통째로 접을 수 있습니다.</b>
        /// <b>지우는 게 아니라 접는 것</b>이라, 체크 한 번이면 그대로 돌아옵니다.
        ///
        /// 그릇은 <b>판 전체를 덮게</b> 만듭니다.
        /// 그래야 안에 든 것이 «화면 왼쪽 위» 같은 자리를 <b>그대로 쓸 수 있습니다.</b>
        /// </summary>
        public static RectTransform Group(Canvas canvas, string name)
        {
            Transform found = canvas.transform.Find(name);
            if (found != null) return (RectTransform)found;

            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);

            var rt = go.GetComponent<RectTransform>();
            rt.Stretch();

            Undo.RegisterCreatedObjectUndo(go, $"{name} 만들기");
            EditorUtility.SetDirty(go);
            return rt;
        }

        /// <summary>
        /// 묶음을 <b>켜고 끕니다.</b> <paramref name="visible"/> 에 없는 것은 접힙니다.
        ///
        /// 아직 안 만들어진 묶음은 <b>그냥 넘어갑니다.</b>
        /// (14차시 시점에는 19 · 20차시 묶음이 없습니다)
        ///
        /// <b>묶음에 안 든 것은 건드리지 않습니다.</b>
        /// <c>HealthBar</c> · <c>ScoreBox</c> 는 17차시부터 끝까지 켜져 있어야 해서 일부러 안 묶었습니다.
        /// </summary>
        public static void SetGroupsActive(Canvas canvas, string[] all, string[] visible)
        {
            foreach (string name in all)
            {
                Transform group = canvas.transform.Find(name);
                if (group == null) continue;

                bool on = System.Array.IndexOf(visible, name) >= 0;
                if (group.gameObject.activeSelf == on) continue;

                group.gameObject.SetActive(on);
                EditorUtility.SetDirty(group.gameObject);
            }
        }

        /// <summary>
        /// 화면 크기가 바뀌어도 안 깨지게 맞춥니다. <b>17차시 실습 ②가 이걸 손으로 합니다.</b>
        /// </summary>
        public static void ScaleWithScreen(CanvasScaler scaler)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            EditorUtility.SetDirty(scaler);
        }

        /// <summary>
        /// 안내데스크가 없으면 만듭니다. 19차시에서 배우는 그 부품입니다.
        /// 이 프로젝트는 새 Input System 전용이라 그에 맞는 것을 붙입니다.
        /// </summary>
        public static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;

            GameObject go = BuildKit.Empty("EventSystem");
            go.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
            go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            go.AddComponent<StandaloneInputModule>();
#endif
            EditorUtility.SetDirty(go);
        }

        /// <summary>
        /// 한글 글꼴을 돌려줍니다. 없으면 글자가 네모(☐)로 나옵니다.
        /// </summary>
        public static TMP_FontAsset KoreanFont()
        {
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(KoreanFontPath);
            if (font == null)
            {
                Debug.LogWarning(
                    "한글 글꼴을 못 찾았습니다. 버튼 글자가 네모(☐)로 나올 수 있습니다.\n" +
                    KoreanFontPath);
            }
            return font;
        }

        /// <summary>글자를 하나 올립니다.</summary>
        public static TextMeshProUGUI Text(string name, string content, Transform parent,
                                           Vector2 anchoredPos, Vector2 size,
                                           float fontSize = 36f)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            TMP_FontAsset font = KoreanFont();
            if (font != null) tmp.font = font;

            Undo.RegisterCreatedObjectUndo(go, $"{name} 만들기");
            EditorUtility.SetDirty(go);
            return tmp;
        }

        /// <summary>화면을 덮는 판을 만듭니다. 시작 화면처럼 위에 올려두는 것에 씁니다.</summary>
        public static Image Panel(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var image = go.AddComponent<Image>();
            image.color = color;

            Undo.RegisterCreatedObjectUndo(go, $"{name} 만들기");
            EditorUtility.SetDirty(go);
            return image;
        }

        /// <summary>
        /// 단순한 사각형을 하나 올립니다. 십자선 · 배경판 · HUD 조각에 씁니다.
        ///
        /// <paramref name="blocksClick"/> — <b>이 그림이 클릭을 가로챌지</b>(<c>Raycast Target</c>).
        /// 십자선은 <c>false</c>(안 가로챔), 17차시 HUD·19차시 <c>Cover</c> 는 <c>true</c> 입니다.
        /// 19차시 실습 ②가 바로 이 값을 껐다 켜는 실습입니다.
        /// </summary>
        public static Image Box(string name, Transform parent,
                                Vector2 anchoredPos, Vector2 size, Color color,
                                bool blocksClick = false)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;

            var image = go.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = blocksClick;

            Undo.RegisterCreatedObjectUndo(go, $"{name} 만들기");
            EditorUtility.SetDirty(go);
            return image;
        }

        /// <summary>
        /// 끌어서 값을 정하는 슬라이더를 만듭니다. 18차시의 주인공입니다.
        ///
        /// 유니티가 <c>UI ▸ Slider</c> 로 만드는 것과 같은 구조로 짭니다.
        /// <c>Background</c> · <c>Fill Area ▸ Fill</c> · <c>Handle Slide Area ▸ Handle</c>
        /// </summary>
        public static Slider Slider(string name, Transform parent,
                                    Vector2 anchoredPos, Vector2 size,
                                    float min, float max, float value, bool wholeNumbers)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;

            Image background = Stretch(
                Box("Background", go.transform, Vector2.zero, Vector2.zero,
                    new Color(0.85f, 0.85f, 0.85f), true));

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(go.transform, false);
            StretchRect(fillArea.GetComponent<RectTransform>(), new Vector2(10f, 0f));

            Image fill = Stretch(
                Box("Fill", fillArea.transform, Vector2.zero, Vector2.zero,
                    new Color(0.3f, 0.6f, 0.95f), true));

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(go.transform, false);
            StretchRect(handleArea.GetComponent<RectTransform>(), new Vector2(10f, 0f));

            Image handle = Box("Handle", handleArea.transform, Vector2.zero,
                               new Vector2(20f, 0f), Color.white, true);
            var handleRt = handle.GetComponent<RectTransform>();
            handleRt.anchorMin = new Vector2(0f, 0f);
            handleRt.anchorMax = new Vector2(0f, 1f);
            handleRt.offsetMin = new Vector2(handleRt.offsetMin.x, 0f);
            handleRt.offsetMax = new Vector2(handleRt.offsetMax.x, 0f);

            var slider = go.AddComponent<Slider>();
            slider.targetGraphic = handle;
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handleRt;
            slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;

            // 순서가 중요합니다 — wholeNumbers 와 최소·최대를 먼저 정해야 value 가 안 잘립니다
            slider.wholeNumbers = wholeNumbers;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;

            // onValueChanged 는 유니티가 스스로 초기화합니다 (Button.onClick 과 같음).
            // 강의자료 부품처럼 InitEventFields 로 채워줄 필요가 없습니다.

            Undo.RegisterCreatedObjectUndo(go, $"{name} 만들기");
            EditorUtility.SetDirty(go);
            return slider;
        }

        /// <summary>누를 수 있는 버튼을 하나 만듭니다.</summary>
        public static Button Button(string name, string label, Transform parent,
                                    Vector2 anchoredPos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.92f, 0.92f, 0.92f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            TextMeshProUGUI text = Text("Text (TMP)", label, go.transform,
                                        Vector2.zero, size, 30f);
            text.color = Color.black;

            // 글자가 버튼을 꽉 채우게 합니다
            var trt = text.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            Undo.RegisterCreatedObjectUndo(go, $"{name} 만들기");
            EditorUtility.SetDirty(go);
            return button;
        }

        /// <summary>
        /// 화면 어느 구석에 <b>압정을 꽂을지</b> 정합니다. 17차시의 앵커입니다.
        ///
        /// 여기 것은 <b>한 점에 꽂는</b> 것입니다 — <c>top-left</c> · <c>top-right</c> 같은.
        /// 늘어나야 하면 <see cref="StretchX"/> · <see cref="Stretch{T}"/> 를 쓰세요.
        /// </summary>
        public static T Anchor<T>(this T ui, Vector2 anchor) where T : Component
        {
            var rt = ui.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            EditorUtility.SetDirty(ui);
            return ui;
        }

        /// <summary>
        /// <b>가로로 늘어나는</b> 앵커. 17차시의 <c>TopBar</c> · <c>BottomBar</c> 입니다.
        ///
        /// 화면이 넓어지면 <b>같이 넓어지고</b>, 높이는 <paramref name="height"/> 로 고정됩니다.
        /// <paramref name="top"/> 이 참이면 위쪽에, 거짓이면 아래쪽에 붙습니다.
        /// </summary>
        public static T StretchX<T>(this T ui, bool top, float height,
                                    float margin = 0f) where T : Component
        {
            var rt = ui.GetComponent<RectTransform>();
            float y = top ? 1f : 0f;

            rt.anchorMin = new Vector2(0f, y);
            rt.anchorMax = new Vector2(1f, y);
            rt.pivot = new Vector2(0.5f, y);

            rt.offsetMin = new Vector2(0f, top ? -height - margin : margin);
            rt.offsetMax = new Vector2(0f, top ? -margin : height + margin);

            EditorUtility.SetDirty(ui);
            return ui;
        }

        /// <summary>
        /// <b>사방으로 늘어나는</b> 앵커 (<c>stretch-stretch</c>).
        /// 19차시 실습 ②의 <c>Cover</c> 처럼 <b>화면 전체를 덮는</b> 것에 씁니다.
        /// </summary>
        public static T Stretch<T>(this T ui, float margin = 0f) where T : Component
        {
            StretchRect(ui.GetComponent<RectTransform>(), new Vector2(margin, margin));
            EditorUtility.SetDirty(ui);
            return ui;
        }

        static void StretchRect(RectTransform rt, Vector2 margin)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = margin;
            rt.offsetMax = -margin;
        }
    }
}
