using System.Collections.Generic;
using UnityEngine;

public class UIButtonToggleTab : UIButtonBase
{
    [SerializeField] private GameObject targetOn;
    [SerializeField] private List<GameObject> targetsOff;

    protected override void OnClick()
    {
        if (targetOn != null) targetOn.SetActive(true);
        if (targetsOff.Count != 0)
        {
            foreach (GameObject target in targetsOff)
            {
                target.SetActive(false);
            }
        }
    }
}