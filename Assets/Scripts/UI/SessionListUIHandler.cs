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

    private List<SessionInfo> sessionList = new List<SessionInfo>();

    private void Awake(){
        ClearList();
    }

    public void ClearList(){
        foreach(Transform child in verticalLayoutGroup.transform){
            Destroy(child.gameObject);
        }
        sessionList.Clear();
        statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo){
        sessionList.Add(sessionInfo);
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

    // 세션 목록 업데이트 메소드
    public void UpdateSessionList(List<SessionInfo> updatedSessionList)
    {
        ClearList();
        foreach (SessionInfo sessionInfo in updatedSessionList)
        {
            AddToList(sessionInfo);
        }
    }

    // 세션 목록 크기 반환 메소드
    public int GetSessionListCount()
    {
        return sessionList.Count;
    }

    // 랜덤한 세션 정보 반환 메소드
    public SessionInfo GetRandomSession()
    {
        if (sessionList.Count > 0)
        {
            int randomIndex = Random.Range(0, sessionList.Count);
            return sessionList[randomIndex];
        }
        return null;
    }
}
