using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP = 100;
    public Animator animator;

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            animator.SetTrigger("death");
        }
        else
        {
            animator.SetTrigger("hit");
        }
    }
}
