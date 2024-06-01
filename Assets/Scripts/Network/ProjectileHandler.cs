using System.Collections;
using System.Collections.Generic;
using Fusion;
using ParrelSync.NonCore;
using UnityEngine;

public class ProjectileHandler : NetworkBehaviour
{
    [Header("Collsion detection")]
    public LayerMask collsionLayers;

    PlayerRef shootByPlayerRef;
    string shootByPlayerName;

    TickTimer DelTickTimer = TickTimer.None;

    //Hit info
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();


    //other components
    NetworkObject networkObject;
    NetworkRigidbody networkRigidbody;

    

    public void  Shoot(Vector3 shootForce, PlayerRef shootByPlayerRef, string shootByPlayerName){
        networkObject = GetComponent<NetworkObject>();
        networkRigidbody = GetComponent<NetworkRigidbody>();

        //ForceMode : Force - RigidBody���������� ���� ����. Impulse - �������� ��. Acceleration - �������� ���ӵ�. VelocityChange - �������� �ӵ� ��ȭ.
        networkRigidbody.Rigidbody.AddForce(shootForce*5f, ForceMode.Impulse);
        Debug.Log(shootForce);
        //networkRigidbody.transform.GetChild(0).GetChild(0).transform.GetComponent<Rigidbody>().AddForce(shootForce,ForceMode.Impulse);
        
        //transform.position += shootForce * Runner.DeltaTime;
        
        this.shootByPlayerRef = shootByPlayerRef;
        this.shootByPlayerName = shootByPlayerName ?? "Unknown";

        DelTickTimer = TickTimer.CreateFromTicks(Runner,500);
    }


    public override void FixedUpdateNetwork()
    {
        if(Object.HasStateAuthority){
            
                int hitCount = Runner.LagCompensation.OverlapSphere(transform.GetChild(0).GetChild(3).position, 0.3f, shootByPlayerRef, hits, collsionLayers);

                if(hitCount>0){
                    Debug.Log(hitCount);
                    HPHandler hPHandler = hits[0].Hitbox.transform.root.GetComponent<HPHandler>();
                    if(hPHandler != null){
                        //�� �κп��� hits[0].Hitbox.gameObject.tag�� ����Ͽ� ������ ������ ���� ����
                        hPHandler.OnTakeDamage(shootByPlayerName,50);
                        Debug.Log($"Hit {hPHandler.gameObject.name}, dealt 50 damage.");
                        Runner.Despawn(networkObject);
                        return;
                    }
                    
                }
                
                if(DelTickTimer.Expired(Runner)){
                    Runner.Despawn(networkObject);
                }
            }
        
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        
    }
    
}
