using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiSinglePlay : MonoBehaviour
{
    public void BackBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
