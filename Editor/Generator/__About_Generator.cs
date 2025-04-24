/*
Generator (Editor 도구)

이 폴더는 Addressables 기반 자산을 자동화하거나,
ScenePreset 또는 PluginSO를 자동 등록/생성하는 "도구 스크립트"를 포함합니다.

주요 목적:
- 반복되는 ScenePreset 구성 절차 자동화
- Enum 키, SO 파일 자동 생성
- 플러그인 자동 삽입으로 실수 방지

포함 기능 예시:
- ScenePresetAutoBuilderWindow: 플러그인 자동 등록 UI
- KeyEnumGenerator: Addressable 키 enum 자동 생성기
- SOAssetCreator: 미리 정의된 플러그인 SO 생성기

실무에서는 Addressables 키, Preset 구성 등 반복 작업을 자동화해
협업 중 실수를 줄이고 초기 세팅 속도를 높입니다.
*/