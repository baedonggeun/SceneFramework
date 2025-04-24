/*
Validation (Editor 도구)

ScenePresetSO의 구성이 올바른지 자동으로 검사하는
정적 유효성 검사 도구가 포함됩니다.

주요 목적:
- 중복 플러그인 등록 감지
- 미지정 의존성 누락 검출
- ScenePluginSO 구조 규칙 위반 감지

포함 기능 예시:
- PresetValidationTool: 유효성 검사 메인 클래스
- PluginValidator: 플러그인 목록 검사기

프로젝트가 커질수록 누락된 플러그인, 순서 꼬임 등의 문제가 커지므로,
자동 검사 도구는 배포 안정성에 매우 중요합니다.
*/