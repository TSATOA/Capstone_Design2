using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyAI : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform handTransform;
    public Animator animator;
    public GameObject bloodPrefab;

    private GameObject enemyArrow;
    private Transform arrowHead;
    private Transform arrowTail;
    private GameObject player;
    private PlayerStatus playerStatus;
    private GameObject bloodInstance;

    private Transform target;
    
    // 정면 벡터
    private Vector3 forwardVector = new Vector3(1, 0, 0);

    // 플레이어가 조준하고 있는 동안 AI가 회피 동작을 할 확률
    public float evadeChance = 1.0f; // 회피 동작 성공 확률 (0 ~ 1)
    private bool isEvadeDone = false; // 플레이어가 조준하는 동안 구르기를 수행할 것인지 결정하였는가? (구르기는 조준당 한번만 수행)

    // 적 NPC가 구르기를 수행한 횟수
    private int leftEvade = 0;
    private int rightEvade = 0;
    private int maxEvade = 5; // 한쪽 방향으로 최대 구르기 가능 횟수

    public float health;
    public float arrowPower = 10.0f;

    // 각 부위를 향해 화살을 발사할 확률
    private static class targetProb
    {
        public const float Head = 0.0f;
        public const float Body = 1.0f;
        public const float Arm = 0.0f;
        public const float Leg = 0.0f;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStatus = player.GetComponent<PlayerStatus>();

        target = GameObject.FindGameObjectWithTag("Target").transform;
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
            isEvadeDone = true;

            // 구르기 성공 확률에 따라 구르기 실행
            if (Random.value < evadeChance)
            {
                // 구르기 전에 정면 바라보기(좌우로만 구르기 위해)
                transform.rotation = Quaternion.LookRotation(forwardVector);

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
        }
        // 플레이어가 조준을 풀면 다시 구르기 가능 상태로 변경
       else if (isEvadeDone && !playerStatus.IsPlayerAiming())
        {
            isEvadeDone = false;
        }
    }

    // 화살통에서 화살이 꺼내는 애니메이션이 재생되는 순간 손에 화살 생성하여 부착
    public void GrabArrow()
    {
        // 화살 생성
        enemyArrow = Instantiate(arrowPrefab, handTransform.position, handTransform.rotation);
        if (enemyArrow == null) return;
        enemyArrow.tag = "Arrow_Enemy";
        enemyArrow.layer = 8; // CollisionWithPlayer Layer

        // 화살촉과 화살깃 부분의 Transform 저장
        arrowHead = enemyArrow.transform.Find("ArrowHead");
        arrowTail = enemyArrow.transform.Find("ArrowTail");

        // 화살을 손의 ArrowAttach 부분의 child로 설정
        enemyArrow.transform.parent = handTransform;

        // 플레이어의 어디를 조준할지 결정
        changeTarget();

        // 화살의 위치, 회전 세부 조정
        //enemyArrow.transform.localPosition = Vector3.zero;
        //enemyArrow.transform.localRotation = Quaternion.identity; 
    }

    // 활을 당겨서 발사하는 순간 화살을 발사하는 함수 실행
    public void FireArrow_Enemy()
    {
        // 화살을 시위에서 제거하고 다음 화살을 발사 가능한 상태로 전환
        enemyArrow.transform.parent = null;

        // 화살 방향 계산
        Vector3 direction = CalculateDirection(arrowTail, target);

        Debug.DrawRay(arrowTail.position, direction, Color.red, 2.0f);

        // 화살을 발사
        enemyArrow.GetComponent<Arrow>().ReleaseArrow(arrowPower, direction, gameObject);
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
    public void takeDamge(float damage, Transform hitPos, Quaternion hitDir)
    {
        health -= damage;

        bloodInstance = Instantiate(bloodPrefab, hitPos.position, hitDir);

        if (health <= 0)
        {
            animator.SetTrigger("Death");
            DisableColliders(gameObject);
        }
        else
        {
            animator.SetTrigger("Hit_Small");
        }

        Destroy(bloodInstance, 2.0f);
    }

    Vector3 CalculateDirection(Transform from, Transform to)
    {
        Vector3 direction = to.position - from.position;

        direction.Normalize();

        return direction;
    }

    private void changeTarget() { 
        float prob = Random.value;

        if (!target.CompareTag("Target"))
            target = target.parent;

        if (prob < targetProb.Head)
        {
            target = target.Find("Target_Head");
            return;
        }
        prob -= targetProb.Head;
        
        if (prob < targetProb.Body)
        {
            target = target.Find("Target_Body");
            return;
        }
        prob -= targetProb.Body;

        if (prob < targetProb.Arm)
        {
            target = target.Find("Target_Arm");
            return;
        }
        prob -= targetProb.Arm;

        if (prob < targetProb.Leg)
        {
            target = target.Find("Target_Leg");
            return;
        }
    }

    // 해당 캐릭터가 죽을 경우 자식 게임 오브젝트의 Collider를 모두 비활성화하여 더이상 데미지를 입지 못하게 한다.
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
