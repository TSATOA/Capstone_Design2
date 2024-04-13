using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    // 네트워크를 통해 동기화된 입력을 받아 캐릭터의 실제 움직임을 처리
    // FixedUpdateNetwork() 메서드에서 네트워크 입력 데이터를 사용하여 캐릭터의 방향 전환, 이동 및 점프를 구현
    // NetworkCharacterControllerPrototypeCustom에서 정의된 물리적 움직임과 동기화 로직을 사용하여 실제 게임 월드에서 캐릭터의 움직임을 제어
    
    //Vector2 viewInput;
    //float cameraRotationX = 0;
    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;

    private void Awake(){
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
    }
    void Start()
    {
        
    }

    /*
    이 부분은 localCameraHandler를 추가하여 수정하면서 삭제
    void Update()
    {
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX,-90,90);
        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX,0,0);
    }
    */
    public override void FixedUpdateNetwork()
    {
        //FixedUpdateNetwork를 통해 일관된 동기화 보장
        //Update는 매 피레임, FixedUpdate는일정 시간마다
        if(GetInput(out NetworkInputData networkInputData)){
            //Rotation the view
            //networkCharacterControllerPrototypeCustom.Rotate(networkInputData.rotationInput);

            //client aim vector rotate
            transform.forward = networkInputData.aimForwardVector;
            //x축 tilt 고정
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0,rotation.eulerAngles.y,rotation.eulerAngles.z);
            transform.rotation = rotation;

            //Move
            Vector3 moveDirection = transform.forward*networkInputData.movementInput.y + transform.right*networkInputData.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(moveDirection);
            //Jump
            if(networkInputData.isJumpPressed){
                networkCharacterControllerPrototypeCustom.Jump();
            }

            //이건 걍 맵에서 떨어졌을때 대비
            CheckFallRespawn();
        }
    }
    void CheckFallRespawn(){
        if(transform.position.y < -12)
            transform.position = Utils.GetRandomSpawnPoint();
    }
    /*
    public void SetViewInputVector(Vector2 viewInput){
        this.viewInput = viewInput;
    }
    */
}
