using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public Animator animator;
    public float health;

    private bool isAiming;

    [SerializeField] private Image barImage;

    // Start is called before the first frame update
    void Start()
    {
        isAiming = false;
        health = 100;
        barImage.fillAmount = health / 100;
    }

    // �������� �Դ� ���??
    public void takeDamge(float damage)
    {
        health -= damage;
        barImage.fillAmount = health / 100;
        if (health <= 0)
        {
            animator.enabled = true;
            animator.SetTrigger("Death");
        }
    }

    public bool IsPlayerAiming()
    {
        return isAiming;
    }

    public void ChangePlayerPoseStatus(bool poseStatus)
    {
        isAiming = poseStatus;
    }
}
