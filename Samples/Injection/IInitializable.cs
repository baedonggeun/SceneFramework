using Cysharp.Threading.Tasks;

public interface IInitializable
{
    //비동기 초기화 함수
    UniTask InitializeAsync();

    //초기화 우선순위(낮을수록 먼저 초기화)
    int Priority { get; }

    // true일 경우 ScenePreset 로딩 시 자동 초기화됨
    // false일 경우 명시적으로 호출해야 초기화됨
    bool AutoInitialize { get; }
}
