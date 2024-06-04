using System;
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

    // �������� �Դ� ���
    public void takeDamge(float damage, Transform hitPos, Quaternion hitDir)
    {
        health -= damage;

        bloodInstance = Instantiate(bloodPrefab, hitPos.position, hitDir);

        //Debug.DrawRay(hitPos.position, hitDir * Vector3.forward * 2, Color.red, 2.0f);

        if (health <= 0)
        {
            animator.enabled = true;
            animator.SetTrigger("Death");
            DisableColliders(gameObject);
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

    private void DisableColliders(GameObject parentObject)
    {
        // parentObject�� ��� �ڽ� Collider���� �����ɴϴ�.
        Collider[] childColliders = parentObject.GetComponentsInChildren<Collider>();

        // �� Collider�� ��Ȱ��ȭ�մϴ�.
        foreach (Collider collider in childColliders)
        {
            collider.enabled = false;
        }
    }
}
