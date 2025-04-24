/*
Conditions (조건 기반 전이 시스템)

이 폴더는 씬 전이 시 사용할 조건들을 ScriptableObject로 정의한
플러그인화 구조를 담고 있습니다.

주요 목적:
- 특정 아이템 보유 여부, 플래그 상태 등 조건으로 전이 제어
- 조건 조합 (AND/OR) 가능
- UI/버튼과 SO 연동하여 조건 기반 전이 구현

포함 기능 예시:
- CheckHasItemConditionSO
- CheckFlagConditionSO
- AndConditionSO (복합 조건)

조건 로직을 SO로 분리하면 디자이너도 구성 가능하며,
유지보수성과 확장성이 비약적으로 증가합니다.
*/