using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationFunctions : MonoBehaviour
{
    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
