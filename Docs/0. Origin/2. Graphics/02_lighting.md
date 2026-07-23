# **Lighting**
# **광원의 기본 개념**
> 게임 엔진으로 이미지를 확인하는걸로
> 

### **광원?**
광원(Light Source)은 게임 엔진에서 오브젝트를 조명하여 현실적인 환경을 구현하는 요소이다. 광원은 장면의 분위기, 가시성, 심미성을 결정하는 중요한 요소이며, 성능에도 영향을 미친다.

### **광원의 주요 속성**
- **색상(Color):** 빛의 색상으로 조명의 분위기를 결정
- **강도(Intensity):** 빛의 밝기를 조절하는 요소
- **범위(Range):** 빛이 영향을 미치는 거리 (점광원과 같은 일부 광원에 적용)
- **감쇠(Attenuation):** 거리에 따른 빛의 감소율
- **각도(Spot Angle):** 스포트라이트에서 빛이 퍼지는 범위
- **섀도우(Shadows):** 광원이 만드는 그림자의 품질 및 설정

---

# **Unity에서 제공하는 기본 광원 종류**
![Lighting 그림 1](lighting/image.png)

## **Unity의 Light 컴포넌트 종류**
| Light 타입 | 설명 |
| --- | --- |
| **Point Light** | 한 지점에서 모든 방향으로 빛을 발산 |
| **Spot Light** | 원뿔 모양으로 특정 방향을 비춤 |
| **Directional Light** | 태양광처럼 일정한 방향으로 빛을 비춤 |
| **Area Light** | 사각형 형태에서 빛을 방출 (실시간 불가) |

---

## **Light 컴포넌트의 공통 프로퍼티 및 설정 가능 옵션**
## **Light 컴포넌트의 기본 속성**
| 프로퍼티 | 설명 |
| --- | --- |
| **Type** | 광원의 종류 (Directional, Point, Spot, Area) |
| **Color** | 광원의 색상 |
| **Intensity** | 광원의 강도 (기본값: 1) |
| **Mode** | 조명 방식 (Realtime, Mixed, Baked) |
| **Shadow Type** | 그림자 유형 (None, Hard Shadows, Soft Shadows) |
| **Cookie** | 조명 텍스처 마스킹 (스포트라이트 등에서 활용) |

---

## **각 광원 유형별 주요 프로퍼티**
### **Directional Light (평행광)**
- 장면 전체를 비추는 균일한 빛 (태양광과 유사)
- 거리에 관계없이 모든 오브젝트에 동일한 밝기로 적용됨

| 프로퍼티 | 설명 |
| --- | --- |
| **Color** | 태양광의 색을 설정 |
| **Intensity** | 빛의 강도를 설정 (기본: 1) |
| **Shadow Type** | 그림자 유형 (None, Hard, Soft) |
| **Shadow Strength** | 그림자의 강도를 조절 (0 ~ 1) |
| **Light Cookie** | 텍스처를 적용해 빛을 패턴화 (ex: 창살 그림자) |

**추가 옵션:**

- **Shadow Bias, Normal Bias**: 그림자의 품질을 조정하여 아티팩트 감소
- **Realtime, Mixed, Baked 지원**

---

### **Point Light (점광원)**
- 특정 지점에서 모든 방향으로 빛을 발산
- 거리에 따라 빛이 감쇠됨

| 프로퍼티 | 설명 |
| --- | --- |
| **Range** | 빛이 닿는 최대 거리 (기본: 10) |
| **Color** | 빛의 색상을 설정 |
| **Intensity** | 빛의 강도를 조정 |
| **Shadow Type** | 그림자 유형 설정 |
| **Light Cookie** | 텍스처를 사용해 특정 형태로 빛을 투사 |

**추가 옵션:**

- **Attenuation (감쇠 효과)** 적용 가능
- **Realtime, Mixed, Baked 지원**

---

### **Spot Light (스포트라이트)**
- 원뿔 형태로 특정 방향을 비추는 광원

| 프로퍼티 | 설명 |
| --- | --- |
| **Spot Angle** | 빛의 확산 각도 (1~179도) |
| **Range** | 빛이 영향을 미치는 거리 |
| **Color** | 빛의 색상을 설정 |
| **Intensity** | 빛의 강도를 조정 |
| **Shadow Type** | 그림자 유형 설정 |
| **Cookie** | 텍스처를 적용해 빛의 패턴을 변경 |

**추가 옵션:**

- **Inner Spot Angle** (HDRP에서 지원)
- **Realtime, Mixed, Baked 지원**

---

