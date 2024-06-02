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
        barImage.fillAmount = health;
    }

    // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ô´ï¿½ ï¿½ï¿½ï¿??
    public void takeDamge(float damage)
    {
        health -= damage;
        barImage.fillAmount = health;
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
