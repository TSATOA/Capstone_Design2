using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
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
