using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiSinglePlay : MonoBehaviour
{
    public GameObject statusPanel;
    public void PlaySingle()
    {
        statusPanel.SetActive(true);
        SceneManager.LoadScene("SingleMap");
    }
    public void BackBtn()
    {
        SceneManager.LoadScene("Title");
    }
    public void PlayAgain()
    {
        SceneManager.LoadScene("SinglePlay");
    }
}
