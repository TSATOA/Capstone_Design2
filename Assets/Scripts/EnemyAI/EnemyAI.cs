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
    private GameObject player;
    private PlayerStatus playerStatus;

    // 플레이어가 조준하고 있는 동안 AI가 회피 동작을 할 확률
    public float evadeChance = 1.0f; // 회피 동작 성공 확률 (0 ~ 1)
    private bool isEvadeDone = false; // 플레이어가 조준하는 동안 구르기를 수행할 것인지 결정하였는가? (구르기는 조준당 한번만 수행)

    // 적 NPC가 구르기를 수행한 횟수
    private int leftEvade = 0;
    private int rightEvade = 0;
    private int maxEvade = 5; // 한쪽 방향으로 최대 구르기 가능 횟수

    public float health;
    public float arrowPower = 10.0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStatus = player.GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        // Idle 상태라면 플레이어를 바라보게 회전
        if (IsIdle())
        {
            LookAtPlayer();
        }
        // 구르기 가능 상태이고 플레이어가 조준 중이라면
        if (!isEvadeDone && playerStatus.IsPlayerAiming())
        {
            // 구르기 성공 확률에 따라 구르기 실행
            if (Random.value < evadeChance)
            {
                // 구르기 실행
                if (Random.value < 0.5f && leftEvade < maxEvade)
                {
                    // 왼쪽으로 구르기
                    leftEvade++;
                    rightEvade--;
                    if (rightEvade < 0) rightEvade = 0;
                    animator.SetTrigger("LeftEvade");
                }
                else
                {
                    // 오른쪽으로 구르기
                    rightEvade++;
                    leftEvade--;
                    if (leftEvade < 0) leftEvade = 0;
                    animator.SetTrigger("RightEvade");
                }
            }

            isEvadeDone = true;
        }
        // 플레이어가 조준을 풀면 다시 구르기 가능 상태로 변경
        else if (isEvadeDone && !playerStatus.IsPlayerAiming())
        {
            isEvadeDone = false;
        }
    }

    // 화살통에서 화살이 꺼내는 애니메이션이 재생되는 순간 손에 화살 생성하여 부착
    void GrabArrow()
    {
        // 화살 생성
        enemyArrow = Instantiate(arrowPrefab, handTransform.position, handTransform.rotation);
        if (enemyArrow == null) return;

        // 화살촉과 화살깃 부분의 Transform 저장
        arrowHead = enemyArrow.transform.Find("ArrowHead");
        arrowTail = enemyArrow.transform.Find("ArrowTail");

        // 화살을 손의 ArrowAttach 부분의 child로 설정
        enemyArrow.transform.parent = handTransform;

        // 화살의 위치, 회전 세부 조정
        //enemyArrow.transform.localPosition = Vector3.zero;
        //enemyArrow.transform.localRotation = Quaternion.identity; 
    }

    // 활을 당겨서 발사하는 순간 화살을 발사하는 함수 실행
    void FireArrow_Enemy()
    {
        // 화살을 시위에서 제거하고 다음 화살을 발사 가능한 상태로 전환
        enemyArrow.transform.parent = null;

        // 화살 방향 계산
        Vector3 direction = CalculateDirection(arrowTail, arrowHead);

        // 화살을 발사
        enemyArrow.GetComponent<Arrow>().ReleaseArrow(arrowPower, direction);
    }

    // AI의 상태가 idle인지 확인
    private bool IsIdle()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Idle");
        }
        return false;
    }

    // 플레이어를 바라보도록 회전
    public void LookAtPlayer()
    {
        if (player != null)
        {
            //Debug.Log("LookAtPlayer!!");
            transform.LookAt(player.transform);
        }
    }

    // 데미지를 입는 경우
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
