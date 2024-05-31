using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyAI : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform handTransform;
    public Animator animator;

    private GameObject enemyArrow;
    private Transform arrowHead;
    private Transform arrowTail;
    private PlayerStatus playerStatus;

    // �÷��̾ �����ϰ� �ִ� ���� AI�� ȸ�� ������ �� Ȯ��
    public float evadeChance = 1.0f; // ȸ�� ���� ���� Ȯ�� (0 ~ 1)
    private bool isEvadeDone = false; // �÷��̾ �����ϴ� ���� �����⸦ ������ ������ �����Ͽ��°�? (������� ���ش� �ѹ��� ����)

    // �� NPC�� �����⸦ ������ Ƚ��
    private int leftEvade = 0;
    private int rightEvade = 0;
    private int maxEvade = 5; // ���� �������� �ִ� ������ ���� Ƚ��

    public float health;

    private void Start()
    {
        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        // ������ ���� �����̰� �÷��̾ ���� ���̶��
        if (!isEvadeDone && !playerStatus.IsPlayerAiming())
        {
            // ������ ���� Ȯ���� ���� ������ ����
            if (Random.value < evadeChance)
            {
                // ������ ����
                if (Random.value < 0.5f && leftEvade < maxEvade)
                {
                    // �������� ������
                    leftEvade++;
                    animator.SetTrigger("LeftEvade");
                }
                else
                {
                    // ���������� ������
                    rightEvade++;
                    animator.SetTrigger("RightEvade");
                }
            }

            isEvadeDone = true;
        }
        // �÷��̾ ������ Ǯ�� �ٽ� ������ ���� ���·� ����
        /*else if (isEvadeDone && !playerStatus.IsPlayerAiming())
        {
            isEvadeDone = false;
        }*/
    }

    // ȭ���뿡�� ȭ���� ������ �ִϸ��̼��� ����Ǵ� ���� �տ� ȭ�� �����Ͽ� ����
    void GrabArrow()
    {
        // ȭ�� ����
        enemyArrow = Instantiate(arrowPrefab, handTransform.position, handTransform.rotation);
        if (enemyArrow == null) return;

        // ȭ���˰� ȭ��� �κ��� Transform ����
        arrowHead = enemyArrow.transform.Find("ArrowHead");
        arrowTail = enemyArrow.transform.Find("ArrowTail");

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

        // ȭ�� ���� ���
        Vector3 direction = CalculateDirection(arrowTail, arrowHead);

        // ȭ���� �߻�
        enemyArrow.GetComponent<Arrow>().ReleaseArrow(10.0f, direction);
    }

    // �������� �Դ� ���
    public void takeDamge(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            animator.SetTrigger("Death");
        }
    }

    Vector3 CalculateDirection(Transform from, Transform to)
    {
        Vector3 direction = to.position - from.position;

        direction.Normalize();

        return direction;
    }
}
