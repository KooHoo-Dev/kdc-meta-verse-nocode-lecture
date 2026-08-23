using System.Collections.Generic;
using NoCodeKit.EditorTools;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 26차시 — 난이도와 제한 시간.
    ///
    /// 교안 `26_clay_difficulty_nocode.md` 의 실습 ② ~ ④ 를 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ② 제한 시간 (`RoundTimer` · `On Game End` 에 넷)</item>
    /// <item>실습 ③ 난이도 버튼 세 개 (<b>Dynamic string</b>)</item>
    /// <item>실습 ④ 시작 버튼과 커서 정리 (<b>함정 ②</b> — 발판 → 버튼)</item>
    /// </list>
    ///
    /// 실습 ①(값을 손으로 바꿔보기)은 <b>느껴보는 실습</b>이라 만들 것이 없습니다.
    ///
    /// 이 차시에서 <c>On Game Start</c> 에 <b>여섯 개</b>가 걸립니다.
    /// 18차시의 "칸을 여러 개 만들면 한꺼번에 실행된다" 가 여기서 최대로 회수됩니다.
    /// </summary>
    public static class Build26ClayDifficulty
    {
        const string FromScene = "25_완성";
        const string StartScene = "26_시작";
        const string CompleteScene = "26_완성";

        [MenuItem("Tools/교안 씬 빌더/26차시 — 난이도와 제한 시간", false, 26)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "26차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「사격장 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFrom(FromScene, StartScene);

            Parts p = Collect();

            RoundTimer timer = BuildTimer(p);
            Image startPanel = BuildDifficultyPanel(p, timer);
            BuildStartButton(p, startPanel);
            RemoveLaunchButton();

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  앞 차시에서 만들어둔 것들 모으기
        // ══════════════════════════════════════════════════════

        class Parts
        {
            public GameObject Manager;
            public GameStarter Starter;
            public TargetLauncher Launcher;
            public PlayerMover Mover;
            public MouseLook Look;
            public SimpleGun Gun;
            public Canvas Canvas;
        }

        static Parts Collect()
        {
            var p = new Parts
            {
                Manager = GameObject.Find("GameManager"),
                Starter = Object.FindFirstObjectByType<GameStarter>(),
                Launcher = Object.FindFirstObjectByType<TargetLauncher>(),
                Mover = Object.FindFirstObjectByType<PlayerMover>(),
                Look = Object.FindFirstObjectByType<MouseLook>(),
                Gun = Object.FindFirstObjectByType<SimpleGun>(),
                Canvas = BuildKitUi.Root(),
            };

            if (p.Manager == null) p.Manager = BuildKit.Empty("GameManager");

            if (p.Starter == null || p.Launcher == null || p.Mover == null
                || p.Look == null || p.Gun == null)
            {
                throw new System.InvalidOperationException(
                    "앞 차시에서 만든 부품이 빠져 있습니다. 22 ~ 25차시부터 다시 만들어주세요.");
            }

            return p;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 제한 시간
        // ══════════════════════════════════════════════════════

        static RoundTimer BuildTimer(Parts p)
        {
            var timer = BuildKit.Add<RoundTimer>(p.Manager);
            timer.duration = BuildConfig.RoundDuration;
            timer.autoStart = false;          // 발판을 밟거나 버튼을 눌러야 시작합니다
            EditorUtility.SetDirty(timer);

            TextMeshProUGUI timeText = BuildKitUi.Text(
                "TimeText", "남은 시간: 60초", p.Canvas.transform,
                new Vector2(0f, -40f), new Vector2(420f, 70f), 44f);
            timeText.Anchor(new Vector2(0.5f, 1f));   // 가운데 위

            var display = BuildKit.Add<ValueDisplay>(timeText.gameObject);
            display.prefix = "남은 시간: ";
            display.suffix = "초";
            display.decimals = 0;
            EditorUtility.SetDirty(display);

            // 목록 위쪽 Dynamic float. 아래쪽 고정값을 고르면 숫자가 안 줄어듭니다.
            BuildKit.WireDynamic(timer.onTick, display.SetValue);

            // 시작할 때 시간도 같이 흐르게
            BuildKit.Wire(p.Starter.onGameStart, timer.StartTimer);

            // 시간이 다 되면 게임을 끝냅니다
            BuildKit.Wire(timer.onFinished, p.Starter.EndGame);

            // 끝나면 넷을 한꺼번에 — 18차시의 "칸을 여러 개" 가 여기서 커집니다
            BuildKit.Wire(p.Starter.onGameEnd, p.Launcher.StopLaunching);
            BuildKit.Wire(p.Starter.onGameEnd, p.Gun.DisableFire);
            BuildKit.Wire(p.Starter.onGameEnd, p.Mover.DisableMove);
            BuildKit.Wire(p.Starter.onGameEnd, p.Look.UnlockCursor);

            return timer;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 난이도를 버튼에 담기
        // ══════════════════════════════════════════════════════

        static Image BuildDifficultyPanel(Parts p, RoundTimer timer)
        {
            var preset = BuildKit.Add<DifficultyPreset>(p.Manager);
            preset.launcher = p.Launcher;
            preset.timer = timer;
            EditorUtility.SetDirty(preset);

            Image panel = BuildKitUi.Panel("StartPanel", p.Canvas.transform,
                                           new Color(0f, 0f, 0f, 0.6f));

            TextMeshProUGUI label = BuildKitUi.Text(
                "DifficultyText", "보통", panel.transform,
                new Vector2(0f, 140f), new Vector2(500f, 80f), 52f);

            // 목록 위쪽 Dynamic string. 아래쪽을 고르면 난이도를 바꿔도 글자가 그대로입니다.
            BuildKit.WireDynamicProperty(preset.onDifficultyChanged, label, "text");

            MakeDifficultyButton(panel, "EasyButton", "쉬움", -240f, preset.ApplyEasy);
            MakeDifficultyButton(panel, "NormalButton", "보통", 0f, preset.ApplyNormal);
            MakeDifficultyButton(panel, "HardButton", "어려움", 240f, preset.ApplyHard);

            return panel;
        }

        static void MakeDifficultyButton(Image panel, string name, string label,
                                         float x, UnityEngine.Events.UnityAction apply)
        {
            Button button = BuildKitUi.Button(name, label, panel.transform,
                                              new Vector2(x, 0f), new Vector2(200f, 80f));
            BuildKit.Wire(button.onClick, apply);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 시작 버튼과 커서 정리 (함정 ②)
        // ══════════════════════════════════════════════════════

        static void BuildStartButton(Parts p, Image startPanel)
        {
            // 시작 전에는 못 움직이고 커서가 보여야 버튼을 누를 수 있습니다.
            p.Mover.canMove = false;
            p.Gun.canFire = false;
            p.Look.lockCursor = false;
            EditorUtility.SetDirty(p.Mover);
            EditorUtility.SetDirty(p.Gun);
            EditorUtility.SetDirty(p.Look);

            Button start = BuildKitUi.Button("StartButton", "게임 시작", startPanel.transform,
                                             new Vector2(0f, -140f), new Vector2(300f, 90f));
            BuildKit.Wire(start.onClick, p.Starter.StartGame);

            // 시작하면 풀어줍니다. 여기까지 하면 On Game Start 에 여섯 개가 걸립니다.
            BuildKit.Wire(p.Starter.onGameStart, p.Look.LockCursor);
            BuildKit.Wire(p.Starter.onGameStart, p.Mover.EnableMove);
            BuildKit.Wire(p.Starter.onGameStart, p.Gun.EnableFire);
            BuildKit.WireBool(p.Starter.onGameStart, startPanel.gameObject.SetActive, false);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ 4단계 — 임시로 만든 도구 치우기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 23차시의 "한 발 쏘기" 버튼을 치웁니다.
        ///
        /// 그건 <c>Launch Force</c> 를 맞추려고 만든 <b>개발용 도구</b>였습니다.
        /// 이제 난이도 버튼이 값을 바꿔주니 역할이 끝났고,
        /// 남겨두면 <b>완성한 게임 화면에 개발용 버튼이 보입니다.</b>
        /// </summary>
        static void RemoveLaunchButton()
        {
            foreach (Button b in Object.FindObjectsByType<Button>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                // 그 버튼이 하는 일로 찾습니다. 이름을 바꿔도 잡힙니다.
                for (int i = 0; i < b.onClick.GetPersistentEventCount(); i++)
                {
                    if (b.onClick.GetPersistentMethodName(i) != "LaunchOne") continue;

                    Object.DestroyImmediate(b.gameObject);
                    return;
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            GameStarter starter = Object.FindFirstObjectByType<GameStarter>();
            int onStart = starter != null ? starter.onGameStart.GetPersistentEventCount() : -1;
            int onEnd = starter != null ? starter.onGameEnd.GetPersistentEventCount() : -1;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 26차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"On Game Start 에 {onStart}개, On Game End 에 {onEnd}개가 걸렸습니다. (6개 · 4개면 정상)\n" +
                    "▶ 를 누르고 난이도 고르기 → 게임 시작 → 플레이 순서로 해보세요.");
                return;
            }

            Debug.LogWarning($"⚠️ 26차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
