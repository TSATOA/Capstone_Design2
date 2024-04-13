using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;


public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TextMeshProUGUI playerNickNameTM;
    public static NetworkPlayer Local {get; set;}

    public Transform playerModel;
    //닉네임은 host만 변경 가능
    //clinet는 자기 이름을 서버에 알려줘야함 -> rpc
    //계속 동기화 되는 변수 - 오직 서버에 의해서만 변경 가능
    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName {get;set;}

    [Networked] public int token {get; set;}
    public LocalCameraHandler localCameraHandler;
    void Start()
    {
        
    }
    public override void Spawned()
    {
        // player spawn
        if(Object.HasInputAuthority){
            Local = this;
            //local player layer set
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            //main camera 비활성화 
            if(Camera.main != null)
                Camera.main.gameObject.SetActive(false);

            //listener 1개로 
            AudioListener audioListener = GetComponentInChildren<AudioListener>(true);
            audioListener.enabled = false;

            localCameraHandler.localCamera.enabled = true;

            localCameraHandler.transform.parent = null;

            RPC_SentNickName(GameManager.instance.playerNickName);

            Debug.Log("Spawned local Player");
        }else{
            //Local camera가 아니면 비활성화
            localCameraHandler.localCamera.enabled = false;
            
            //listener 1개로 
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            Debug.Log("Spanwed remote player");
        }
        //이름 설정
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player){
        if(player == Object.InputAuthority){
            Runner.Despawn(Object);
        }
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed){
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.nickName}");
        changed.Behaviour.OnNickNameChanged();
    }

    public void OnNickNameChanged(){
        Debug.Log($"Nickname changed for player to {nickName} for player {gameObject.name}");
        playerNickNameTM.text = nickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SentNickName(string nickName, RpcInfo info = default){
        //RPC를 통해 이름 설정
        Debug.Log($"[RPC] SetNickName {nickName}");
        this.nickName = nickName; //NetworkPlayer 스크립트의 nickName variable 변경
    }

    void OnDestroy(){
        if(localCameraHandler!=null)
            Destroy(localCameraHandler.gameObject);

    }
}
