using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    
    //public float rotationInput;
    public Vector3 aimForwardVector;
    public NetworkBool isJumpPressed;
    public PoseEstimationData poseData;

}

public struct PoseEstimationData : INetworkStruct
{
    public Vector3 Joint0;
    public Vector3 Joint1;
    public Vector3 Joint2;
    public Vector3 Joint3;
    public Vector3 Joint4;
    public Vector3 Joint5;
    public Vector3 Joint6;
    public Vector3 Joint7;
    public Vector3 Joint8;
    public Vector3 Joint9;
    public Vector3 Joint10;
    public Vector3 Joint11;
    public Vector3 Joint12;
    public Vector3 Joint13;
    public Vector3 Joint14;
    public Vector3 Joint15;
    public Vector3 Joint16;

}
