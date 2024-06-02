using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform arrowHead = null;
    public float destroyDelay = 15.0f;
    public float arrowDamage = 50.0f;

    // ȭ�� �ӵ�(�ʿ��ϸ� public���� �����ؼ� �����տ��� ���� ���� ����)
    private float arrowSpeed = 10.0f;

    // �ǰ� ������ ���� ������ ����
    private static class damageMultiplier
    {
        public const float Head = 2.0f;
        public const float Body = 1.0f;
        public const float Arm = 0.8f;
        public const float Leg = 0.7f;
    }

    private Rigidbody rigidBody = null;
    private bool isInAir = false;
    private Vector3 lastPosition = Vector3.zero;

    protected void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isInAir)
        {
            //CheckForCollision();
            lastPosition = arrowHead.position;
        }
    }

    // Lincast�� �̿��ؼ� �浹 ó���� ���� ���� ��� �Ʒ� �޼��带 �̿�, �� ���� ��� �Ʒ� �޼��带 �ּ� ó���ϰ� CheckForCollision�� �̿�
    private void OnTriggerEnter(Collider other)
    {
        if (isInAir)
        {
            StopArrow();

            GameObject otherGameObject = other.gameObject;

            // �浹�� �߻��� ������ Ȯ���ϰ� ������ ������ ������ ������ ������ ���� �̿��� takeDamage �޼��� ȣ��
            // �÷��̾� ȭ���� ���
            if (gameObject.CompareTag("Arrow_Player")) 
            {
                if (otherGameObject.CompareTag("Enemy_Head"))
                {
                    otherGameObject.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Head);
                }
                else if (otherGameObject.CompareTag("Enemy_Body"))
                {
                    otherGameObject.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Body);
                }
                else if (otherGameObject.CompareTag("Enemy_Arm"))
                {
                    otherGameObject.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Arm);
                }
                else if (otherGameObject.CompareTag("Enemy_Leg"))
                {
                    otherGameObject.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Leg);
                }
            }
            // �� AI ȭ���� ���
            else if (gameObject.CompareTag("Arrow_Enemy"))
            {
                if (otherGameObject.CompareTag("Player_Head"))
                {
                    otherGameObject.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Head);
                }
                else if (otherGameObject.CompareTag("Player_Body"))
                {
                    otherGameObject.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Body);
                }
                else if (otherGameObject.CompareTag("Player_Arm"))
                {
                    otherGameObject.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Arm);
                }
                else if (otherGameObject.CompareTag("Player_Leg"))
                {
                    otherGameObject.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Leg);
                }
            }

            transform.position = other.ClosestPoint(transform.position);
        }
    }

    /*
    private void CheckForCollision()
    {
        if (Physics.Linecast(lastPosition, arrowHead.position))
            StopArrow();
    }
    */

    private void StopArrow()
    {
        isInAir = false;
        SetPhysics(false);

        StartCoroutine(DestroyArrowAfterDelay(destroyDelay));
    }

    public void ReleaseArrow(float pullValue, Vector3 direction)
    {
        isInAir = true;
        SetPhysics(true);

        FireArrow(pullValue, direction);
        //StartCoroutine(RotateWithVelocity());

        lastPosition = arrowHead.position;
    }

    private void SetPhysics(bool usePhysics)
    {
        rigidBody.isKinematic = !usePhysics;
        rigidBody.useGravity = usePhysics;
    }

    private void FireArrow(float power, Vector3 direction)
    {
        Vector3 force = direction * (power * arrowSpeed);

        // for Debugging
        /*Debug.Log("Direction: " + direction);
        Debug.Log("power: " + power);
        Debug.Log("arrowSpeed: " + arrowSpeed);
        Debug.Log("Force" + force);
        Debug.DrawRay(transform.position, force, Color.red, 2.0f);
        */

        rigidBody.AddForce(force, ForceMode.Impulse);
    }

    // delay �ð��� ������ ȭ���� �ı�
    private IEnumerator DestroyArrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    
    /*private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();

        while (isInAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(rigidBody.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }
    */
}
