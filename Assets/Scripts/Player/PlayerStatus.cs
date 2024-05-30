using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public Animator animator;
    public float health;

    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    // �������� �Դ� ���
    void takeDamge(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            animator.enabled = true;
            animator.SetTrigger("Death");
        }
    }
}
