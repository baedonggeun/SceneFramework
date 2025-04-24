/*
Debugger (Editor 도구)

InitScheduler로 초기화되는 각 매니저/서비스의 실행 결과를
시각적으로 디버깅하기 위한 도구들이 위치합니다.

주요 목적:
- 병렬/순차 초기화 그룹 시각화
- InitializeAsync 실행 성공/실패 추적
- 실행 순서, 시간, 우선순위 로그 표시

포함 기능 예시:
- InitDebuggerWindow: 디버그 UI
- InitLogView: 결과 요약 시각화

병렬 초기화 환경에서는 실행 순서 오류나 누락이 자주 발생하므로,
시각적 디버깅 도구는 필수입니다.
*/