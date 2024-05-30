using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Windows.WebCam;

public class CharacterInputHandler : MonoBehaviour
{
    // �÷��̾��� �Է��� ó���ϰ� �� �Է��� ��Ʈ��ũ�� ���� �ٸ� �÷��̾�� ����ȭ�ϱ� ���� �����͸� �غ�
    // CharacterInputHandler�� �÷��̾�κ��� �Է��� �޾� LocalCameraHandler�� NetworkInputData�� ����
    // ����ڷκ��� �̵�, ����, ī�޶� ȸ�� ���Է����� ����
    // ������ ���ÿ� LocalCameraHandler�� �ٸ� ���� ���� ��ҷ� �����Ͽ� ���� �� ĳ������ �����Ӱ� ī�޶� ����
    // GetNetworkInput() �޼��带 ���� ��Ʈ��ũ�� ���� ����ȭ�� �Է� ������(NetworkInputData)�� �����ϰ� �ʱ�ȭ

    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;

    //other components
    //CharacterMovementHandler characterMovementHandler;
    LocalCameraHandler localCameraHandler;

    CharacterControl characterControl;
    private PoseEstimator poseEstimator;
    private PoseEstimationData poseEstimationData;
    // private bool goodEs;

    WebCamTexture webcamTexture;

    private void Awake(){
        //characterMovementHandler = GetComponent<CharacterMovementHandler>();
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // WebCamDevice[] devices = WebCamTexture.devices;
        
        /******************************************************************
            WebCamTexture�� �� �� �̻��� ��ũ��Ʈ���� instantiate �� �� �� �����ϴ�.
            PoseEstimator ��ũ��Ʈ�� MonoBehavior�� ����ϵ��� ���������� ��ũ��Ʈ��
            ���ư��� ���� poseEstimator.GetNetworkPoseData()�� �ֽ� ������ �ҷ����� �˴ϴ�.
        ******************************************************************/

        // webcamTexture = new WebCamTexture(devices[0].name, 640, 360, 60);

        // webcamTexture.Play();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterControl = GetComponentInChildren<CharacterControl>();
        // pose estimator 
        poseEstimator = GetComponentInChildren<PoseEstimator>();

    }

    // Update is called once per frame
    void Update()
    {
        //View Input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y")*-1;

        //LocalCameraHandler�� ���� Update�� rotation�� �ƴ϶� ��Ʈ��ũ�� aim�� ����
        //characterMovementHandler.SetViewInputVector(viewInputVector);

        //Move Input
        //�� ������ ���� sync�� ������� �Ʒ�ó�� ���� ����
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        //Shooting, jump���� Update�� GetNetworkInput�� sync �ȸ¾� �Ʒ�ó�� �Ұ���
        //isJumpButtonPressed = Input.GetButtonDown("Jump");
        //�̷��� �����ؾ���
        if(Input.GetButtonDown("Jump"))
            isJumpButtonPressed = true;

        //Set View
        localCameraHandler.SetViewInputVector(viewInputVector);
        if(poseEstimator!=null){
            poseEstimationData = poseEstimator.GetNetworkPoseData(); // good estimation���� Ȯ���� �ʿ� X
        }
        
    }

    public NetworkInputData GetNetworkInput(){
        NetworkInputData networkInputData = new NetworkInputData();
        //View data
        //networkInputData.rotationInput = viewInputVector.x;

        //Aim data
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;
        //Move data
        networkInputData.movementInput = moveInputVector;

        networkInputData.isJumpPressed = isJumpButtonPressed;
        networkInputData.poseData = poseEstimationData;
        // networkInputData.goodEstimate = goodEs; // good estimation���� Ȯ���� �ʿ� X
        isJumpButtonPressed = false;

        return networkInputData;
    }


    public void SetPoseEstimator(PoseEstimator estimator) {
        poseEstimator = estimator;
    }

}
