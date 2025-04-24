using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonQuitApplication : UIButtonBase
{
    protected override void OnClick()
    {
        // Quit the application
        Application.Quit();
        // If running in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