### **Area Light (면광원)**
- 사각형 형태에서 부드러운 빛을 발산
- **실시간 지원 안됨, 베이크된 조명(Baked)에서만 사용 가능**

| 프로퍼티 | 설명 |
| --- | --- |
| **Width / Height** | 광원의 크기를 설정 |
| **Color** | 빛의 색상을 설정 |
| **Intensity** | 빛의 강도를 조정 |

**추가 옵션:**

- **Realtime 지원 안됨, Baked 전용**

---

### **추가적인 고급 조명 옵션**
### **Light Probe (라이트 프로브)**
- **정적인 오브젝트가 아닌 동적 오브젝트에 간접광을 적용**하는 시스템
- **베이크된 조명과 함께 사용하여 최적화된 환경 제공**

| 프로퍼티 | 설명 |
| --- | --- |
| **Light Probe Group** | 씬에서 Light Probe를 그룹화 |
| **Interpolation** | 주변 라이트 프로브를 보간하여 부드럽게 표현 |

---

### **Reflection Probe (반사 프로브)**
- 반사 효과를 위한 **큐브맵을 생성**하여 빛을 반사시키는 역할

| 프로퍼티 | 설명 |
| --- | --- |
| **Type** | Baked, Realtime, Custom 설정 |
| **Refresh Mode** | 반사 맵 업데이트 빈도 설정 |
| **Box Projection** | 반사의 왜곡을 줄이는 옵션 |

---

## **광원 렌더링 기법과 성능 최적화**
### **실시간 광원 (Real-Time Lighting)**
- 즉각적으로 장면의 조명 변화를 반영
- 게임 내에서 동적인 환경을 만들 때 사용
- **단점:** 많은 실시간 광원이 존재하면 성능이 크게 저하됨

### **베이크드 라이팅 (Baked Lighting)**
- 광원 데이터를 미리 계산하여 텍스처에 저장 (Lightmap 활용)
- 실시간 계산이 필요 없어 성능이 뛰어남
- **단점:** 조명 변경이 불가능

### **혼합 광원 (Mixed Lighting)**
- 실시간 광원과 베이크드 광원을 혼합하여 사용
- **활용:** 실시간 움직이는 오브젝트는 실시간 광원을, 정적 환경은 베이크드 광원을 사용

### **광원 최적화 기법**
- 불필요한 실시간 광원 제한
- 그림자 품질 조정 (Soft Shadows vs. Hard Shadows)
- **LOD (Level of Detail) 조정**으로 멀리 있는 오브젝트에 대한 조명 연산 축소
- **Light Probe와 Reflection Probe 활용**하여 실시간 연산 감소

---

## **Unity에서 광원 효과 활용 예제**
### **태양광(Directional Light) 설정**
1. `GameObject > Light > Directional Light` 생성
2. 강도를 조절하고 그림자 설정을 `Soft Shadows`로 변경
3. `Color` 값을 조정하여 따뜻한 햇빛 또는 차가운 달빛을 구현

### **손전등(Spot Light) 구현**
1. 플레이어 오브젝트에 `Spot Light` 추가
2. `Spot Angle`과 `Intensity` 조정
3. `Light Component`의 `Culling Mask`를 활용하여 특정 오브젝트에만 빛이 닿도록 설정

### **동적 조명 효과 (Light Probe 활용)**
1. `GameObject > Light > Light Probe Group` 추가
2. 광원의 영향을 받지 않는 정적 오브젝트 주변에 배치
3. 실시간 조명 변경 없이 빛을 반영하는 최적화된 환경 구현

### **반사 효과 (Reflection Probe 활용)**
1. `GameObject > Light > Reflection Probe` 추가
2. 반사 맵을 생성하여 유리, 금속 표면에 반사 효과 적용
3. 실시간 업데이트 옵션을 조정하여 성능 최적화

!!! tip
    Light Probe와 Refelection Probe는 유니티 씬에서 확인

---

## **고급 광원 효과 (URP 및 HDRP 활용)**
### **Universal Render Pipeline (URP)에서의 조명**
- `Forward Renderer`를 사용하여 모바일 및 저사양 환경에 최적화
- `Additional Lights` 옵션으로 다중 광원 효과 가능
- **스크린 스페이스 셰도우(Screen Space Shadows)** 지원

### **High Definition Render Pipeline (HDRP)에서의 조명**
- *물리 기반 조명 시스템(PBR)**을 활용한 고품질 광원 효과
- `Volumetric Lighting`을 활용한 안개 및 광원 확산 효과
- `Ray Tracing`을 이용한 실시간 반사 및 그림자 적용

---