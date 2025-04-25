//프로젝트에 존재하는 Scene의 이름과 같아야 함
public enum SceneKey
{
    Intro,
    Game,
    Global
}

//Monosingleton 을 상속받는 클래스의 타입과 같아야 함
public enum ServiceKey
{
    UIManager,
    AudioManager,
    ResolutionManager,
    SceneServiceManager,
    SceneTransitionManager,
    GameManager
}

//addressable 에 등록된 ScriptableObject의 addressable key 와 같아야 함
public enum SOKey
{
}

//addressable 에 등록된 UI Prefab의 addressable key 와 같아야 함
public enum UIKey
{
    LoadingUI
}

//addressable 에 등록된 Prefab의 addressable key 와 같아야 함
public enum PrefabKey
{
}

//addressable 에 등록된 AudioMixer의 addressable key 와 같아야 함
public enum AudioMixerKey
{
    MasterMixer
}

//exposed parameter 이름과 같아야 함
public enum SourceName
{
    Master, BGM, SFX
}

//AudioClip의 이름과 같아야 함
public enum BGMName
{
}

//AudioClip의 이름과 같아야 함
public enum SFXName
{
}