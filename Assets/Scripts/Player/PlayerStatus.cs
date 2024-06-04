using System;
using System.Collections;
using System.Collections.Generic;
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

    public GameObject ResultPage;

    [SerializeField] private Image barImage;

    // Start is called before the first frame update
    void Start()
    {
        isAiming = false;
<<<<<<< HEAD
        health = 100;
        barImage.fillAmount = health / 100;
    }

    // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ô´ï¿½ ï¿½ï¿½ï¿½??
    public void takeDamge(float damage)
    {
        health -= damage;
        barImage.fillAmount = health / 100;
=======
        playerMesh = gameObject.transform.Find("Medieval_warriors").gameObject;
    }

    // µ¥¹ÌÁö¸¦ ÀÔ´Â °æ¿ì
    public void takeDamge(float damage, Transform hitPos, Quaternion hitDir)
    {
        health -= damage;

        bloodInstance = Instantiate(bloodPrefab, hitPos.position, hitDir);

        //Debug.DrawRay(hitPos.position, hitDir * Vector3.forward * 2, Color.red, 2.0f);

>>>>>>> 74c7bbd7b5f9507d25eef3cbc2832866bebc2c25
        if (health <= 0)
        {
            animator.enabled = true;
            animator.SetTrigger("Death");
<<<<<<< HEAD
            ResultPage.SetActive(true);
=======
            DisableColliders(gameObject);
>>>>>>> 74c7bbd7b5f9507d25eef3cbc2832866bebc2c25
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

        // ÇÃ·¹ÀÌ¾î ½Ã¾ß¿¡ µû¶ó¼­ mesh È°¼ºÈ­ / ºñÈ°¼ºÈ­
        gameObject.GetComponent<PlayerMeshControl>().ControlMesh(isAiming);
    }

    private void DisableColliders(GameObject parentObject)
    {
        // parentObjectÀÇ ¸ðµç ÀÚ½Ä ColliderµéÀ» °¡Á®¿É´Ï´Ù.
        Collider[] childColliders = parentObject.GetComponentsInChildren<Collider>();

        // °¢ Collider¸¦ ºñÈ°¼ºÈ­ÇÕ´Ï´Ù.
        foreach (Collider collider in childColliders)
        {
            collider.enabled = false;
        }
    }
}
