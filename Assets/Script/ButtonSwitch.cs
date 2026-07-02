using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonSwitch : MonoBehaviour
{
    public void LoadSceneByName()
    {
        SceneManager.LoadScene("Main");
    }


    public void LoadNextInBuild()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}