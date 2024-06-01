using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponeHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public ProjectileHandler Arrow_Shoot_Network_Prefab;

    [Header("Aim")]
    public Transform aimPoint;

    [Header("Collision")]
    public LayerMask collisionLayers;

    float lastTimeShooted = 0;
    TickTimer projectileDelay = TickTimer.None;

    [Networked(OnChanged = nameof(OnShootingChanged))]
    public bool isShooting {get;set;}

    //other components
    HPHandler hPHandler;
    NetworkPlayer networkPlayer;


    public void Start(){
        hPHandler = GetComponent<HPHandler>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }
    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData networkInputData)){
            if(networkInputData.isShootingStart){
                Shoot(networkInputData.aimForwardVector);
            }
        }
    }


    void Shoot(Vector3 aimForwardVector){
        //�������� ���� �߻� ����
        //if(Time.time - lastTimeShooted < .15f){
        //    return;
        //}
        
        if(projectileDelay.ExpiredOrNotRunning(Runner)){
            Runner.Spawn(Arrow_Shoot_Network_Prefab,aimPoint.position+aimForwardVector*1.5f, Quaternion.LookRotation(aimForwardVector)*Quaternion.Euler(90, 0, 0), Object.InputAuthority, (runner,spawnedProjectile)=>{
                spawnedProjectile.GetComponent<ProjectileHandler>().Shoot(aimForwardVector*10f,Object.InputAuthority,networkPlayer.nickName.ToString());
            });
            projectileDelay = TickTimer.CreateFromSeconds(Runner,1.0f);
        }

        //StartCoroutine(ShootEffect());

        //lastTimeShooted = Time.time;
        
    }

    
    IEnumerator ShootEffect(){
        isShooting = true;
        // �� �κп� ȭ�� �߻� ����Ʈ, ���� �� �߰��ϸ� �ȴ�.
        yield return new WaitForSeconds(0.09f);
        isShooting = false;
    }
    static void OnShootingChanged(Changed<WeaponeHandler> changed){
        Debug.Log($"{Time.time} OnShootingChanged vlaue {changed.Behaviour.isShooting}");
        bool isShootingCurrent = changed.Behaviour.isShooting;

        changed.LoadOld();
        bool isShootingOld = changed.Behaviour.isShooting;

        if(isShootingCurrent && !isShootingOld){
            changed.Behaviour.OnShootRemote();
        }
        
    }

    void OnShootRemote(){
        if(!Object.HasInputAuthority){
            //�� �κп� ShootEffect�� �ִ� �Ͱ� �����ϰ� ȭ�� �߻� ����Ʈ, ���带 �߰����־� �ٸ� �÷��̾ ���̰� �Ѵ�.
        }
    }
}
