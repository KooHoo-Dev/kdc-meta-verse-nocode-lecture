using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 33차시 — 사격장이 여러 개인 게임월드. <b>전 과정의 마지막 차시입니다.</b>
    ///
    /// 교안 `33_vr_clay_world_nocode.md` 의 실습 ① ~ ④ 를 그대로 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 사격장을 하나로 묶기 — <b>사대 바닥 + 발사기만</b></item>
    /// <item>실습 ② 틀로 만들고 세 개 놓기 (핵심)</item>
    /// <item>실습 ③ <b>셋 다 일하게</b> — 시작 목록에 두 줄 더하기</item>
    /// <item>실습 ④ 사격장마다 난이도 다르게</item>
    /// </list>
    ///
    /// <b>새로 만드는 부품이 없습니다.</b> 묶고 · 틀로 만들고 · 연결을 더하는 것이 전부입니다.
    /// </summary>
    public static class Build33VrClayWorld
    {
        const string FromScene = "32_완성";
        const string StartScene = "33_시작";
        const string CompleteScene = "33_완성";

        const string RangeName = "ShootingRange";

        [MenuItem("Tools/교안 씬 빌더/33차시 — 사격장이 여러 개인 게임월드", false, 33)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "33차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「VR 사격 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFrom(FromScene, StartScene);

            GameObject range = GroupTheRange();
            GameObject mold = BuildKit.SaveAsPrefab(range, RangeName, connect: true);

            List<TargetLauncher> all = PlaceTwoMore(mold, range);
            WireThemAll(all);
            MakeEachDifferent(all);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 무엇을 묶을까
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 사격장이 될 것들을 하나로 묶습니다. <b>«세 개 있어도 말이 되나?» 로 가릅니다.</b>
        ///
        /// | 넣는 것 | 왜 |
        /// | 사대 바닥 · 발사기 | 사격장마다 하나씩 있어야 <b>맞습니다</b> |
        ///
        /// | 빼는 것 | 왜 |
        /// | <c>GameManager</c> | 🚨 <b>심판 · 시간 · 난이도는 하나만</b> (25차시) |
        /// | <c>StartZone</c> · 3D 버튼 · 점수판 | 시작과 표시는 하나만 |
        /// | <c>Gun</c> · <c>GunStand</c> | 총이 세 자루 필요하진 않습니다 |
        /// | <c>Ground</c> · <c>Fence</c> | 🚨 <b>울타리가 여러 개면 옆 사격장으로 못 갑니다</b> |
        /// | <c>XR Origin</c> · Simulator | 사람은 하나입니다 |
        /// </summary>
        static GameObject GroupTheRange()
        {
            GameObject launcher = GameObject.Find("Launcher");
            if (launcher == null)
            {
                throw new System.InvalidOperationException(
                    "23차시에서 만든 발사기(Launcher)가 없습니다. 22 ~ 27차시부터 다시 만들어주세요.");
            }

            // 사대 바닥 — 사격장이 여럿이면 «어디가 어딘지» 보여야 합니다.
            Material mat = BuildKit.Mat("RangeFloor", BuildVrClayLayout.Levels[0].Floor);

            GameObject floor = BuildKit.Shape("RangeFloor", PrimitiveType.Cube)
                                       .At(BuildVrClayLayout.RangeFloor,
                                           scale: BuildVrClayLayout.RangeFloorScale)
                                       .Paint(mat);

            GameObject range = BuildKit.Empty(RangeName).At(Vector3.zero);

            // 자리를 그대로 지키며 들어가야 합니다. 발사기는 이미 Z 15 에 있어요.
            floor.transform.SetParent(range.transform, true);
            launcher.transform.SetParent(range.transform, true);

            EditorUtility.SetDirty(range);
            return range;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 틀로 만들고 세 개 놓기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 틀에서 <b>두 개를 더</b> 찍어냅니다. 원본은 <c>X 0</c> 에 그대로 둡니다.
        /// 15차시에 자동차 다섯 대를 놓은 것과 완전히 같은 방식입니다.
        /// </summary>
        static List<TargetLauncher> PlaceTwoMore(GameObject mold, GameObject first)
        {
            var found = new List<TargetLauncher> { first.GetComponentInChildren<TargetLauncher>() };
            float[] xs = BuildVrClayLayout.RangeX;

            for (int i = 1; i < xs.Length; i++)
            {
                GameObject copy = BuildKit.PlaceFromPrefab(
                    mold, new Vector3(xs[i], 0f, 0f), $"{RangeName} ({i})");

                found.Add(copy.GetComponentInChildren<TargetLauncher>());
            }

            return found;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 셋 다 일하게
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// <b>새로 놓은 발사기는 «시작할 때» 목록에 없습니다.</b>
        ///
        /// `GameStarter` 는 자기 목록에 걸린 것만 실행합니다.
        /// 2 · 3번은 나중에 놓은 것이라 <b>있는 줄도 모릅니다.</b>
        /// 그래서 목록에 <b>두 줄을 더합니다.</b>
        ///
        /// 18차시의 «칸을 여러 개 만들면 한꺼번에 실행된다» 가 여기서 마지막으로 쓰입니다.
        /// 18차시에 둘, 26차시에 여섯, 오늘 셋 — <b>방식은 하나도 안 바뀌었습니다.</b>
        /// </summary>
        static void WireThemAll(List<TargetLauncher> all)
        {
            var starter = Object.FindFirstObjectByType<GameStarter>();
            if (starter == null)
            {
                throw new System.InvalidOperationException(
                    "23차시에서 만든 GameStarter 가 없습니다.");
            }

            // 1번은 23차시에 이미 걸려 있습니다. 2번부터 더합니다.
            for (int i = 1; i < all.Count; i++)
            {
                if (all[i] == null) continue;

                BuildKit.Wire(starter.onGameStart, all[i].StartLaunching);
                BuildKit.Wire(starter.onGameEnd, all[i].StopLaunching);
            }

            BuildKit.RecordPrefabChange(starter);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 사격장마다 다르게
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 곳마다 난이도를 다르게 박아둡니다. <b>15차시 실습 ④의 «하나만 따로 다르게»</b> 입니다.
        ///
        /// <c>DifficultyPreset</c> 은 발사기를 <b>하나만</b> 가리켜서
        /// 난이도 버튼은 1번 사격장만 바꿉니다. 그게 노코드의 경계예요.
        /// 대신 <b>값을 박아두면</b> 성격이 다른 사격장 셋이 됩니다.
        ///
        /// 사대 바닥 색도 같이 바꿉니다. <b>VR 에서는 글자보다 색이 잘 읽힙니다.</b>
        /// 단, <b>틀이 아니라 «놓아둔 것»</b> 을 고쳐야 셋이 서로 달라집니다.
        /// </summary>
        static void MakeEachDifferent(List<TargetLauncher> all)
        {
            BuildVrClayLayout.Level[] levels = BuildVrClayLayout.Levels;

            for (int i = 0; i < all.Count && i < levels.Length; i++)
            {
                TargetLauncher launcher = all[i];
                if (launcher == null) continue;

                BuildVrClayLayout.Level level = levels[i];

                launcher.interval = level.Interval;
                launcher.launchForce = level.Force;
                launcher.spreadAngle = level.Spread;
                BuildKit.RecordPrefabChange(launcher);

                PaintFloor(launcher, level);
            }
        }

        static void PaintFloor(TargetLauncher launcher, BuildVrClayLayout.Level level)
        {
            Transform range = launcher.transform.parent;
            if (range == null) return;

            Transform floor = range.Find("RangeFloor");
            if (floor == null) return;

            Material mat = BuildKit.Mat($"RangeFloor_{level.Label}", level.Floor);
            floor.gameObject.Paint(mat);

            var renderer = floor.GetComponent<Renderer>();
            if (renderer != null) BuildKit.RecordPrefabChange(renderer);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var launchers = Object.FindObjectsByType<TargetLauncher>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);

            var starter = Object.FindFirstObjectByType<GameStarter>();
            int onStart = starter != null ? starter.onGameStart.GetPersistentEventCount() : -1;
            int onEnd = starter != null ? starter.onGameEnd.GetPersistentEventCount() : -1;

            int scorers = Object.FindObjectsByType<HitScorer>(
                FindObjectsInactive.Include, FindObjectsSortMode.None).Length;

            bool mold = BuildKit.LoadPrefab(RangeName) != null;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 33차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다. **전 과정(11 ~ 33차시)이 여기서 끝납니다.**\n" +
                    $"사격장(발사기): {launchers.Length}곳 (3곳이면 정상)" +
                    $" · 틀: {(mold ? "만들어짐" : "❌ 없음")}\n" +
                    $"On Game Start: {onStart}개 · On Game End: {onEnd}개 (각 6개 · 4개면 정상)\n" +
                    "  — 32차시가 마우스 시야 · 캡슐 걷기 두 줄을 치웠기 때문입니다\n" +
                    $"점수 담당(HitScorer): {scorers}개 — **1개라야 합니다.** 심판은 하나\n" +
                    "\n▶ 를 누른 뒤 이 순서대로 —\n" +
                    "  ① 미리보기 창 클릭  ② `]` 로 오른손\n" +
                    "  ③ **초록 시작 버튼**을 겨누고 `G`   ← 손을 대기만 해서는 안 됩니다\n" +
                    "  ④ 총에 `G` 를 «누른 채로»  ⑤ 그 상태로 `T`\n" +
                    "  ⑥ 세 사격장을 돌아보기 (걷거나 `I` 로 순간이동)\n\n" +
                    "어디서 맞혀도 점수판 숫자가 올라야 합니다. 심판이 하나라서요.");
                return;
            }

            Debug.LogWarning($"⚠️ 33차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
