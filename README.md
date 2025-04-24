# 🎬 SceneFramework

A hybrid scene management framework for Unity, powered by **Addressables** and **UniTask**.  
Includes preset-based scene loading, transition management, and dependency injection.

---

## 📦 설치 방법 (UPM via Git)

### ✅ 1. Git URL로 설치

1. Unity에서 메뉴: `Window > Package Manager`
2. 왼쪽 상단 `+` 버튼 → `Add package from Git URL...`
3. Git 주소 입력 : https://github.com/baedonggeun/SceneFramework.git

> `package.json` 기준으로 `Addressables`는 자동 설치됩니다.

---

## 📥 필수 의존 패키지 수동 설치

### 🔹 UniTask 설치 (Cysharp)

1. Unity에서 `Window > Package Manager`
2. `+ > Add package from Git URL...`
3. URL 입력 : https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask

> UniTask는 Unity의 async-await을 대체하는 고성능 비동기 라이브러리입니다.

---

## 🧪 샘플 에셋 Import (필수 ScriptableObjects 포함)

1. Unity에서 `Window > Package Manager` 열기
2. `Scene Framework` 선택
3. 오른쪽 하단 `Import Samples > Essential Scene Assets` 클릭

샘플 에셋은 다음 경로에 설치됩니다 : Assets/Samples/Scene Framework/1.0.0/Essential Scene Assets/

---

## ⚙️ 에셋 경로 정리 (선택사항)

샘플을 프로젝트 루트의 깔끔한 경로로 옮기고 싶다면:

1. 메뉴 → `Tools/Hybrid Scene Framework/Move Imported Samples to Assets`
2. 자동으로 아래 경로로 이동됨 : Assets/SceneFramework/


---

## 📁 폴더 구조

Assets/ 
└── SceneFramework/ 
	├── Bootstrap/ 
	├── Core/ 
	├── Presets/ 
	├── Resources/ 
└── ...


---

## 🧩 포함 기능

- ✅ Scene transition preset system
- ✅ Addressables-based scene loading
- ✅ UniTask 기반 async 지원
- ✅ Resources & ScriptableObject 키 기반 로딩
- ✅ 확장 가능한 ScenePlugin 구조

---

## 📄 라이선스

MIT © 2025 Donggeun Bae