# ARVR-Project-Templete

KDC 메타버스 노코드 강의 프로젝트입니다.

## 📖 강의 교안 사이트

**https://koohoo-dev.github.io/kdc-meta-verse-nocode-lecture/**

`Docs/1. NoCode/` 하위 문서가 [MkDocs Material](https://squidfunk.github.io/mkdocs-material/)로 빌드되어
GitHub Pages에 자동 배포됩니다.

### 로컬 미리보기

최초 1회 설치:

```bash
pip install -r requirements-docs.txt
```

**방법 1 — HTML 파일로 열기** (배포를 기다리기 싫을 때)

```bash
powershell -ExecutionPolicy Bypass -File preview-docs.ps1
```

`site-offline/` 에 실제 `.html` 파일로 빌드한 뒤 브라우저를 띄웁니다.
[mkdocs.offline.yml](mkdocs.offline.yml) 이 `use_directory_urls: false` 와 Material `offline` 플러그인을 써서
서버 없이 `file://` 로 열어도 링크·검색이 동작합니다.

**방법 2 — 개발 서버** (문서를 계속 고칠 때. 저장하면 자동 새로고침)

```bash
mkdocs serve
```

> `site_url`이 설정되어 있어 개발 서버 주소는 루트가 아닌
> `http://127.0.0.1:8000/kdc-meta-verse-nocode-lecture/` 입니다.

### 배포

`main` 브랜치에 아래 경로가 변경되면 [deploy-docs.yml](.github/workflows/deploy-docs.yml)이 자동 실행됩니다.

- `Docs/1. NoCode/**`
- `mkdocs.yml`
- `requirements-docs.txt`

수동 실행은 GitHub Actions 탭의 **Deploy Docs to GitHub Pages → Run workflow**.

> [!IMPORTANT]
> 최초 1회, 저장소 **Settings → Pages → Build and deployment → Source**를
> **GitHub Actions**로 바꿔주셔야 배포가 동작합니다.

### 목차 편집

폴더마다 `.pages` 파일로 제목과 순서를 정합니다.

```yaml
title: 🎮 Unity Basic
nav:
  - '01. Unity와 첫 만남': 01_unity_engine_nocode.md
```

### 사용 중인 마크다운 확장

| 문법 | 담당 확장 |
| :--- | :--- |
| `!!! note` 강조 박스 | `admonition` |
| `??? success` 접히는 블록 | `pymdownx.details` |
| ` ```mermaid ` 다이어그램 | `pymdownx.superfences` (Material 내장 렌더러) |
| `- [ ]` 체크리스트 | `pymdownx.tasklist` |
| `=== "탭"` 탭 블록 | `pymdownx.tabbed` |
| 표 · 각주 · 약어 | `tables` / `footnotes` / `abbr` |
| 이미지 클릭 확대 | `mkdocs-glightbox` |
| 페이지 최종 수정일 | `git-revision-date-localized` |
