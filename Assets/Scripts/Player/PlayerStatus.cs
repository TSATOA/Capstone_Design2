using System;
using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public Animator animator;
    public float health;
    public GameObject bloodPrefab = null;

    private bool isAiming;
    private GameObject bloodInstance = null;
    private GameObject playerMesh = null;

    [SerializeField] private Image barImage;
    public GameObject ResultPage;
    // Start is called before the first frame update
    void Start()
    {
        isAiming = false;
        playerMesh = gameObject.transform.Find("Medieval_warriors").gameObject;
        barImage.fillAmount = 1;
    }

    // �������� �Դ� ���
    public void takeDamge(float damage, Transform hitPos, Quaternion hitDir)
    {
        health -= damage;
        barImage.fillAmount = health / 100;
        bloodInstance = Instantiate(bloodPrefab, hitPos.position, hitDir);

        //Debug.DrawRay(hitPos.position, hitDir * Vector3.forward * 2, Color.red, 2.0f);

        if (health <= 0)
        {
            animator.enabled = true;
            animator.SetTrigger("Death");
            DisableColliders(gameObject);
            ResultPage.SetActive(true);
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

        // �÷��̾� �þ߿� ���� mesh Ȱ��ȭ / ��Ȱ��ȭ
        gameObject.GetComponent<PlayerMeshControl>().ControlMesh(isAiming);
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
