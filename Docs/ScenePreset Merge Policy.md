# ScenePreset 병합 정책 문서 (ScenePreset Merge Policy)

이 문서는 해당 Framework를 사용하는 프로젝트에서 ScenePresetSO를 병합하여 사용하는 경우, 
실행 순서, 의존성, 중복 등의 문제를 예방하고, 협업 혼선을 방지하기 위한 정책을 정의합니다.

ScenePresetSO 병합 시 충돌, 의존성 누락, 순환 참조 등의 문제를 방지하기 위한 기준 정책.

---

## 1. 중복 플러그인 처리

- 동일한 ScenePluginSO 인스턴스가 여러 Preset에 포함된 경우 1회만 병합 및 실행한다.
- PluginName이 같으나 참조가 다르면 경고를 출력하고, 먼저 등장한 플러그인을 기준으로 병합한다.

---

## 2. 플러그인 정렬 순서

- 모든 플러그인은 IInjectable.GetDependencies()를 통해 추출된 종속성을 기준으로 위상 정렬한다.
- 종속성이 동일한 경우, subPreset 배열의 순서를 우선 적용한다.
- 병합 순서도 동일할 경우, PluginName의 알파벳 순으로 정렬한다.

---

## 3. 의존성 누락

- DependsOn 또는 IInjectable.GetDependencies()로 지정된 종속 플러그인이 병합된 목록에 없을 경우 오류로 처리한다.
- IsRequired가 true인 플러그인이 누락되면 검증 실패로 간주한다.

---

## 4. 병합 순서

- subPresets 배열에 등록된 순서대로 Preset이 병합된다.
- 병합 순서는 최종 플러그인 리스트 및 실행 순서에 영향을 준다.

---

## 5. 순환 참조 방지

- subPresets 필드에 자기 자신 또는 상위 Preset이 포함되면 안 된다.
- Preset 간 순환 참조가 감지되면 오류로 처리한다.

---

## 6. 정책 적용 위치

- ScenePluginExecutor.cs
- PresetValidationTool.cs
- PluginGraphWindow.cs

---

## 7. 협업 시 참고

- 병합 정책은 팀 내 모든 개발자와 공유되어야 하며,
- 정책을 위반하거나 예외 사항이 발생하면 GitHub PR에 반드시 명시해야 합니다.