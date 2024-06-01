using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP {get; set;} 

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead {get; set;} 

    //��ũ��Ʈ ����� �ʱ�ȭ �ߴ���
    bool isInitialize = false;

    const byte startingHP = 100;

    void Start()
    {
        HP = startingHP;
        isDead = false;
    }

    //�ش� �Լ��� ���� ���������� �Ҹ���.
    public void OnTakeDamage(string damageCauseByPlayerNickname, byte damageAmount){
        if(isDead)
            return;
        if(damageAmount>HP)
            damageAmount = HP;
        HP -= damageAmount;

        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

        if(HP <= 0){
            Debug.Log($"{Time.time} {transform.name} died");
            isDead = true;
        }
    }
    static void OnHPChanged(Changed<HPHandler> changed){
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<HPHandler> changed){
        Debug.Log($"{Time.time} OnStateChanged value {changed.Behaviour.isDead}");
    }
}
