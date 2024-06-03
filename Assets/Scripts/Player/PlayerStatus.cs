using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public Animator animator;
    public float health;
    public GameObject bloodPrefab = null;

    private bool isAiming;
    private GameObject bloodInstance;

    // Start is called before the first frame update
    void Start()
    {
        isAiming = false;
    }

    // 데미지를 입는 경우
    public void takeDamge(float damage, Transform hitPos, Quaternion hitDir)
    {
        health -= damage;
        bloodInstance = Instantiate(bloodPrefab, hitPos.position, hitDir);
        if (health < 0)
        {
            animator.enabled = true;
            animator.SetTrigger("Death");
        }

        Destroy(bloodInstance, 2.0f);
        //Debug.Log("Player get Damaged!!: " + health);
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
