using System.Collections.Generic;
using NoCodeKit.EditorTools;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 29차시 — 눈높이 맞추기.
    ///
    /// <b>이 차시는 만드는 게 거의 없습니다.</b> 교안이 그렇게 말합니다 —
    /// *"오늘은 만드는 날이 아니라 맞추는 날입니다"*.
    ///
    /// 걷기 · 돌기는 <b>28차시에 끌어다 놓은 틀에 이미 다 들어 있었습니다.</b>
    /// 오늘 실습은 그걸 <b>찾아보고 · 껐다 켜보고 · 눈높이를 맞추는</b> 것입니다.
    ///
    /// <list type="bullet">
    /// <item>실습 ① 조작 익히기 — <b>탐색</b>. 만드는 것 없음</item>
    /// <item>실습 ② <b>눈높이 맞추기</b> — 씬에 남는 유일한 변화</item>
    /// <item>실습 ③ <c>Locomotion</c> 안을 열어보기 — <b>탐색</b></item>
    /// <item>실습 ④ 돌아보는 방식 바꿔보기 — <b>탐색</b></item>
    /// </list>
    ///
    /// 그래서 <c>29_시작</c> 과 <c>29_완성</c> 의 차이는 <b>눈높이 값 하나</b>입니다.
    /// 작지만 <b>실제 차이</b>이고, 교안 그대로입니다.
    /// </summary>
    public static class Build29XrOriginLocomotion
    {
        const string FromScene = "28_완성";
        const string StartScene = "29_시작";
        const string CompleteScene = "29_완성";

        [MenuItem("Tools/교안 씬 빌더/29차시 — 눈높이 맞추기", false, 29)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "29차시 씬 만들기",
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

            SetEyeHeight();

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 눈높이 맞추기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 눈높이를 <b>보통 어른 키</b>로 맞춥니다. 교안 실습 ② 5번의 <c>1.6</c> 입니다.
        ///
        /// <c>Tracking Origin Mode</c> 는 <b><c>Floor</c>(바닥 기준)</b> 여야 합니다.
        /// 교안 3번이 *"되어 있는지 확인합니다"* 라고 하는 그 값이에요.
        /// 바닥 기준이라야 <b>«바닥에서 몇 미터»</b> 가 말이 됩니다.
        /// </summary>
        static void SetEyeHeight()
        {
            var origin = Object.FindFirstObjectByType<XROrigin>();
            if (origin == null)
            {
                throw new System.InvalidOperationException(
                    "28차시에서 놓은 XR Origin 이 없습니다. 28차시부터 다시 만들어주세요.");
            }

            // 🚨 여기는 «속성에 대입» 으로는 안 됩니다.
            //
            //    XR Origin 은 XRI 샘플 «틀에서 찍어낸 것» 입니다.
            //    origin.CameraYOffset = 1.6f 처럼 C# 속성에 넣으면
            //    저장하고 열었을 때 틀의 기본값(1.36144)으로 되돌아갑니다. 두 번 확인했습니다.
            //
            //    SerializedObject 로 쓰면 «틀과 다른 부분» 목록에 제대로 올라갑니다.
            //    인스펙터에서 손으로 고치는 것과 같은 경로예요.
            var so = new SerializedObject(origin);

            SerializedProperty offset = so.FindProperty("m_CameraYOffset");
            SerializedProperty mode = so.FindProperty("m_RequestedTrackingOriginMode");

            if (offset != null) offset.floatValue = BuildXrLayout.EyeHeight;
            if (mode != null) mode.enumValueIndex = (int)XROrigin.TrackingOriginMode.Floor;

            so.ApplyModifiedProperties();

            // 교안 실습 ② 4 · 5번은 «Camera Offset 을 고르고 위치 Y 를 바꾸라» 고 합니다.
            // 수강생이 보는 곳이 거기라, 그 값도 직접 맞춰둡니다.
            MoveCameraOffset(origin);
        }

        /// <summary>
        /// <c>Camera Offset</c> 오브젝트의 <b>위치 Y</b> 를 눈높이로 맞춥니다.
        ///
        /// 교안이 수강생에게 보여주는 칸이 <b>바로 여기</b>입니다 —
        /// *"Hierarchy에서 `Camera Offset` 을 고릅니다 → 위치 부품의 Y 값을 바꿔봅니다"*.
        /// 부품 쪽 값만 맞춰두면 <b>수강생이 보는 숫자는 그대로</b>라 어긋나 보입니다.
        /// </summary>
        static void MoveCameraOffset(XROrigin origin)
        {
            GameObject holder = origin.CameraFloorOffsetObject;
            if (holder == null) return;

            var so = new SerializedObject(holder.transform);
            SerializedProperty pos = so.FindProperty("m_LocalPosition");

            if (pos == null) return;

            pos.vector3Value = new Vector3(
                pos.vector3Value.x, BuildXrLayout.EyeHeight, pos.vector3Value.z);

            so.ApplyModifiedProperties();
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var origin = Object.FindFirstObjectByType<XROrigin>();
            float height = origin != null ? origin.CameraYOffset : -1f;
            bool floor = origin != null
                         && origin.RequestedTrackingOriginMode == XROrigin.TrackingOriginMode.Floor;

            // 수강생이 실제로 보는 칸 — Camera Offset 의 위치 Y
            float shown = origin != null && origin.CameraFloorOffsetObject != null
                ? origin.CameraFloorOffsetObject.transform.localPosition.y
                : -1f;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 29차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"눈높이 — XR Origin 의 Camera Y Offset: {height:0.00}\n" +
                    $"          Camera Offset 의 위치 Y: {shown:0.00}  ← 교안이 보여주는 칸\n" +
                    $"          둘 다 1.60 이면 정상입니다.\n" +
                    $"기준(Tracking Origin Mode): {(floor ? "Floor — 바닥 기준" : "❌ Floor 가 아님")}\n" +
                    "▶ 를 누르고 Game 창 클릭 → W A S D 로 걸어보세요.\n" +
                    "기둥이 지나가면 걷고 있는 것입니다. 걷기 부품은 28차시 틀에 이미 들어 있습니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 29차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
