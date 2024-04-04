using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup; 

    private void Awake(){
        ClearList();
    }

    public void ClearList(){
        foreach(Transform child in verticalLayoutGroup.transform){
            Destroy(child.gameObject);
        }
        statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo){
        SessionInfoListUIItem addedSessionInfoListUIIItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();
        addedSessionInfoListUIIItem.SetInformation(sessionInfo);
        addedSessionInfoListUIIItem.OnJoinSession += AddedSessionInfoListUIIItem_OnJoinSession;
    }

    private void AddedSessionInfoListUIIItem_OnJoinSession(SessionInfo sessionInfo){
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.JoinGame(sessionInfo);

        MainMenuUIHandler mainMenuUIHandler = FindObjectOfType<MainMenuUIHandler>();
        mainMenuUIHandler.OnJoiningServer();

    }

    public void OnNoSessionFound(){
        ClearList();
        statusText.text = "No game session found";
        statusText.gameObject.SetActive(true);
    }

    public void OnLookingForGameSessions(){
        ClearList();
        statusText.text = "Looking for game session";
        statusText.gameObject.SetActive(true);
    }
}
