using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public GameObject arrowPrefab;      // 생성할 화살 프리팹
    public GameObject bowObject;        // 플레이어의 활 오브젝트
    public Transform handTransform;     // 화살을 부착할 위치
    public Transform stringTransform;   // 활시위의 위치
    public GameObject idleCamera;
    public GameObject aimCamera;

    private bool isArrowEquipped = false;
    private bool isArrowReload = false;
    private GameObject playerArrow;
    private Transform bowHead;
    private float fireDistance = 2.0f;
    private Vector3 originalStringPosition;
    private Quaternion originalStringRotation;

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

                // 화살을 손의 ArrowAttach 부분의 child로 설정
                playerArrow.transform.parent = handTransform;

                // 화살의 위치, 회전 세부 조정
                //newArrow.transform.localPosition = Vector3.zero;
                //newArrow.transform.localRotation = Quaternion.identity; 
            }
        }
        // 화살을 손에 든채로 활시위와 접촉하였을 경우
        else if (other.gameObject.name == "BowString")
        {
            if (isArrowEquipped && !isArrowReload)
            {
                isArrowReload = true;

                // 활시위와 화살을 손에 부착
                other.transform.parent = handTransform;
                playerArrow.transform.parent = stringTransform;

                // 화살의 위치, 회전 세부 조정
                playerArrow.transform.localPosition = new Vector3(-0.004f, 0.001f, 0.001f);
                playerArrow.transform.localRotation = Quaternion.Euler(49.527f, 73.301f, -33.277f);

                // Aim Camera로 전환
                idleCamera.SetActive(false);
                aimCamera.SetActive(true);
            }
        }
    }

    private void Start()
    {
        bowHead = bowObject.transform.Find("Armature/Main/Bone");
        originalStringPosition = stringTransform.localPosition;
        originalStringRotation = stringTransform.localRotation;
    }

    private void Update()
    {
        if (isArrowReload)
        {
            // 화살을 당긴 거리 (필요하다면 public 전역 변수로 선언 가능)
            float distance = Vector3.Distance(bowHead.position, stringTransform.position);

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

                // 활시위가 일정이상 당겨지면 화살을 발사
                playerArrow.GetComponent<Arrow>().ReleaseArrow(10.0f, playerArrow.transform.forward);

                // idle Camera로 전환
                idleCamera.SetActive(true);
                aimCamera.SetActive(false);
            }
        }
    }
}
