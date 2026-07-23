# **Material**
---

# **Material?**
Material(재질)은 3D 모델의 표면을 렌더링할 때 사용되는 설정들의 집합으로, 모델이 어떤 색을 띠고, 어떤 질감을 가지며, 빛과 어떻게 상호작용할지를 정의하는 요소이다. 대부분의 게임 엔진에서 Material은 Shader와 Texture를 조합하여 만들어진다.

![Material 그림 1](material/image.png)

## **PBR(Physically Based Rendering)?**
PBR(물리 기반 렌더링)은 실제 세계에서 빛이 물체와 상호작용하는 방식을 시뮬레이션하여 현실적인 렌더링을 구현하는 기법이다. PBR은 **에너지 보존 법칙**, **마이크로 서피스 모델링**, **선형 색공간** 등을 기반으로 한다.

![Material 그림 2](material/image%201.png)

### **1. PBR의 핵심 개념**
- **에너지 보존(Energy Conservation)**
    
    > 반사되는 빛의 양이 입사한 빛의 양을 초과하지 않도록 한다. 즉, 표면이 반사하는 빛과 흡수하는 빛이 총합하여 원래의 빛과 동일해야 한다.
    > 
- **마이크로 서피스(Micro Surface) 모델링**
    
    > 모든 표면은 미세한 요철을 가지고 있으며, 이 요철이 빛을 산란시켜 반사각을 결정한다.
    > 
- **선형 색공간(Linear Color Space)**
    
    > 정확한 조명 계산을 위해 감마 보정이 적용되지 않은 선형 색 공간에서 연산을 수행한다.
    > 

### **2. PBR의 주요 구성 요소**
PBR은 일반적으로 두 가지 모델을 기반으로 한다:

![Material 그림 3](material/image%202.png)

1. **Metallic-Roughness 모델** (유니티, 언리얼에서 주로 사용)
    
    ![Material 그림 4](material/image%203.png)
    
    - **Metallic(금속성)**: 0(비금속)~1(금속) 값으로 설정하여 물체의 재질을 결정.
    - **Roughness(거칠기)**: 표면의 부드러움을 나타내며, 낮을수록 매끈하고 높은 반사를 가짐.
    - **Albedo(기본 색상)**: 표면의 기본적인 색상과 빛의 반사 비율을 결정.
    - **Normal Map(노멀 맵)**: 표면의 미세한 요철을 표현하는 맵.
    - **AO(Ambient Occlusion, 환경 차폐)**: 주변광을 차단하여 더 사실적인 음영 효과를 추가.
2. **Specular-Glossiness 모델** (기존 엔진에서 사용되었으며, 점점 Metallic-Roughness 모델로 전환 중)
    
    ![Material 그림 5](material/image%204.png)
    
    - **Specular(반사광 색상)**: 특정 재질이 빛을 반사하는 성질을 나타냄.
    - **Glossiness(광택도)**: 표면이 빛을 얼마나 날카롭게 반사하는지를 결정함.

### **3. PBR의 장점**
- **일관된 조명 표현**: 다양한 조명 환경에서 일관된 표현을 유지할 수 있다.
- **물리적으로 정확한 반사 효과**: 금속과 비금속의 차이를 명확하게 반영할 수 있다.
- **재사용성 높은 머티리얼**: 하나의 PBR 머티리얼을 다양한 조명 조건에서도 그대로 활용 가능하다.

---

## **유니티(Unity)에서의 Material**
![Material 그림 6](material/image%205.png)

### **1. Material의 기본 개념**
- 유니티에서 Material은 **Shader**와 **Texture**를 포함하여 객체의 외형을 결정한다.
- Material은 다양한 렌더링 파이프라인(URP, HDRP, Built-in)에 따라 다르게 동작한다.

### **2. 유니티의 렌더링 파이프라인과 Material**
- **Built-in Render Pipeline**
    
    > 기본적인 Standard Shader를 사용하며, PBR(Physically Based Rendering)을 지원한다.
    > 
    
- **Universal Render Pipeline(URP)**
    
    > 성능을 최적화하면서도 품질을 유지할 수 있도록 설계된 Material 구조를 사용한다.
    > 

- **High Definition Render Pipeline(HDRP)**
    
    > 고품질 그래픽을 위한 고급 Material과 Shader(Graph-based Shader 포함)를 지원한다.
    > 

### **3. 주요 Shader 및 Material 속성**
- **Standard Shader**: PBR을 지원하며, Metallic 및 Roughness 기반 렌더링을 적용.
- **Lit Shader (URP/HDRP)**: PBR 기반으로 조명과 그림자 효과를 세밀하게 조정할 수 있는 Shader.
- **Unlit Shader**: 조명의 영향을 받지 않는 Material로 UI나 특수 효과에 많이 사용됨.

### **4. 유니티에서 Material 적용하기**
- Material은 **Mesh Renderer**, **Sprite Renderer**, **Skinned Mesh Renderer** 등에 적용할 수 있다.
- Shader Graph를 사용하여 사용자 정의 Shader를 제작할 수 있음.

---

## **언리얼 엔진(Unreal Engine)에서의 Material**
![Material 그림 7](material/image%206.png)

### **1. 언리얼 엔진의 Material 시스템**
- 언리얼 엔진에서는 Material Editor를 사용하여 노드 기반으로 Material을 제작할 수 있다.
- PBR을 기본적으로 지원하며, 다양한 Blend Mode(불투명, 투명, 마스크 등)를 제공한다.

### **2. 주요 Material 유형**
- **Opaque(불투명)**: 조명을 반사하고, 투명도가 없는 일반적인 Material.
- **Masked(마스크 적용)**: 특정 영역을 투명하게 만들 수 있는 Material.
- **Translucent(반투명)**: 반투명 효과를 줄 수 있는 Material.
- **Emissive(발광)**: 자체적으로 빛을 내는 Material로, 네온사인이나 홀로그램 표현에 사용됨.

### **3. Material Instance**
- 기본 Material을 기반으로 **Material Instance**를 생성하여 성능을 최적화할 수 있다.
- Material Instance를 사용하면 여러 개의 객체가 동일한 Material을 공유하면서 개별적인 속성을 변경할 수 있음.

---

## **비교 및 결론**
| 게임 엔진 | 주요 Material 시스템 |
| --- | --- |
| Unity | Standard Shader, Shader Graph, PBR 지원 |
| Unreal Engine | Material Editor 기반, PBR 지원, Material Instance 제공 |