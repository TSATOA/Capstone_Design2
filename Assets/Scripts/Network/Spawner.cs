using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.Diagnostics;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    //token id와 recreated player 매핑
    Dictionary<int, NetworkPlayer> mapTokenIDWithNetworkPlayer;

    //Other components
    CharacterInputHandler characterInputHandler;
    SessionListUIHandler sessionListUIHandler;

    void Awake(){
        mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }
    void Start()
    {
        
    }

    int GetPlayerToken(NetworkRunner runnerm, PlayerRef player){
        if(runnerm.LocalPlayer == player){
            return ConnectionTokenUtils.HashToken(GameManager.instance.GetConnectionToken());
        }else{
            var token = runnerm.GetPlayerConnectionToken(player);

            if(token != null)
                return ConnectionTokenUtils.HashToken(token);
            Debug.LogError($"GetPlayerToken returned invalid token");

            return 0;
        }
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer networkPlayer){
        mapTokenIDWithNetworkPlayer.Add(token, networkPlayer);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 플레이어가 방 입장 시 초기화
        // Host에서 player spawn함
        if(runner.IsServer){
            int playerToken = GetPlayerToken(runner,player);

            Debug.Log($"OnPlayerJoined we are server. Connection token {playerToken}");

            if(mapTokenIDWithNetworkPlayer.TryGetValue(playerToken,out NetworkPlayer networkPlayer)){
                Debug.Log($"Found old connection token for token {playerToken}. Assigning controlls to that player");

                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);

                networkPlayer.Spawned();
            }else{
                Debug.Log($"Spawning new player for connection token {playerToken}");
                NetworkPlayer spawnNetworkPlayer = runner.Spawn(playerPrefab,Utils.GetRandomSpawnPoint(),Quaternion.identity,player);
            
                spawnNetworkPlayer.token = playerToken;

                mapTokenIDWithNetworkPlayer[playerToken] = spawnNetworkPlayer;
            }

        }else{
            Debug.Log("OnPlayerJoined");
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if(characterInputHandler == null && NetworkPlayer.Local != null){
            characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();
        }
        if(characterInputHandler != null){
            input.Set(characterInputHandler.GetNetworkInput());
        }

    }


    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnectRequest");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");

        await runner.Shutdown(shutdownReason : ShutdownReason.HostMigration);
        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // 방 리스트 초기화
        if(sessionListUIHandler == null)
            return;
        if(sessionList.Count == 0){
            Debug.Log("Joined lobby no session found");
            sessionListUIHandler.OnNoSessionFound();
        }else{
            sessionListUIHandler.ClearList();
            foreach(SessionInfo sessionInfo in sessionList){
                sessionListUIHandler.AddToList(sessionInfo);
                Debug.Log($"Found session {sessionInfo.Name} playerCount {sessionInfo.PlayerCount}");
            }
        }

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }


    public void OnHostMigrationCleanUp(){
        Debug.Log("Spawner OnHostMigrationCleanUp started");
        foreach(KeyValuePair<int, NetworkPlayer> entry in mapTokenIDWithNetworkPlayer){
            NetworkObject networkObjectInDictionary = entry.Value.GetComponent<NetworkObject>();
            if(networkObjectInDictionary.InputAuthority.IsNone){
                Debug.Log($"{Time.time} Found player that has not reconnected. Despawning {entry.Value.nickName}");

                networkObjectInDictionary.Runner.Despawn(networkObjectInDictionary);
            }
        }
        Debug.Log("Spawner OnHostMigrationCleanUp completed");
    }

    
}
