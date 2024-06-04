using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlay : MonoBehaviour
{
    public GameObject Loading;
    public void SingleStart()
    {
        Loading.SetActive(true);
        SceneManager.LoadScene("SingleMap");
    }
    public void BackBtn()
    {
        SceneManager.LoadScene("Title");
    }
    
}
