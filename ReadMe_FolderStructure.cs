/*
하이브리드 씬 프레임워크 폴더 구조 개요

이 문서는 프로젝트의 최상위 폴더 구조를 정리한 개요 파일입니다.
각 폴더는 기능 단위로 역할이 분리되어 있으며, 개발자가 처음 구조를 파악하고 유지보수할 때 참고할 수 있도록 작성되었습니다.

────────────────────────────────────────────

📂 Core/
핵심 공통 구조를 담고 있으며, 대부분의 시스템에서 참조하는 기반 코드가 포함됩니다.

포함 요소:
- MonoSingleton: 모든 싱글톤 매니저를 위한 공통 구현
- IInitializable: InitScheduler 기반 비동기 초기화를 위한 인터페이스
- IScenePlugin, ISceneTransitionCondition: 플러그인 시스템 공통 인터페이스
- InitScheduler: 병렬 또는 순차 초기화를 수행하는 핵심 유틸리티

의존성 수준이 가장 낮으며, 모든 시스템에서 안전하게 참조 가능해야 합니다.

────────────────────────────────────────────

📂 Runtime/
게임 실행 중 동작하는 모든 런타임 시스템을 포함합니다.

포함 요소:
- Managers/: 게임 로직, UI, 씬 전환 등을 처리하는 싱글톤 매니저들
- ScenePlugins/: ScriptableObject 기반의 로딩/초기화 플러그인들
- Addressables/: AddressablePrefab, AddressableSO 등 자산 로딩 헬퍼

대부분 MonoSingleton 기반이며, ScenePresetSO → PluginSO → Manager 순으로 실행 흐름이 연결됩니다.

────────────────────────────────────────────

📂 Editor/
에디터에서만 동작하는 도구들을 포함합니다. 런타임 빌드에는 포함되지 않습니다.

주요 하위 폴더:
- Generator/: 키 Enum, SO 자동 생성기 등 초기 설정 자동화 도구
- GraphVisualizer/: PluginSO 간 의존성을 시각화하는 노드 기반 UI 도구
- PresetInitAutomation/: ScenePresetSO에 플러그인을 자동 등록하는 유틸리티
- Debugger/: Init 상태를 시각적으로 보여주는 디버깅 도구
- Validation/: 플러그인 누락, 순환 참조 등 유효성 검사기

개발자와 디자이너의 협업을 지원하며, 구조 오류를 사전에 방지하는 데 기여합니다.

────────────────────────────────────────────

📂 Presets/
모든 ScenePresetSO 관련 자산과 설정 데이터를 포함하는 구조의 중심입니다.

주요 하위 폴더:
- Plugins/: 실제 AddressableSOPluginSO, ServicePluginSO 등의 인스턴스
- Conditions/: ISceneTransitionCondition 기반 전이 조건 SO
- MatchRules/: ScenePreset 선택 조건을 구성하는 MatchRuleSO
- Keys/: 각종 Addressable 키를 Enum으로 정의하고 매핑하는 SO들

이 폴더는 씬 전이, 조건 판단, 플러그인 실행 흐름의 연결점 역할을 합니다.

────────────────────────────────────────────

📂 UI/
게임 UI의 베이스 구조와 Addressable 기반 프리팹 로딩 구조를 포함합니다.

포함 요소:
- UIBase.cs: 모든 UI의 공통 기반 클래스 (Canvas, Opened/Closed 등 제공)
- LoadingUI: 씬 전이 중 보여주는 UI
- 기타 Addressable UI 프리팹

UIManager에서 UI 이름 기반으로 Addressables를 통해 불러오고 관리합니다.

────────────────────────────────────────────

📂 Docs/
이 프레임워크의 설계 철학, 운영 정책, 구조 설명을 담는 문서 전용 폴더입니다.

포함 요소:
- ScenePreset Merge Policy.md
- README_FolderStructure.cs (이 파일)
- 기타 Onboarding 문서, 유지보수 가이드

실무 협업 시 설명 전달, 코드 리뷰 시 배경 설명, 새 팀원 교육 등에 매우 유용합니다.
*/

