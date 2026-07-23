# **XR Project Settings**
## **요구 사항**
1. Unity 버전 : **2022.3 LTS 이상** 권장
2. 플랫폼 : Android (Meta Quest는 Android 기반입니다)
3. XR Plugin : **OpenXR** (Meta 공식 지원)
4. Meta SDK : Meta XR SDK (via Unity Package Manager)
5. 개발자 계정 : Meta Quest 개발자 계정 및 개발자 모드 설정

## **Package**
- XR Plug-in Management
- XR Interaction Toolkit
    - Input System
- XR Device Simulator
  - `Window > Package Manager > XR Interaction Toolkit`
  - `Samples에서 XR Device Simulator 설치`

### **XR Plug-in Management**
XR Plug-in Management는 Unity에서 **플랫폼별 XR SDK(예: Meta Quest, Windows Mixed Reality, OpenXR 등)** 를 
설치하고, 프로젝트 설정과 플랫폼 설정을 중앙에서 관리할 수 있도록 지원하는 패키지 기반 시스템.

- 플랫폼별 XR 런타임 관리 (예: Android용 Meta Quest, Windows용 OpenXR 등)
- XR SDK의 자동 설치 및 설정
- Unity 프로젝트에서 XR 기능 활성화 
- 플랫폼별 XR 초기화/종료 루틴 제어