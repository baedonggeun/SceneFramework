using UnityEngine;

public class LoadingUI : UIBase
{
    private UISliderTransitionLoading slider;

    public override bool IsGlobalUI() => true;

    public override void Opened(params object[] param)
    {
        base.Opened(param);
        slider = GetUI<UISliderTransitionLoading>("ProgressBar");
    }

    public void SetProgress(float progress)
    {
        slider?.SetProgress(progress);
    }

    public void CompleteAndHide()
    {
        slider?.SetProgress(1f);
        Hide();
    }
}
