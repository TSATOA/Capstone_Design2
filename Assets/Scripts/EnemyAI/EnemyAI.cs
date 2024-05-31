using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform handTransform;
    public Animator animator;

    private GameObject enemyArrow;
    private Transform arrowHead;
    private Transform arrowTail;

    public float health;

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
    void takeDamge(float damage)
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
