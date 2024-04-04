using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CharacterInputHandler : MonoBehaviour
{
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
