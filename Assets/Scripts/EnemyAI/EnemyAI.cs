using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform handTransform;
    public float health;
    public Animator animator;

    private GameObject enemyArrow;


    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    // ȭ���뿡�� ȭ���� ������ �ִϸ��̼��� ����Ǵ� ���� �տ� ȭ�� �����Ͽ� ����
    void GrabArrow()
    {
        // ȭ�� ����
        enemyArrow = Instantiate(arrowPrefab, handTransform.position, handTransform.rotation);

        // ȭ���� ���� ArrowAttach �κ��� child�� ����
        enemyArrow.transform.parent = handTransform;

        // ȭ���� ��ġ, ȸ�� ���� ����
        //enemyArrow.transform.localPosition = Vector3.zero;
        //enemyArrow.transform.localRotation = Quaternion.identity; 
    }

    // Ȱ�� ��ܼ� �߻��ϴ� ���� ȭ���� �߻��ϴ� �Լ� ����
    void FireArrow_Enemy()
    {
        // ȭ���� �������� �����ϰ� ���� ȭ���� �߻� ������ ���·� ��ȯ
        enemyArrow.transform.parent = null;

        // ȭ���� �߻�
        enemyArrow.GetComponent<Arrow>().ReleaseArrow(10.0f);
    }

    // �������� �Դ� ���
    void takeDamge(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            animator.SetTrigger("Death");
        }
    }
}
