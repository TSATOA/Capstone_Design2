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
    CharacterControl characterControl;
    Camera localCamera;
    Vector3[] threeDJoints;

    private void Awake(){
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        characterControl = GetComponentInChildren<CharacterControl>();
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
            
            if(networkInputData.goodEstimate){
                threeDJoints = GetJointsList(networkInputData);
                networkCharacterControllerPrototypeCustom.JointApplication(threeDJoints);
                //characterControl.Draw3DPoints(threeDJoints);
                //characterControl.scaleTranslateJoints(threeDJoints);
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
    Vector3[] GetJointsList(NetworkInputData networkInputData) {{
        Vector3[] result = new Vector3[17];
        result[0]=networkInputData.poseData.Joint0;
        result[1]=networkInputData.poseData.Joint1;
        result[2]=networkInputData.poseData.Joint2;
        result[3]=networkInputData.poseData.Joint3;
        result[4]=networkInputData.poseData.Joint4;
        result[5]=networkInputData.poseData.Joint5;
        result[6]=networkInputData.poseData.Joint6;
        result[7]=networkInputData.poseData.Joint7;
        result[8]=networkInputData.poseData.Joint8;
        result[9]=networkInputData.poseData.Joint9;
        result[10]=networkInputData.poseData.Joint10;
        result[11]=networkInputData.poseData.Joint11;
        result[12]=networkInputData.poseData.Joint12;
        result[13]=networkInputData.poseData.Joint13;
        result[14]=networkInputData.poseData.Joint14;
        result[15]=networkInputData.poseData.Joint15;
        result[16]=networkInputData.poseData.Joint16;
        return result;
    }

    }

}
