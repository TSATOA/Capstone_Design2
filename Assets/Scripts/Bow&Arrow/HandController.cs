using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    public GameObject arrowPrefab;      // 생성할 화살 프리팹
    public GameObject bowObject;        // 플레이어의 활 오브젝트
    public Transform handTransform;     // 화살을 부착할 위치
    public Transform stringTransform;   // 활시위의 위치
    public GameObject idleCamera;
    public GameObject aimCamera;

    private GameObject player;

    public float arrowPower = 10.0f;

    private bool isArrowEquipped = false;
    private bool isArrowReload = false;
    private GameObject playerArrow;
    private Transform bowHead;
    private float fireDistance = 1.0f;
    private Vector3 originalStringPosition;
    private Quaternion originalStringRotation;
    private Transform arrowHead;
    private Transform arrowTail;

    [SerializeField] private Image barImage;
    public GameObject Crosshair;

    private void OnTriggerEnter(Collider other)
    {
        // 손이 Quiver의 ArrowSelect 부분과 접촉하였을 경우
        if (other.gameObject.name == "QuiverTop")
        {
            if (!isArrowEquipped)
            {
                isArrowEquipped = true;

                // 화살 생성
                playerArrow = Instantiate(arrowPrefab, handTransform.position, handTransform.rotation);
                if (playerArrow == null) return;
                playerArrow.tag = "Arrow_Player";
                playerArrow.layer = 9; // CollisionWithEnemy layer

                // 화살촉과 화살깃 부분의 Transform 저장
                arrowHead = playerArrow.transform.Find("ArrowHead");
                arrowTail = playerArrow.transform.Find("ArrowTail");

                // 화살을 손의 ArrowAttach 부분의 child로 설정
                playerArrow.transform.parent = handTransform;

                // 화살의 위치, 회전 세부 조정
                playerArrow.transform.localPosition = Vector3.zero;
                playerArrow.transform.localRotation = Quaternion.Euler(-41.131f, 0 , 180);
            }
        }
        // 화살을 손에 든채로 활시위와 접촉하였을 경우
        else if (other.gameObject.name == "BowString")
        {
            if (isArrowEquipped && !isArrowReload)
            {
                isArrowReload = true;

                // Player Status 변경
                player.GetComponent<PlayerStatus>().ChangePlayerPoseStatus(isArrowReload);

                // 활시위와 화살을 손에 부착
                other.transform.parent = handTransform;
                playerArrow.transform.parent = stringTransform;

                // 화살의 위치, 회전 세부 조정
                playerArrow.transform.localPosition = Vector3.zero;
                // playerArrow.transform.localRotation = Quaternion.Euler(95,-50,41);

                // Aim Camera로 전환
                idleCamera.SetActive(false);
                aimCamera.SetActive(true);
            }
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bowHead = bowObject.transform.Find("Armature/Main/Bone");
        originalStringPosition = stringTransform.localPosition;
        originalStringRotation = stringTransform.localRotation;
    }

    private void Update()
    {
        if (isArrowReload)
        {
            Crosshair.SetActive(true);
            // 장전된 화살이 활을 바라보도록 조정
            playerArrow.transform.LookAt(bowObject.transform);
            playerArrow.transform.Rotate(90, 0, 0);
            // 화살을 당긴 거리 (필요하다면 public 전역 변수로 선언 가능)
            float distance = Vector3.Distance(bowHead.position, stringTransform.position);
            barImage.fillAmount = distance;

            if (distance > fireDistance)
            {
                // 화살을 시위에서 제거하고 다음 화살을 발사 가능한 상태로 전환
                playerArrow.transform.parent = null;
                isArrowEquipped = false;
                isArrowReload = false;

                // 손에 부착된 활시위를 제거
                stringTransform.parent = bowHead.parent;
                stringTransform.localPosition = originalStringPosition;
                stringTransform.localRotation = originalStringRotation;

                // 화살 방향 계산
                Vector3 direction = CalculateDirection(arrowTail, arrowHead);

                // 활시위가 일정이상 당겨지면 화살을 발사
                playerArrow.GetComponent<Arrow>().ReleaseArrow(arrowPower, direction, gameObject);

                // Player Status 변경
                player.GetComponent<PlayerStatus>().ChangePlayerPoseStatus(isArrowReload);

                // idle Camera로 전환
                idleCamera.SetActive(true);
                aimCamera.SetActive(false);
                Crosshair.SetActive(false);
            }
        }
    }
    Vector3 CalculateDirection(Transform from, Transform to)
    {
        Vector3 direction = to.position - from.position;

        direction.Normalize();

        return direction;
    }
}
