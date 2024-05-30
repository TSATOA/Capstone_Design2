using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiSettings : MonoBehaviour
{
    public void BackBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
