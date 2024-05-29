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

    // 화살통에서 화살이 꺼내는 애니메이션이 재생되는 순간 손에 화살 생성하여 부착
    void GrabArrow()
    {
        // 화살 생성
        enemyArrow = Instantiate(arrowPrefab, handTransform.position, handTransform.rotation);

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

        // 화살을 발사
        enemyArrow.GetComponent<Arrow>().ReleaseArrow(10.0f);
    }

    // 데미지를 입는 경우
    void takeDamge(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            animator.SetTrigger("Death");
        }
    }
}
