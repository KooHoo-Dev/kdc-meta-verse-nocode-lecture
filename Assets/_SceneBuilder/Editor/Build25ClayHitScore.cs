using System.Collections.Generic;
using NoCodeKit.EditorTools;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 25차시 — 맞으면 터지고 점수가 오르게. <b>여기서 게임이 성립합니다.</b>
    ///
    /// 교안 `25_clay_hit_score_nocode.md` 의 실습 ① · ③ 을 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 표적 틀에 `Hittable` — 맞으면 사라지게</item>
    /// <item>실습 ③ `HitScorer` + `ValueDisplay` — 점수가 오르게 (<b>Dynamic float</b>)</item>
    /// </list>
    ///
    /// 실습 ②(터지는 효과)는 <b>만들지 않습니다.</b> 기본 도형 효과가 보기에 좋지 않아,
    /// 실제 효과 리소스가 들어오면 그때 `FeedbackSpawner` 와 함께 붙입니다.
    ///
    /// 실습 ④(놓친 것 세어보기)는 <b>막히는 것을 겪는 실습</b>이라 만들 것이 없습니다.
    /// </summary>
    public static class Build25ClayHitScore
    {
        const string FromScene = "24_완성";
        const string StartScene = "25_시작";
        const string CompleteScene = "25_완성";

        [MenuItem("Tools/교안 씬 빌더/25차시 — 맞으면 터지고 점수가 오르게", false, 25)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "25차시 씬 만들기",
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

            MakeTargetHittable();
            BuildScore();

            // 실습 ②(터지는 효과)는 만들지 않습니다. §터지는 효과 참고.

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 맞으면 사라지게
        // ══════════════════════════════════════════════════════

        static void MakeTargetHittable()
        {
            // 23차시에 만든 접시 틀을 열어 고칩니다. 틀을 고치면 앞으로 나올 표적 전부에 적용됩니다.
            BuildKit.EditPrefab("Target", root =>
            {
                var hittable = BuildKit.Add<Hittable>(root);
                hittable.hitterTag = "Bullet";
                hittable.destroyOnHit = true;
                hittable.addScore = true;
            });
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 점수가 오르게
        // ══════════════════════════════════════════════════════

        static void BuildScore()
        {
            // 23차시에 빈 그릇으로 만들어뒀습니다. 없으면 새로 만듭니다.
            GameObject manager = BuildKit.EnsureEmpty("GameManager");

            // 씬에 하나만 둡니다. 표적 틀이 씬의 점수판을 끌어다 놓을 수 없어서,
            // 표적 쪽이 이 담당을 알아서 찾아옵니다 (함정 ①).
            var scorer = BuildKit.Add<HitScorer>(manager);
            scorer.pointsPerHit = BuildConfig.PointsPerHit;
            EditorUtility.SetDirty(scorer);

            Canvas canvas = BuildKitUi.Root();

            TextMeshProUGUI scoreText = BuildKitUi.Text(
                "ScoreText", "점수: 0 점", canvas.transform,
                new Vector2(-40f, -40f), new Vector2(360f, 70f), 44f);
            scoreText.alignment = TextAlignmentOptions.Right;
            scoreText.Anchor(new Vector2(1f, 1f));   // 오른쪽 위

            var display = BuildKit.Add<ValueDisplay>(scoreText.gameObject);
            display.prefix = "점수: ";
            display.suffix = " 점";
            display.decimals = 0;
            EditorUtility.SetDirty(display);

            // 🔴 목록 위쪽 Dynamic float 입니다. 아래쪽 고정값을 고르면
            //    점수가 올라도 화면이 안 바뀝니다. 18 · 26차시에서도 반복되는 함정입니다.
            BuildKit.WireDynamic(scorer.onScoreChanged, display.SetValue);
        }

        // ══════════════════════════════════════════════════════
        //  터지는 효과 — 지금은 만들지 않습니다
        // ══════════════════════════════════════════════════════
        //
        // 교안 실습 ② 는 기본 도형 파티클로 터지는 효과를 만듭니다. 24차시의 총구 불꽃과
        // 같은 문제라, 실제 효과 리소스가 들어오면 그때 붙입니다.
        //
        // 지금 비워둬도 아무 문제 없습니다.
        //   · Hittable 은 On Hit 에 아무것도 없어도 표적을 없애고 점수를 올립니다
        //   · FeedbackSpawner 를 아예 안 붙였으므로 점검기가 빈 칸을 지적할 일도 없습니다
        //
        // 붙일 때는 표적 틀 안에 FeedbackSpawner 를 넣고
        // On Hit ▸ FeedbackSpawner.Spawn 으로 이으면 됩니다. (틀 안끼리라 연결됩니다)

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 25차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    "▶ 를 누르고 발판을 밟은 뒤 표적을 맞혀보세요. " +
                    "맞으면 표적이 사라지고 오른쪽 위 점수가 10씩 오릅니다.\n" +
                    "\n" +
                    "※ 터지는 효과(실습 ②)는 넣지 않았습니다. 실제 효과 리소스가 들어오면 붙입니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 25차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
