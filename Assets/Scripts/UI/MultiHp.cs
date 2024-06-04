using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiHp : MonoBehaviour
{
    //public Animator animator;
    public byte health;
    [SerializeField] private Image barImage;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        barImage.fillAmount = health / 100;
    }

    // �������� �Դ� ���??
    public void checkDamage()
    {

        //health = byte.Find("")
        barImage.fillAmount = health / 100;
        if (health <= 0)
        {
            //animator.enabled = true;
            //animator.SetTrigger("Death");
        }
    }
}
