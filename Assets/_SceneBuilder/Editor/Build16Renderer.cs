using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 16차시 — 화면에 그리는 부품 세 가지를 갈아 끼우기.
    ///
    /// 교안 `16_renderer_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ <c>Display</c> 안에 입체 · 납작한 그림 · 효과를 겹쳐 놓고 버튼 넷으로 갈아 끼우기</item>
    /// </list>
    ///
    /// <b>실습 ① · ② · ④는 만들지 않습니다.</b>
    /// <list type="bullet">
    /// <item>①은 체크를 껐다 켜보는 실습입니다. 게다가 여기서 만드는 큐브가 <b>원점</b>에 생겨
    ///       <b>14차시 자동차 안에 박힙니다.</b> 바닥(Plane)도 21차시 것과 겹칩니다</item>
    /// <item>②는 <c>Metallic</c> · <c>Smoothness</c> 를 움직여보는 실습입니다</item>
    /// <item>④는 <c>Order in Layer</c> 로 앞뒤를 바꿔보는 실습입니다</item>
    /// </list>
    /// </summary>
    public static class Build16Renderer
    {
        const string FromScene = "15_완성";
        const string StartScene = "16_시작";
        const string CompleteScene = "16_완성";

        [MenuItem("Tools/교안 씬 빌더/16차시 — 세 가지를 갈아 끼우기", false, 16)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "16차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「기초 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFrom(FromScene, StartScene);

            RendererSwitcher switcher = BuildDisplay();
            BuildModeButtons(switcher);

            BuildBasicsLayout.CameraFor(16, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 1 · 2단계 — 세 가지를 한자리에 겹쳐 놓기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 빈 그릇 하나에 <b>세 가지를 같은 자리에 겹쳐</b> 넣습니다.
        ///
        /// 셋 다 «화면에 그린다» 는 일을 하지만 방식이 다릅니다 —
        /// 입체는 <b>덩어리</b>로, 그림은 <b>납작한 판</b>으로, 효과는 <b>알갱이 수백 개</b>로.
        ///
        /// 교안은 <c>Display</c> 를 <c>0, 1, 0</c> 에 두라고 하지만
        /// <b>거기는 14차시 자동차 자리</b>라서 옆으로 옮겼습니다. (`BuildBasicsLayout` 참고)
        /// </summary>
        /// <summary>
        /// 납작한 그림을 몇 칸으로 보이게 할지.
        ///
        /// 큐브가 1칸이라 <b>조금 크게</b> 잡았습니다.
        /// 같은 크기면 정면에서 봤을 때 <b>둘 다 그냥 흰 사각형</b>이라 구분이 안 됩니다.
        /// </summary>
        const float SpriteSize = 1.5f;

        static RendererSwitcher BuildDisplay()
        {
            GameObject display = BuildKit.Empty("Display").At(BuildBasicsLayout.Display);

            // 세 개 다 부모 기준 0,0,0 — 같은 자리에 겹칩니다. 교안 실습 ③ 5번.
            GameObject mesh = BuildKit.Shape("ShowMesh", PrimitiveType.Cube, display.transform)
                                      .At(Vector3.zero);

            // 큐브(1칸)보다 조금 크게 잡습니다. 갈아 끼웠을 때 «납작한 판» 이 확실히 보이게요.
            GameObject sprite = BuildKit.Sprite2D("ShowSprite", display.transform, SpriteSize)
                                        .gameObject.At(Vector3.zero);

            GameObject particle = BuildKit.Particle("ShowParticle", new Color(1f, 0.75f, 0.2f),
                                                    display.transform).gameObject
                                          .At(Vector3.zero);

            var switcher = BuildKit.Add<RendererSwitcher>(display);
            switcher.meshObject = mesh;
            switcher.spriteObject = sprite;
            switcher.particleObject = particle;

            // 처음 열었을 때 입체가 보이게 둡니다. 버튼을 눌러 갈아 끼우는 게 실습이니까요.
            switcher.showMode = RendererSwitcher.ShowMode.Mesh;
            EditorUtility.SetDirty(switcher);

            return switcher;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 4단계 — 버튼 네 개
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 버튼 넷을 만들어 갈아 끼우는 동작에 겁니다. 교안 실습 ③ 13번 표 그대로입니다.
        ///
        /// 연결 방식은 <b>14차시와 똑같습니다</b> —
        /// <b>누가</b>(<c>Display</c> 를 끌어다 놓기) — <b>무엇을</b>(목록에서 고르기).
        /// </summary>
        static void BuildModeButtons(RendererSwitcher switcher)
        {
            Canvas canvas = BuildKitUi.Root(scaleWithScreen: false);
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L16);

            (string name, string label, UnityEngine.Events.UnityAction call)[] buttons =
            {
                ("MeshButton",     "입체", switcher.ShowMesh),
                ("SpriteButton",   "그림", switcher.ShowSprite),
                ("ParticleButton", "효과", switcher.PlayParticle),
                ("HideButton",     "끄기", switcher.HideAll),
            };

            float[] ys = BuildBasicsLayout.ModeButtonY;

            for (int i = 0; i < buttons.Length; i++)
            {
                (string name, string label, UnityEngine.Events.UnityAction call) = buttons[i];

                Button b = BuildKitUi.Button(
                    name, label, group,
                    new Vector2(BuildBasicsLayout.SideColumnX, ys[i]),
                    BuildBasicsLayout.SideButton);
                b.Anchor(BuildBasicsLayout.MiddleLeft);

                BuildKit.Wire(b.onClick, call);
            }

            BuildBasicsLayout.ApplyUiGroups(canvas, 16);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var switcher = Object.FindFirstObjectByType<RendererSwitcher>();
            int filled = switcher == null ? 0
                : (switcher.meshObject != null ? 1 : 0)
                + (switcher.spriteObject != null ? 1 : 0)
                + (switcher.particleObject != null ? 1 : 0);

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 16차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"갈아 끼울 칸 세 개 중 {filled}개가 채워졌습니다. (3개면 정상)\n" +
                    "▶ 를 누르고 «입체 · 그림 · 효과 · 끄기» 를 눌러보세요.\n" +
                    "«효과» 는 여러 번 눌러도 매번 새로 터져야 합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 16차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
