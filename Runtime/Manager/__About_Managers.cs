/*
Managers

실행 중 지속적으로 동작하는 MonoSingleton 기반의 매니저들이 위치합니다.

주요 목적:
- UI, 씬 전이, 게임 로직 등 핵심 시스템 관리
- ServicePluginSO 통해 자동 생성 및 초기화

포함 기능 예시:
- GameManager
- SceneServiceManager, SceneTransitionManager
- UIManager, ResolutionManager, AudioManager

대부분 IInitializable을 구현하며,
InitScheduler를 통해 병렬 또는 순차 초기화됩니다.
*/