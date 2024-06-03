using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void SingleBtn()
    {
        SceneManager.LoadScene("SinglePlay");
    }
    public void MultiBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void SettingBtn()
    {
        SceneManager.LoadScene("Settings");
    }
}
