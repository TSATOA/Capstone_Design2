using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public GameObject arrowPrefab;      // ������ ȭ�� ������
    public GameObject bowObject;        // �÷��̾��� Ȱ ������Ʈ
    public Transform handTransform;     // ȭ���� ������ ��ġ
    public Transform stringTransform;   // Ȱ������ ��ġ
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
        // ���� Quiver�� ArrowSelect �κа� �����Ͽ��� ���
        if (other.gameObject.name == "QuiverTop")
        {
            if (!isArrowEquipped)
            {
                isArrowEquipped = true;

                // ȭ�� ����
                playerArrow = Instantiate(arrowPrefab, handTransform.position, handTransform.rotation);

                // ȭ���� ���� ArrowAttach �κ��� child�� ����
                playerArrow.transform.parent = handTransform;

                // ȭ���� ��ġ, ȸ�� ���� ����
                //newArrow.transform.localPosition = Vector3.zero;
                //newArrow.transform.localRotation = Quaternion.identity; 
            }
        }
        // ȭ���� �տ� ��ä�� Ȱ������ �����Ͽ��� ���
        else if (other.gameObject.name == "BowString")
        {
            if (isArrowEquipped && !isArrowReload)
            {
                isArrowReload = true;

                // Ȱ������ ȭ���� �տ� ����
                other.transform.parent = handTransform;
                playerArrow.transform.parent = stringTransform;

                // ȭ���� ��ġ, ȸ�� ���� ����
                playerArrow.transform.localPosition = new Vector3(-0.004f, 0.001f, 0.001f);
                playerArrow.transform.localRotation = Quaternion.Euler(49.527f, 73.301f, -33.277f);

                // Aim Camera�� ��ȯ
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
            // ȭ���� ��� �Ÿ� (�ʿ��ϴٸ� public ���� ������ ���� ����)
            float distance = Vector3.Distance(bowHead.position, stringTransform.position);

            if (distance > fireDistance)
            {
                // ȭ���� �������� �����ϰ� ���� ȭ���� �߻� ������ ���·� ��ȯ
                playerArrow.transform.parent = null;
                isArrowEquipped = false;
                isArrowReload = false;

                // �տ� ������ Ȱ������ ����
                stringTransform.parent = bowHead.parent;
                stringTransform.localPosition = originalStringPosition;
                stringTransform.localRotation = originalStringRotation;

                // Ȱ������ �����̻� ������� ȭ���� �߻�
                playerArrow.GetComponent<Arrow>().ReleaseArrow(10.0f, playerArrow.transform.forward);

                // idle Camera�� ��ȯ
                idleCamera.SetActive(true);
                aimCamera.SetActive(false);
            }
        }
    }
}
