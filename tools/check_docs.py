# -*- coding: utf-8 -*-
"""교안 자가 검증 — `mkdocs build --strict` 를 대신합니다.

사이트 빌드를 없앤 뒤로, 문서가 GitHub 에서 제대로 보이는지 확인할 방법이
필요해서 만든 것입니다.

사용법:
    python tools/check_docs.py                 # Docs/1. NoCode 전체
    python tools/check_docs.py path/to/one.md  # 파일 지정

경고가 0건이면 통과입니다.
"""
import re
import sys
import pathlib
from urllib.parse import unquote

ROOT = pathlib.Path(__file__).resolve().parent.parent
DOCS = ROOT / "Docs" / "1. NoCode"

EMOJI = "🎯✅⚠️🚨💡ℹ️📝🧩💬"

# 사이트 전용 문법 — GitHub 에서는 글자 그대로 노출됩니다
SITE_ONLY = [
    (re.compile(r'^\s*!!!\s+[a-z]+'),        "admonition `!!!` — `> 이모지 **제목**` 으로"),
    (re.compile(r'^\s*\?\?\?\+?\s+[a-z]+'),  "접기 `???` — `<details>` 로"),
    (re.compile(r'^\s*===\s+"'),             "탭 `=== \"...\"`"),
    (re.compile(r':(?:material|octicons|fontawesome)-'), "Material 아이콘"),
    (re.compile(r'<div class='),             "사이트 전용 `<div class=...>`"),
    (re.compile(r'^\s*\{\s*\.[a-z]'),        "attr_list `{ .class }`"),
]

# 치환표·용어 부록 안에서는 금지 용어가 나와도 정상입니다
GLOSSARY_HEAD = re.compile(r'^#{1,3}\s+\**(비프로그래머 대상 표현 치환표|부록)')

BANNED = re.compile(
    r'변수|필드|메서드|매개변수|상속|컴파일|프레임|클래스|인스턴스|렌더링'
)


class Report:
    def __init__(self):
        self.errors = []
        self.notes = []

    def err(self, path, line, msg):
        self.errors.append(f"{path}:{line}  {msg}")

    def note(self, path, line, msg):
        self.notes.append(f"{path}:{line}  {msg}")


