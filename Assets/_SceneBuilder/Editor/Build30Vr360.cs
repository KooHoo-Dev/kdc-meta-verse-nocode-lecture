using System.Collections.Generic;
using NoCodeKit.EditorTools;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 30차시 — 360 배경으로 세상 만들기.
    ///
    /// 교안 `30_vr360_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ 3단계 <b>하늘 재료를 씌우기</b></item>
    /// <item>실습 ④ <c>WorldManager</c> + <c>SkyboxSwitcher</c> + «다음 배경» 버튼 + 이름 글자</item>
    /// </list>
    ///
    /// <b>실습 ① · ②는 씬에 안 남습니다.</b> 하늘을 둘러보고 값을 만져보는 실습입니다.
    /// </summary>
    public static class Build30Vr360
    {
        const string FromScene = "29_완성";
        const string StartScene = "30_시작";
        const string CompleteScene = "30_완성";

        [MenuItem("Tools/교안 씬 빌더/30차시 — 360 배경으로 세상 만들기", false, 30)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "30차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「VR 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFrom(FromScene, StartScene);

            SkyboxSwitcher.Sky[] skies = BuildSkies();
            RenderSettings.skybox = skies[0].material;

            SkyboxSwitcher switcher = BuildWorldManager(skies);
            BuildSkyUi(switcher);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 하늘 재료
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 하늘 셋을 만듭니다.
        ///
        /// <b>🟨 교안은 «360 사진» 으로 만들라고 합니다.</b> (실습 ③ 1 · 2단계 — <c>Panoramic</c> 재료)
        /// 그런데 <b>그 사진이 아직 없습니다.</b> (`plans/11` ⑥번)
        ///
        /// 그래서 <b>유니티 내장 절차적 하늘</b>로 셋을 만듭니다.
        /// 27차시가 노을을 만든 것과 같은 방식이에요.
        ///
        /// | | |
        /// | 실습 ③ (사진으로 만들기) | 🟨 <b>사진이 오면 다시 씁니다</b> |
        /// | 실습 ④ (버튼으로 갈아 끼우기) | ✅ <b>그대로 성립합니다</b> — 재료가 사진이든 아니든 같습니다 |
        ///
        /// 사진이 준비되면 <b>이 함수만</b> `Panoramic` 재료를 만들게 고치면 됩니다.
        /// 뒤쪽(버튼 · 글자 연결)은 손댈 것이 없습니다.
        /// </summary>
        static SkyboxSwitcher.Sky[] BuildSkies()
        {
            return new[]
            {
                Sky("낮", BuildKit.SkyboxMat("Sky_Day",
                    tint: new Color(0.55f, 0.62f, 0.72f),
                    ground: new Color(0.38f, 0.38f, 0.40f),
                    thickness: 1.0f, exposure: 1.30f)),

                Sky("노을", BuildKit.SkyboxMat("Sky_Sunset30",
                    tint: new Color(0.90f, 0.58f, 0.40f),
                    ground: new Color(0.30f, 0.24f, 0.20f),
                    thickness: 2.2f, exposure: 1.10f)),

                Sky("밤", BuildKit.SkyboxMat("Sky_Night",
                    tint: new Color(0.18f, 0.22f, 0.40f),
                    ground: new Color(0.08f, 0.09f, 0.14f),
                    thickness: 0.4f, exposure: 0.55f)),
            };
        }

        static SkyboxSwitcher.Sky Sky(string label, Material material)
        {
            return new SkyboxSwitcher.Sky { label = label, material = material };
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 버튼으로 세상 바꾸기
        // ══════════════════════════════════════════════════════

        static SkyboxSwitcher BuildWorldManager(SkyboxSwitcher.Sky[] skies)
        {
            GameObject manager = BuildKit.EnsureEmpty("WorldManager");

            var switcher = BuildKit.Add<SkyboxSwitcher>(manager);
            switcher.skyboxes = skies;
            switcher.startIndex = 0;
            EditorUtility.SetDirty(switcher);

            return switcher;
        }

        /// <summary>
        /// «다음 배경» 버튼과 <b>지금 배경 이름</b>을 만듭니다.
        ///
        /// 이름 글자는 <b>목록 위쪽 <c>Dynamic string</c></b> 으로 겁니다.
        /// 26차시 난이도 이름을 표시할 때 쓴 그 방식이에요 — <b>넘겨주는 게 글자면 string.</b>
        ///
        /// ⚠️ <b>여기 화면 요소는 «고글 흉내» 에서만 보입니다.</b>
        /// 판(Canvas)이 <c>Screen Space - Overlay</c> 라 <b>실제 고글에서는 안 보입니다.</b>
        /// 30차시는 시뮬레이터로 확인하는 차시라 문제가 없지만,
        /// <b>32차시는 이걸 세상 안의 물건으로 옮겨야 합니다.</b> (`plans/08` §6-1)
        /// </summary>
        static void BuildSkyUi(SkyboxSwitcher switcher)
        {
            Canvas canvas = BuildKitUi.Root();

            Button next = BuildKitUi.Button(
                "NextSkyButton", "다음 배경", canvas.transform,
                BuildXrLayout.NextSkyButton, BuildXrLayout.NextSkyButtonSize);
            next.Anchor(BuildBasicsLayout.BottomCenter);

            BuildKit.Wire(next.onClick, switcher.Next);

            TextMeshProUGUI label = BuildKitUi.Text(
                "SkyLabel", "낮", canvas.transform,
                BuildXrLayout.SkyLabel, BuildXrLayout.SkyLabelSize, 40f);
            label.Anchor(BuildBasicsLayout.BottomCenter);

            BuildKit.WireDynamicProperty(switcher.onSkyboxChanged, label, "text");
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var switcher = Object.FindFirstObjectByType<SkyboxSwitcher>();
            int count = switcher != null ? switcher.skyboxes.Length : -1;
            int wired = switcher != null ? switcher.onSkyboxChanged.GetPersistentEventCount() : -1;
            bool sky = RenderSettings.skybox != null;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 30차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"갈아 끼울 하늘: {count}개 (3개면 정상) · 하늘 씌움: {(sky ? "네" : "❌ 아니오")}\n" +
                    $"이름 글자 연결: {wired}개 (1개면 정상 · Dynamic string)\n" +
                    "▶ 를 누르고 Game 창 클릭 → «다음 배경» 을 눌러보세요.\n" +
                    "하늘과 글자가 같이 바뀌고, 기둥에 닿는 빛도 달라져야 합니다.\n" +
                    "🟨 교안의 «360 사진» 은 아직 없어서 내장 하늘로 만들었습니다. (plans/11 ⑥번)");
                return;
            }

            Debug.LogWarning($"⚠️ 30차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
