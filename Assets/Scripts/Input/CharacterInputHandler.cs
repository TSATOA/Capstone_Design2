using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Windows.WebCam;

public class CharacterInputHandler : MonoBehaviour
{
    // 플레이어의 입력을 처리하고 그 입력을 네트워크를 통해 다른 플레이어와 동기화하기 위한 데이터를 준비
    // CharacterInputHandler는 플레이어로부터 입력을 받아 LocalCameraHandler와 NetworkInputData로 전달
    // 사용자로부터 이동, 점프, 카메라 회전 등입력정보 수집
    // 수집과 동시에 LocalCameraHandler와 다른 게임 구성 요소로 전달하여 게임 내 캐릭터의 움직임과 카메라 조작
    // GetNetworkInput() 메서드를 통해 네트워크를 통해 동기화할 입력 데이터(NetworkInputData)를 생성하고 초기화

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
            WebCamTexture는 두 개 이상의 스크립트에서 instantiate 할 수 가 없습니다.
            PoseEstimator 스크립트를 MonoBehavior를 상속하도록 수정했으니 스크립트가
            돌아가는 동안 poseEstimator.GetNetworkPoseData()로 최신 정보를 불러오면 됩니다.
        ******************************************************************/

        // webcamTexture = new WebCamTexture(devices[0].name, 640, 360, 60);

        // webcamTexture.Play();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterControl = GetComponentInChildren<CharacterControl>();

    }

    // Update is called once per frame
    void Update()
    {
        //View Input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y")*-1;

        //LocalCameraHandler를 통해 Update의 rotation이 아니라 네트워크의 aim과 맞춤
        //characterMovementHandler.SetViewInputVector(viewInputVector);

        //Move Input
        //꾹 누르는 것은 sync와 상관없이 아래처럼 구현 가능
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        //Shooting, jump등은 Update와 GetNetworkInput의 sync 안맞아 아래처럼 불가능
        //isJumpButtonPressed = Input.GetButtonDown("Jump");
        //이렇게 구현해야함
        if(Input.GetButtonDown("Jump"))
            isJumpButtonPressed = true;

        //Set View
        localCameraHandler.SetViewInputVector(viewInputVector);
        if(poseEstimator!=null){
            poseEstimationData = poseEstimator.GetNetworkPoseData(); // good estimation인지 확인할 필요 X
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
        // networkInputData.goodEstimate = goodEs; // good estimation인지 확인할 필요 X
        isJumpButtonPressed = false;

        return networkInputData;
    }


    public void SetPoseEstimator(PoseEstimator estimator) {
        poseEstimator = estimator;
    }

}
