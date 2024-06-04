using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiPlay : MonoBehaviour
{
    public void PlayAgainBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void BackBtn()
    {
        SceneManager.LoadScene("Title");
    }
    
}
