/*
ScenePlugins

이 폴더는 플러그인 시스템 기반으로 자원을 로드하거나 서비스를 초기화하는
SO 기반 플러그인 클래스들을 포함합니다.

주요 목적:
- 각 책임별 플러그인 구조 분리
- Addressable 기반 프리팹, UI, ScriptableObject 로딩
- MonoSingleton 서비스 자동 생성 및 초기화

포함 기능 예시:
- AddressablePrefabPluginSO
- AddressableUIPluginSO
- AddressableSOPluginSO
- ServicePluginSO (InitScheduler 포함)

모든 플러그인은 ScenePresetSO에 등록되어 실행되며,
단일 책임 원칙을 기반으로 설계되어 확장성이 뛰어납니다.
*/