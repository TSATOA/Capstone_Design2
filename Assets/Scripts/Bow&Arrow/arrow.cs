using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform arrowHead = null;
    public float destroyDelay = 15.0f;

    // 화살 속도(필요하면 public으로 변경해서 프리팹에서 직접 수정 가능)
    private float arrowSpeed = 100.0f;

    private float arrowDamage = 20.0f;

    // 피격 부위에 따른 데미지 배율
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

    // Lincast를 이용해서 충돌 처리를 하지 않을 경우 아래 메서드를 이용, 그 외의 경우 아래 메서드를 주석 처리하고 CheckForCollision을 이용
    private void OnTriggerEnter(Collider other)
    {
        if (isInAir)
        {
            StopArrow();

            GameObject otherGameObject = other.gameObject;
            GameObject parentObj = GetRootParent(otherGameObject);

            // 충돌이 발생한 부위를 확인하고 부위별 데미지 배율을 적용한 데미지 값을 이용해 takeDamage 메서드 호출
            // 플레이어 화살의 경우
            if (gameObject.CompareTag("Arrow_Player"))
            {
                if (otherGameObject.CompareTag("Enemy_Head"))
                {
                    parentObj.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Head);
                }
                else if (otherGameObject.CompareTag("Enemy_Body"))
                {
                    parentObj.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Body);
                }
                else if (otherGameObject.CompareTag("Enemy_Arm"))
                {
                    parentObj.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Arm);
                }
                else if (otherGameObject.CompareTag("Enemy_Leg"))
                {
                    parentObj.GetComponent<EnemyAI>().takeDamge(arrowDamage * damageMultiplier.Leg);
                }
            }
            // 적 AI 화살의 경우
            else if (gameObject.CompareTag("Arrow_Enemy"))
            {
                //Debug.Log("Arrow hit Player!!" + other.name);
                if (otherGameObject.CompareTag("Player_Head"))
                {
                    parentObj.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Head);
                }
                else if (otherGameObject.CompareTag("Player_Body"))
                {
                    //Debug.Log("Arrow hit Player Body!!!!");
                    parentObj.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Body);
                }
                else if (otherGameObject.CompareTag("Player_Arm"))
                {
                    parentObj.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Arm);
                }
                else if (otherGameObject.CompareTag("Player_Leg"))
                {
                    parentObj.GetComponent<PlayerStatus>().takeDamge(arrowDamage * damageMultiplier.Leg);
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
        //SetPhysics(false);
        Destroy(transform.GetComponent<Rigidbody>());

        StartCoroutine(DestroyArrowAfterDelay(destroyDelay));
    }

    public void ReleaseArrow(float power, Vector3 direction)
    {
        isInAir = true;
        SetPhysics(true);

        arrowDamage = power;

        FireArrow(direction);
        //StartCoroutine(RotateWithVelocity());

        lastPosition = arrowHead.position;
    }

    private void SetPhysics(bool usePhysics)
    {
        rigidBody.isKinematic = !usePhysics;
        rigidBody.useGravity = usePhysics;
    }

    private void FireArrow(Vector3 direction)
    {
        Vector3 force = direction * arrowSpeed;

        // for Debugging
        /*Debug.Log("Direction: " + direction);
        Debug.Log("power: " + power);
        Debug.Log("arrowSpeed: " + arrowSpeed);
        Debug.Log("Force" + force);
        Debug.DrawRay(transform.position, force, Color.red, 2.0f);
        */

        rigidBody.AddForce(force, ForceMode.Impulse);
    }

    // delay 시간이 지나면 화살을 파괴
    private IEnumerator DestroyArrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    GameObject GetRootParent(GameObject obj)
    {
        Transform currentParent = obj.transform;

        // 루트 부모를 찾을 때까지 반복
        while (currentParent.parent != null)
        {
            currentParent = currentParent.parent;
        }

        return currentParent.gameObject;
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
