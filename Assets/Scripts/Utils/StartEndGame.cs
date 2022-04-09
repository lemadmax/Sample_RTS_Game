using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class StartEndGame : MonoBehaviour
{
    public void EndClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void StartClick()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene(1);
#else
        Application.LoadLevel(1);
#endif
    }
}