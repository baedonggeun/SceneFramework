/*
UI

모든 사용자 인터페이스 관련 스크립트의 기반 구조가 위치합니다.

하위 구성

1. Base
   - UIBase: 공통 UI 캐싱, Show/Hide, Opened 구조 제공
   - UIButtonBase / UISliderBase / UITextBase: 기능 분리된 UI 요소 기반 클래스

2. Controls
   - 버튼(Button), 슬라이더(Slider), 텍스트(Text)로 나뉘며,
   - 각 UI 요소는 자신의 동작 책임만 수행하며 재사용 가능하게 설계됨

주요 목적:
- UI prefab에 붙이는 전용 로직을 스크립트로 분리하여 SRP 구현
- 디자이너는 prefab 구조 설계, 개발자는 로직 컴포넌트만 작성
- UI 자동 캐싱 및 이름 기반 접근 구조 지원

적용 예시:
- UIButtonSceneTransition: 버튼 클릭 시 씬 전이
- UISliderVolumeControl: AudioMixer와 연동된 볼륨 조절
- UITextScoreChange: 점수 변화를 실시간으로 텍스트에 반영

*/