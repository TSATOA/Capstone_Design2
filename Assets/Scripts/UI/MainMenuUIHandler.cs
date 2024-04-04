using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    [Header("Panels")]
    public GameObject playerDetailPanel;
    public GameObject sessionBrowserPanel;
    public GameObject createSessionPanel;
    public GameObject statusPanel;

    [Header("Player settings")]
    public TMP_InputField playerNameInputField;
    [Header("New game Session")]
    public TMP_InputField sessionNameInputField;

    void Start(){
        if(PlayerPrefs.HasKey("PlayerNickname")){
            playerNameInputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }
    void HideAllPanel(){
        playerDetailPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
        createSessionPanel.SetActive(false);
        statusPanel.SetActive(false);
    }
    public void OnFindGameClicked(){
        PlayerPrefs.SetString("PlayerNickname",playerNameInputField.text);
        PlayerPrefs.Save();

        GameManager.instance.playerNickName = playerNameInputField.text;

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobby();
        
        HideAllPanel();

        sessionBrowserPanel.SetActive(true);

        FindObjectOfType<SessionListUIHandler>(true).OnLookingForGameSessions();
    }

    public void OnCreateNewGameClicked(){
        HideAllPanel();
        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked(){
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.CreateGame(sessionNameInputField.text,"World1");
        HideAllPanel();
        statusPanel.SetActive(true);
    }

    public void OnJoiningServer(){
        HideAllPanel();
        statusPanel.SetActive(true);
    }
}