def check_file(p: pathlib.Path, rep: Report):
    try:
        rel = p.resolve().relative_to(ROOT).as_posix()
    except ValueError:
        rel = p.as_posix()
    lines = p.read_text(encoding="utf-8").split("\n")

    # ── 1. 사이트 전용 문법 잔재 ────────────────────────────────
    # 인라인 코드(`...`) 안의 언급은 "쓰지 말 것"을 설명하는 글이므로 제외합니다
    bare = []
    in_fence = False
    glossary_from = len(lines)
    for i, l in enumerate(lines, 1):
        if l.lstrip().startswith("```"):
            in_fence = not in_fence
        if GLOSSARY_HEAD.match(l):
            glossary_from = min(glossary_from, i)
        bare.append("" if in_fence else re.sub(r'`[^`]*`', '', l))
        if in_fence:
            continue
        for pat, msg in SITE_ONLY:
            if pat.search(bare[-1]):
                rep.err(rel, i, f"사이트 전용 문법 — {msg}")

    # ── 2. <details> 짝과 빈 줄 ─────────────────────────────────
    opens = [i for i, l in enumerate(bare, 1) if "<details" in l]
    closes = [i for i, l in enumerate(bare, 1) if "</details>" in l]
    summaries = [i for i, l in enumerate(bare, 1) if "</summary>" in l]

    if not (len(opens) == len(closes) == len(summaries)):
        rep.err(rel, 0,
                f"접기 짝이 안 맞음 — <details> {len(opens)} / "
                f"</details> {len(closes)} / <summary> {len(summaries)}")

    for i in summaries:
        if i < len(lines) and lines[i].strip() != "":
            rep.err(rel, i, "</summary> 다음에 빈 줄이 없음 — 안쪽이 렌더링되지 않습니다")

    for i in closes:
        if i >= 2 and lines[i - 2].strip() != "":
            rep.err(rel, i, "</details> 앞에 빈 줄이 없음")

    # ── 3. 인용 강조 블록의 제목 줄 뒤 빈 인용 줄 ────────────────
    head = re.compile(r'^(\s*)>\s+[' + EMOJI + r']+\s+\*\*.+\*\*\s*$')
    for i, l in enumerate(lines, 1):
        m = head.match(l)
        if not m:
            continue
        nxt = lines[i] if i < len(lines) else ""
        if nxt.strip() == "":
            continue                            # 본문 없는 한 줄 블록 — 정상
        if nxt.strip() != ">" and not nxt.strip().startswith("> "):
            rep.err(rel, i, "강조 블록 제목 다음 줄이 `>` 로 이어지지 않음")
        elif nxt.strip() != ">":
            rep.err(rel, i, "강조 블록 제목과 본문 사이에 `>` 빈 줄이 없음")

    # 교안에만 적용하는 규칙과, 모든 문서에 적용하는 규칙을 나눕니다.
    # plans/ 는 설계 문서라 C# 조각이나 용어가 나오는 게 정상입니다.
    is_lecture = rel.startswith("Docs/")

    # ── 4. C# 코드블록 (교안에만) ───────────────────────────────
    if is_lecture:
        for i, l in enumerate(lines, 1):
            if l.strip().startswith("```csharp") or l.strip().startswith("```cs"):
                rep.err(rel, i, "C# 코드블록 — 노코드 교안에는 넣지 않습니다")

    # ── 5. mermaid 펜스 짝 ─────────────────────────────────────
    if sum(1 for l in lines if l.strip().startswith("```")) % 2:
        rep.err(rel, 0, "코드 펜스(```) 개수가 홀수 — 어딘가 안 닫혔습니다")

    # ── 6. 내부 링크 대상이 실제로 있는지 ───────────────────────
    for i, l in enumerate(bare, 1):
        for target in re.findall(r'\]\(([^)#\s]+\.md)(?:#[^)]*)?\)', l):
            if target.startswith(("http://", "https://")):
                continue
            if not (p.parent / unquote(target)).exists():
                rep.err(rel, i, f"링크 대상 없음 — {target}")
        # 경로에 공백이 있으면 GitHub 이 링크로 인식하지 못합니다
        for bad in re.findall(r'\]\(([^)]*\s[^)]*\.md[^)]*)\)', l):
            if not bad.startswith(("http://", "https://")):
                rep.err(rel, i, f"링크 경로에 공백 — `%20` 으로 바꾸세요: {bad}")

    # ── 7. 정리 문제 수 == 정답 보기 수 ────────────────────────
    if p.name.endswith("_nocode.md"):
        problems = sum(1 for l in lines if re.match(r'^#{3,4}\s+\**문제\s*\d', l))
        answers = sum(1 for l in lines
                      if l.strip().startswith("<summary>") and "정답 보기" in l)
        if problems and problems != answers:
            rep.err(rel, 0, f"정리 문제 {problems}개 / 정답 보기 {answers}개 — 안 맞음")

    # ── 8. 금지 용어 (교안의 치환표·부록 밖에서만) ──────────────
    if not is_lecture:
        return

    in_fence = False
    for i, l in enumerate(lines, 1):
        if l.lstrip().startswith("```"):
            in_fence = not in_fence
        if in_fence or i >= glossary_from:
            continue
        m = BANNED.search(l)
        if m:
            rep.note(rel, i, f"금지 용어 '{m.group()}' — {l.strip()[:60]}")


def main(argv):
    targets = [pathlib.Path(a) for a in argv] if argv else sorted(DOCS.rglob("*.md"))
    rep = Report()
    for p in targets:
        check_file(p, rep)

    print(f"검사한 파일: {len(targets)}개\n")

    if rep.errors:
        print(f"[오류] {len(rep.errors)}건")
        for e in rep.errors:
            print("  ", e)
    else:
        print("[오류] 없음 ✅")

    if rep.notes:
        print(f"\n[확인 필요] 금지 용어 {len(rep.notes)}건 — 본문에서 쓰였는지 눈으로 봐주세요")
        for n in rep.notes:
            print("  ", n)

    return 1 if rep.errors else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
