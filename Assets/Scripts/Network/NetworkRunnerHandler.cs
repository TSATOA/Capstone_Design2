using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;


public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    NetworkRunner networkRunner;

    private void Awake(){
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();
        if(networkRunnerInScene != null)
            networkRunner = networkRunnerInScene;
    }
    
    void Start()
    {
        //Main시작 시 NetworkRunner 생성
        if(networkRunner == null){
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";

            if(SceneManager.GetActiveScene().name != "MainMenu"){
                //초기 NetworkRunner 초기화
                var clientTask = InitializeNetworkRunner(networkRunner,GameMode.AutoHostOrClient, "TestSession", GameManager.instance.GetConnectionToken(), NetAddress.Any(),SceneManager.GetActiveScene().buildIndex, null);
            }
            Debug.Log($"Server NetworkRunner started.");
        }
        
    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken){
        //새 networkrunner 생성
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner - Migrated";
        var clientTask = InitializeNetworkRunnerHostMigration(networkRunner,hostMigrationToken);
        Debug.Log($"Host migration started");
    }


    INetworkSceneManager GetSceneManager(NetworkRunner runner){
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        if(sceneManager == null){
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }
        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, byte[] connectionToken, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized){
        var sceneManager = GetSceneManager(runner);
        runner.ProvideInput = true;
        return runner.StartGame(new StartGameArgs{
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Initialized = initialized,
            SceneManager = sceneManager,
            ConnectionToken = connectionToken
        });
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){
        var sceneManager = GetSceneManager(runner);
        runner.ProvideInput = true;
        return runner.StartGame(new StartGameArgs{
            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken, //restart전 모든 필요한 정보 들어있음
            HostMigrationResume = HostMigrationResume,
            ConnectionToken = GameManager.instance.GetConnectionToken()
        });
    }



    void HostMigrationResume(NetworkRunner runner){
        Debug.Log($"HostmigrationResume started");
        //client들은 old서버에만 존재할 수 있으므로 network objects를 recreate해야한다
        foreach(var resumeNetworkOnject in runner.GetResumeSnapshotNetworkObjects()){
            //player grab NetworkCharacterController가지고 있는 애들인 경우
            if(resumeNetworkOnject.TryGetBehaviour<NetworkCharacterControllerPrototypeCustom>(out var characterController)){
                runner.Spawn(resumeNetworkOnject,position:characterController.ReadPosition(),rotation:characterController.ReadRotation(),onBeforeSpawned:(runner,newNetworkObject)=>{
                    newNetworkObject.CopyStateFrom(resumeNetworkOnject);
                    
                    //추후 HP 구현 시 HP또한 복사하는 코드를 구현해야 한다.//////////////////
                    
                    //새 player 토큰연결
                    if(resumeNetworkOnject.TryGetBehaviour<NetworkPlayer>(out var oldNetworkPlayer)){
                        //reconnection을 위한 player token store
                        FindObjectOfType<Spawner>().SetConnectionTokenMapping(oldNetworkPlayer.token, newNetworkObject.GetComponent<NetworkPlayer>());
                    }
                });
            }
        }
        StartCoroutine(CleanUpHostMigration());
        Debug.Log($"HostmigrationResume completed");
    }
    
    IEnumerator CleanUpHostMigration(){
        yield return new WaitForSeconds(5.0f);
        FindObjectOfType<Spawner>().OnHostMigrationCleanUp();
    }

    public void OnJoinLobby(){
        //로비 입장 버튼
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby(){
        //로비 입장 시 초기화
        Debug.Log("JoinLobby started");

        string lobbyID = "OurLobbyID";
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);
        if(!result.Ok){
            Debug.LogError($"Unable to Join lobby {lobbyID}");
        }else{
            Debug.Log($"JoinLobby {lobbyID} ok");
        }
    }

    public void CreateGame(string sessionName, string sceneName){
        // 방 생성
        Debug.Log($"Create session {sessionName} scene {sceneName} build Index {SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}")}");
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, GameManager.instance.GetConnectionToken(), NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}"),null);
    }

    public void JoinGame(SessionInfo sessionInfo){
        Debug.Log($"Join session {sessionInfo.Name}");

        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, sessionInfo.Name, GameManager.instance.GetConnectionToken(), NetAddress.Any(), SceneManager.GetActiveScene().buildIndex,null);
    }
}

