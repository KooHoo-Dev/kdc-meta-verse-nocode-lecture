using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NoCodeKit.EditorTools
{
    /// <summary>
    /// 실습 점검기. 지금 만들고 있는 것에서 <b>손볼 곳을 찾아 메모장(Console)에 적어줍니다.</b>
    ///
    /// 쓰는 방법은 두 가지입니다.
    /// <list type="bullet">
    /// <item><c>Tools ▸ 노코드 점검 ▸ 지금 점검하기</c> 를 누른다</item>
    /// <item>▶ 를 누를 때 알아서 한 번 봐준다 (끌 수 있습니다)</item>
    /// </list>
    ///
    /// 메모장에 나온 줄을 <b>클릭하면 그 자리가 잡힙니다.</b>
    /// </summary>
    [InitializeOnLoad]
    public static class NoCodeCheck
    {
        const string Root = "Tools/노코드 점검/";
        const string AutoMenu = Root + "▶ 누를 때 자동으로 봐주기";
        const string AutoKey = "NoCodeKit.AutoCheckOnPlay";

        static NoCodeCheck()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        // ── 메뉴 ──────────────────────────────────────────────

        [MenuItem(Root + "지금 점검하기 %#k", false, 1)]
        public static void CheckNow()
        {
            Report(NoCodeCheckRules.CheckAll(), manual: true);
        }

        [MenuItem(AutoMenu, false, 20)]
        static void ToggleAuto()
        {
            AutoCheck = !AutoCheck;
        }

        [MenuItem(AutoMenu, true, 20)]
        static bool ToggleAutoValidate()
        {
            Menu.SetChecked(AutoMenu, AutoCheck);
            return true;
        }

        static bool AutoCheck
        {
            get => EditorPrefs.GetBool(AutoKey, true);
            set => EditorPrefs.SetBool(AutoKey, value);
        }

        // ── ▶ 를 누를 때 ──────────────────────────────────────

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            // 미리보기로 들어가기 직전입니다. 여기서 잡아주면 헤매지 않습니다.
            if (state != PlayModeStateChange.ExitingEditMode) return;
            if (!AutoCheck) return;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();
            if (found.Count > 0) Report(found, manual: false);
        }

        // ── 결과 ──────────────────────────────────────────────

        static void Report(List<NoCodeIssue> found, bool manual)
        {
            if (found.Count == 0)
            {
                if (manual) Debug.Log("✅ 노코드 점검 — 손볼 곳이 없습니다. 잘하셨어요!");
                return;
            }

            Debug.LogWarning(
                $"⚠️ 노코드 점검 — 손볼 곳이 {found.Count}군데 있습니다.\n" +
                "아래 줄을 하나씩 눌러보세요. 누르면 그 자리가 잡힙니다.");

            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
