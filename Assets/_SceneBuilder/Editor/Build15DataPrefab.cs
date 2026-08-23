using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 15차시 — 틀(Prefab)로 만들고 다섯 대 놓기.
    ///
    /// 교안 `15_data_prefab_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ 자동차를 틀로 만들고 · 다섯 대 놓고 · <b>틀 하나만 고쳐 전부 바꾸기</b></item>
    /// </list>
    ///
    /// <b>실습 ①(재료함 정리) · ②(그림 파일 넣기)는 씬에 안 남습니다.</b>
    /// 폴더를 만들고 파일을 가져오는 것이라 <b>재료함에서 일어나는 일</b>입니다.
    ///
    /// <b>실습 ④(한 대만 다르게 · 틀에서 떼어내기)도 만들지 않습니다.</b>
    /// 해보고 되돌리는 실습이라, 정답 씬에 <b>한 대만 크고 틀에서 떨어진 자동차</b>가 남으면
    /// *"왜 얘만 다르지?"* 로 읽힙니다.
    /// </summary>
    public static class Build15DataPrefab
    {
        const string FromScene = "14_완성";
        const string StartScene = "15_시작";
        const string CompleteScene = "15_완성";

        [MenuItem("Tools/교안 씬 빌더/15차시 — 틀로 만들고 다섯 대 놓기", false, 15)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "15차시 씬 만들기",
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

            GameObject car = FindCar();
            CalmDown(car);

            GameObject prefab = BuildKit.SaveAsPrefab(car, "Car", connect: true);
            PlaceFourMore(prefab);
            PaintTheMold();

            // 화면 요소를 안 만드는 차시입니다. 14차시 버튼은 켜둡니다 —
            // 자동차를 멈춰두고 다섯 대를 놓는 차시라 그 버튼이 필요합니다.
            BuildBasicsLayout.ApplyUiGroups(BuildKitUi.Root(), 15);

            BuildBasicsLayout.CameraFor(15, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        static GameObject FindCar()
        {
            GameObject car = GameObject.Find("Car");
            if (car == null)
            {
                throw new System.InvalidOperationException(
                    "14차시에서 만든 자동차(Car)가 없습니다. 14차시부터 다시 만들어주세요.");
            }
            return car;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 1단계 — 틀 만들기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 자동차를 멈춰둡니다. 교안 실습 ③ 2번 — *"돌면 확인하기 어렵습니다"*.
        ///
        /// <b>부품 자체의 체크가 아니라 <c>Is Spinning</c> 을 끕니다.</b>
        /// 부품을 꺼버리면 14차시에 만든 <b>"다시 돌리기" 버튼이 죽습니다.</b>
        /// (꺼진 부품은 <c>StartSpin</c> 을 불러도 돌지 않습니다)
        ///
        /// 21차시 교안도 같은 상황에서 *"`Is Spinning` 체크를 해제하고"* 라고 합니다.
        /// 이쪽이 이 강의의 방식입니다.
        /// </summary>
        static void CalmDown(GameObject car)
        {
            var spinner = car.GetComponent<CubeSpinner>();
            if (spinner == null) return;

            spinner.isSpinning = false;
            EditorUtility.SetDirty(spinner);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 2단계 — 다섯 대 놓기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 원래 있던 한 대는 <c>X 0</c> 에 그대로 두고, <b>네 대를 더</b> 찍어냅니다.
        /// 자리는 교안 실습 ③ 7번의 <c>0 · 3 · 6 · 9 · 12</c> 입니다.
        /// </summary>
        static void PlaceFourMore(GameObject prefab)
        {
            float[] xs = BuildBasicsLayout.CarLineX;
            Vector3 origin = BuildBasicsLayout.CarOrigin;

            for (int i = 1; i < xs.Length; i++)
            {
                BuildKit.PlaceFromPrefab(
                    prefab, new Vector3(xs[i], origin.y, origin.z), $"Car ({i})");
            }
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 3단계 — 틀 하나만 고치기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// <b>틀 안의 <c>Body</c> 에만</b> 색을 입힙니다. 다섯 대가 한꺼번에 바뀝니다.
        ///
        /// 13차시에 만든 <c>RedMat</c> 을 그대로 씁니다.
        /// 교안 실습 ③ 9번 — *"13차시에 만든 색깔 재료를 끌어다 놓습니다"*.
        ///
        /// <b>이게 이 차시의 전부입니다.</b> 한 대만 고쳤는데 다섯 대가 바뀌는 것.
        /// 그래서 14차시 빌더가 몸체를 안 칠하고 남겨뒀습니다.
        /// </summary>
        static void PaintTheMold()
        {
            Material red = BuildKit.Mat("RedMat", new Color(0.85f, 0.2f, 0.2f));

            BuildKit.EditPrefab("Car", root =>
            {
                Transform body = root.transform.Find("Body");
                if (body == null)
                {
                    throw new System.InvalidOperationException(
                        "틀 안에 Body 가 없습니다. 14차시부터 다시 만들어주세요.");
                }
                body.gameObject.Paint(red);
            });
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            // 자동차에만 CubeSpinner 가 붙어 있으니 이걸로 셉니다.
            // 이름으로 세면 수강생이 «내 자동차» 라고 지었을 때 못 셉니다.
            int cars = Object.FindObjectsByType<CubeSpinner>(
                FindObjectsInactive.Include, FindObjectsSortMode.None).Length;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 15차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"자동차가 {cars}대입니다. (5대면 정상)\n" +
                    "다섯 대 몸체가 전부 빨간지 보세요. 틀 하나만 고친 결과입니다.\n" +
                    "Hierarchy 에서 이름이 파란색이면 틀에 연결된 것입니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 15차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
