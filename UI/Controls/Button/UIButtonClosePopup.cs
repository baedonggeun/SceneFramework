using UnityEngine;

public class UIButtonClosePopup : UIButtonBase
{
    [SerializeField] private UIBase target;

    protected override void OnClick()
    {
        if (target != null)
            target.Hide();
        else
            gameObject.GetComponentInParent<UIBase>()?.Hide();
    }
}