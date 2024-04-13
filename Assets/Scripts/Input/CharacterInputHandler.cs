using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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

    private void Awake(){
        //characterMovementHandler = GetComponent<CharacterMovementHandler>();
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        isJumpButtonPressed = false;

        return networkInputData;
    }
}
