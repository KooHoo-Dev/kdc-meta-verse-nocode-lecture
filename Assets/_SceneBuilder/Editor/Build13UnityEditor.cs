using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 13차시 — Unity Editor 둘러보기.
    ///
    /// 교안 `13_unity_editor_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ 색깔 재료를 만들어 큐브에 입히기</item>
    /// <item>실습 ④ <c>ConsoleTester</c> 로 메시지 찍어보기</item>
    /// </list>
    ///
    /// <b>실습 ① · ②는 만들 것이 없습니다.</b>
    /// ①은 창을 어질러보고 되돌리는 것, ②는 Scene 창에서 시점을 움직여보는 것이라
    /// <b>결과가 씬에 남지 않습니다.</b> 갈래 A 앞쪽에는 이런 실습이 많습니다.
    ///
    /// 갈래 A의 출발점이라 <b>빈 씬에서 시작</b>합니다.
    /// </summary>
    public static class Build13UnityEditor
    {
        const string StartScene = "13_시작";
        const string CompleteScene = "13_완성";

        [MenuItem("Tools/교안 씬 빌더/13차시 — Unity Editor 둘러보기", false, 13)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "13차시 씬 만들기",
                    $"지금 열려 있는 씬을 닫고 {StartScene} · {CompleteScene} 을 새로 만듭니다.\n" +
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
            // 갈래 A의 첫 차시입니다. 앞 차시가 없으니 빈 씬에서 시작합니다.
            BuildKit.BeginFromEmpty(StartScene);

            GameObject cube = BuildCube();
            AttachConsoleTester(cube);

            BuildBasicsLayout.CameraFor(13, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 재료를 만들어서 입혀보기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 큐브 하나에 빨간 재료를 입힙니다.
        ///
        /// 교안은 재료 이름을 <c>RedMat</c> 이라고 정해뒀습니다.
        /// <c>BuildKit.Mat</c> 이 URP 셰이더로 만들어주므로 <b>분홍색이 되지 않습니다.</b>
        /// </summary>
        static GameObject BuildCube()
        {
            Material red = BuildKit.Mat("RedMat", new Color(0.85f, 0.2f, 0.2f));

            return BuildKit.Shape("Cube", PrimitiveType.Cube)
                           .At(BuildBasicsLayout.DemoCube)
                           .Paint(red);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 실행하고 Console 에서 메시지 읽기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// <c>ConsoleTester</c> 를 붙입니다.
        ///
        /// 교안은 <c>Log Type</c> 을 <c>Info ▸ Warning ▸ Error</c> 로 바꿔가며 확인시킵니다.
        /// <b>완성 씬은 하얀 줄(Info) 상태로 둡니다.</b>
        /// 노란 줄·빨간 줄은 수강생이 직접 바꿔보는 것이고,
        /// 정답 씬이 Console 에 경고를 남기면 <b>진짜 문제와 헷갈립니다.</b>
        /// </summary>
        static void AttachConsoleTester(GameObject cube)
        {
            var tester = BuildKit.Add<ConsoleTester>(cube);
            tester.message = BuildBasicsLayout.ConsoleMessage;
            tester.logType = ConsoleTester.MessageKind.Info;
            tester.sendOnStart = true;
            EditorUtility.SetDirty(tester);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 13차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    "▶ 를 누르면 Console 에 하얀 줄로 «안녕하세요» 가 떠야 합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 13차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
