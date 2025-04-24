/*
MatchRules (Preset 선택 규칙)

이 폴더는 어떤 ScenePresetSO를 사용할지 결정하는
선택 규칙들을 ScriptableObject로 정의합니다.

주요 목적:
- 현재 씬 이름, 시간대, 상태값 등에 따라 Preset 선택
- 다중 Preset 중 조건 기반으로 선택 분기

포함 기능 예시:
- SceneMatchRuleSO: 전이 룰 베이스
- SceneNameContainsRuleSO: 이름 조건
- CompositeMatchRuleSO: 조합 조건

씬 전환 시 단일 조건이 아닌 유동적인 룰이 필요할 경우 유용합니다.
*/