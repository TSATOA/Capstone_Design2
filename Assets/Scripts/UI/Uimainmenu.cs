using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Uimainmenu : MonoBehaviour
{
    public void SingleBtn()
    {
        SceneManager.LoadScene("SinglePlay");
    }
    public void MultiBtn()
    {
        SceneManager.LoadScene("MultiPlay");
    }
    public void SettingBtn()
    {
        SceneManager.LoadScene("Settings");
    }
}
