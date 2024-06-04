using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    // ��Ʈ��ũ�� ���� ����ȭ�� �Է��� �޾� ĳ������ ���� �������� ó��
    // FixedUpdateNetwork() �޼��忡�� ��Ʈ��ũ �Է� �����͸� ����Ͽ� ĳ������ ���� ��ȯ, �̵� �� ������ ����
    // NetworkCharacterControllerPrototypeCustom���� ���ǵ� ������ �����Ӱ� ����ȭ ������ ����Ͽ� ���� ���� ���忡�� ĳ������ �������� ����
    
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
    �� �κ��� localCameraHandler�� �߰��Ͽ� �����ϸ鼭 ����
    void Update()
    {
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX,-90,90);
        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX,0,0);
    }
    */
    public override void FixedUpdateNetwork()
    {
        //FixedUpdateNetwork�� ���� �ϰ��� ����ȭ ����
        //Update�� �� �Ƿ���, FixedUpdate������ �ð�����
        if(GetInput(out NetworkInputData networkInputData)){
            //Rotation the view
            //networkCharacterControllerPrototypeCustom.Rotate(networkInputData.rotationInput);

            //client aim vector rotate
            transform.forward = networkInputData.aimForwardVector;
            //x�� tilt ����
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
            
            threeDJoints = GetJointsList(networkInputData);
            //임시networkCharacterControllerPrototypeCustom.JointApplication(threeDJoints);
            //characterControl.Draw3DPoints(threeDJoints);
            //characterControl.scaleTranslateJoints(threeDJoints);
    
            //�̰� �� �ʿ��� ���������� ���
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
