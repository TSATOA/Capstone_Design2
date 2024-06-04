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
    private GameObject bloodInstance = null;
    private GameObject playerMesh = null;

    // Start is called before the first frame update
    void Start()
    {
        isAiming = false;
        playerMesh = gameObject.transform.Find("Medieval_warriors").gameObject;
    }

    // 데미지를 입는 경우
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

        // 플레이어 시야에 따라서 mesh 활성화 / 비활성화
        gameObject.GetComponent<PlayerMeshControl>().ControlMesh(isAiming);
    }

    private void DisableColliders(GameObject parentObject)
    {
        // parentObject의 모든 자식 Collider들을 가져옵니다.
        Collider[] childColliders = parentObject.GetComponentsInChildren<Collider>();

        // 각 Collider를 비활성화합니다.
        foreach (Collider collider in childColliders)
        {
            collider.enabled = false;
        }
    }
}
