using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Animations.Rigging;
using PoseInformation;

public class CharacterControl : MonoBehaviour
{
    // Game Objects
    private List<GameObject> threeDPoints;
    public bool visualizeKeypoints = false;
    public string nameOfJointsGroup = "pose";

    // IK Control
    private Rig core;
    private Rig arms;
    private Rig look;

    // For joint control
    public Transform characterRoot;
    public Transform spineRoot;
    public Transform spineMiddle;
    public Transform spineTip;
    public Transform neck;
    public Transform nose;
    public Transform leftShoulder;
    public Transform leftArm;
    public Transform leftForeArm;
    public Transform leftWrist;
    public Transform leftWristTarget;
    public Transform rightShoulder;
    public Transform rightArm;
    public Transform rightForeArm;
    public Transform rightWrist;
    public Transform rightWristTarget;

    void Start()
    {
        // IK setup
        init3DKeypoints(nameOfJointsGroup);
        SetupRig();

        PoseFormat.BoneDistances[PoseFormat.Bone.RootToRhip] = 0.0f;
        PoseFormat.BoneDistances[PoseFormat.Bone.RhipToRknee] = 0.0f;
        PoseFormat.BoneDistances[PoseFormat.Bone.RkneeToRankle] = 0.0f;
        PoseFormat.BoneDistances[PoseFormat.Bone.RootToLhip] = 0.0f;
        PoseFormat.BoneDistances[PoseFormat.Bone.LhipToLknee] = 0.0f;
        PoseFormat.BoneDistances[PoseFormat.Bone.LkneeToLankle] = 0.0f;
        PoseFormat.BoneDistances[PoseFormat.Bone.RootToBelly] = distAtoB(characterRoot.position, spineMiddle.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.BellyToNeck] = distAtoB(spineMiddle.position, neck.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.NeckToNose] = distAtoB(neck.position, nose.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.NoseToHead] = 0.8f * PoseFormat.BoneDistances[PoseFormat.Bone.NeckToNose];
        PoseFormat.BoneDistances[PoseFormat.Bone.NeckToLshoulder] = distAtoB(neck.position, leftArm.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.LshoulderToLelbow] = distAtoB(leftArm.position, leftForeArm.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.LelbowToLwrist] = distAtoB(leftForeArm.position, leftWrist.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.NeckToRshoulder] = distAtoB(neck.position, rightArm.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.RshoulderToRelbow] = distAtoB(rightArm.position, rightForeArm.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.RelbowToRwrist] = distAtoB(rightForeArm.position, rightWrist.position);
    }

    void Update()
    {
        Vector3[] modelOutput;

        modelOutput = GetComponent<PoseEstimator>().getThreeDJoints();

        Vector3[] scaledOutput = scaleOutputJoints(modelOutput);
        Vector3[] moreTargets = getMoreTargets(scaledOutput);

        Draw3DJoints(scaledOutput, moreTargets, visualizeKeypoints);
    }

    public void init3DKeypoints(string name)
    {
        threeDPoints = new List<GameObject>();
        GameObject root = new GameObject(name);
        root.transform.SetParent(characterRoot);
        root.transform.localPosition = Vector3.zero;
        root.transform.Rotate(0, 0, 0);

        int numBodyJoints, numMoreTargets, totalLength;
        numBodyJoints = Enum.GetValues(typeof(PoseFormat.Keypoint)).Length;
        numMoreTargets = Enum.GetValues(typeof(PoseFormat.MoreTargetKeypoint)).Length;

        totalLength = numBodyJoints + numMoreTargets;

        for (int i = 0; i < totalLength; i++)
        {

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = String.Format("joint{0}", i);

            sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            sphere.transform.localPosition = new Vector3(0, 0, 0);

            sphere.transform.SetParent(root.transform, false);

            threeDPoints.Add(sphere);

        }
    }

    public Vector3[] scaleOutputJoints(Vector3[] modelOutput)
    {

        var boneArray = Enum.GetValues(typeof(PoseFormat.Bone));
        int numBodyJoints = Enum.GetValues(typeof(PoseFormat.Keypoint)).Length;

        Vector3[] scaledJoints = new Vector3[numBodyJoints];

        foreach(PoseFormat.Bone bone in boneArray) {
            int joint1, joint2;
            joint1 = (int)PoseFormat.BoneToKeypointPair[bone].Item1;
            joint2 = (int)PoseFormat.BoneToKeypointPair[bone].Item2;

            // Pose estimation 결과의 root position에서 시작해
            // 오른쪽 하반신, 왼쪽 하반신, 허리->머리,
            // 왼쪽 상반신, 오른쪽 상반신 순서로 다음 관절 방향 계산
            Vector3 aToB = vectorFromAtoB(modelOutput[joint1], modelOutput[joint2]).normalized;
            // 해당 방향으로 캐릭터의 관절의 길이만큼 연장하여 keypoint 위치 계산
            scaledJoints[joint2] = scaledJoints[joint1] + aToB * PoseFormat.BoneDistances[bone];
        }

        // Root position은 항상 (0,0,0)
        scaledJoints[0].x = 0;
        scaledJoints[0].y = 0;
        scaledJoints[0].z = 0;

        // Neck joint의 위치 조정
        int bellyIdx, neckIdx;
        bellyIdx = (int)PoseFormat.Keypoint.Belly;
        neckIdx = (int)PoseFormat.Keypoint.Neck;

        Vector3 bellyToNeck = vectorFromAtoB(scaledJoints[bellyIdx], scaledJoints[neckIdx]);
        scaledJoints[(int)PoseFormat.Keypoint.Neck] = scaledJoints[bellyIdx] + 0.8f * bellyToNeck;

        return scaledJoints;
    }

    public Vector3[] getMoreTargets(Vector3[] bodyJoints)
    {
        var targetKeypointArray = Enum.GetValues(typeof(PoseFormat.MoreTargetKeypoint));
        int numMoreTargets = targetKeypointArray.Length;

        Vector3[] bodyAndHandTargets = new Vector3[numMoreTargets];

        // 왼쪽 손목이 향할 좌표 계산
        int leftElbowIdx, leftWristIdx;
        Vector3 leftElbowToWrist;

        leftElbowIdx = (int)PoseFormat.Keypoint.Lelbow;
        leftWristIdx = (int)PoseFormat.Keypoint.Lwrist;
        leftElbowToWrist = vectorFromAtoB(bodyJoints[leftElbowIdx], bodyJoints[leftWristIdx]);

        Vector3 leftHandPoint = bodyJoints[leftElbowIdx] + 1.1f * leftElbowToWrist;

        // 오른쪽 손목이 향할 좌표 계산
        int rightElbowIdx, rightWristIdx;
        Vector3 rightElbowToWrist;

        rightElbowIdx = (int)PoseFormat.Keypoint.Relbow;
        rightWristIdx = (int)PoseFormat.Keypoint.Rwrist;
        rightElbowToWrist = vectorFromAtoB(bodyJoints[rightElbowIdx], bodyJoints[rightWristIdx]);

        Vector3 rightHandPoint = bodyJoints[rightElbowIdx] + 1.1f * rightElbowToWrist;

        // 몸통이 바라보는 방향의 좌표 계산
        int leftShoulderIdx, rightShoulderIdx, bellyIdx, neckIdx;

        leftShoulderIdx = (int)PoseFormat.Keypoint.Lshoulder;
        rightShoulderIdx = (int)PoseFormat.Keypoint.Rshoulder;
        bellyIdx = (int)PoseFormat.Keypoint.Belly;
        neckIdx = (int)PoseFormat.Keypoint.Neck;

        var forwardNormal = calculateForwardDirection(
            bodyJoints[leftShoulderIdx],
            bodyJoints[rightShoulderIdx],
            bodyJoints[bellyIdx]
        ).normalized;

        Vector3 forwardPoint = bodyJoints[neckIdx] + forwardNormal * distAtoB(bodyJoints[leftShoulderIdx], bodyJoints[rightShoulderIdx]);

        bodyAndHandTargets[(int)PoseFormat.MoreTargetKeypoint.LhandPoint] = leftHandPoint;
        bodyAndHandTargets[(int)PoseFormat.MoreTargetKeypoint.RhandPoint] = rightHandPoint;
        bodyAndHandTargets[(int)PoseFormat.MoreTargetKeypoint.BodyFacing] = forwardPoint;

        return bodyAndHandTargets;
    }

    public void Draw3DJoints(Vector3[] bodyJoints, Vector3[] moreTargets, bool visualizeKeypoints)
    {
        for (int idx = 0; idx < bodyJoints.Length; idx++)
        {
            GameObject bodyJoint = threeDPoints[idx];
            bodyJoint.transform.localPosition = bodyJoints[idx];
            bodyJoint.SetActive(visualizeKeypoints);
        }
        for (int idx = 0; idx < moreTargets.Length; idx++)
        {
            GameObject bodyJoint = threeDPoints[idx + bodyJoints.Length];
            bodyJoint.transform.localPosition = moreTargets[idx];
            bodyJoint.SetActive(visualizeKeypoints);
        }
    }

    void SetupRig()
    {
        RigBuilder rigBuilder = gameObject.AddComponent<RigBuilder>();
        GameObject rig_core = new GameObject("Core Rig");
        GameObject rig_arms = new GameObject("Arm Rig");
        GameObject rig_look = new GameObject("Look Rig");
        core = rig_core.AddComponent<Rig>();
        arms = rig_arms.AddComponent<Rig>();
        look = rig_look.AddComponent<Rig>();
        core.weight = 1.0f;
        arms.weight = 1.0f;
        look.weight = 1.0f;

        rig_core.transform.SetParent(gameObject.transform);
        rig_arms.transform.SetParent(gameObject.transform);
        rig_look.transform.SetParent(gameObject.transform);

        rigBuilder.layers.Add(new RigLayer(core.GetComponent<Rig>(), true));
        rigBuilder.layers.Add(new RigLayer(arms.GetComponent<Rig>(), true));
        rigBuilder.layers.Add(new RigLayer(look.GetComponent<Rig>(), true));

        int numBodyJoints = Enum.GetValues(typeof(PoseFormat.Keypoint)).Length;
        int numMoreTargets = Enum.GetValues(typeof(PoseFormat.MoreTargetKeypoint)).Length;

        // Setup Core Rig
        GameObject spineControl = new GameObject("spineControl");
        GameObject lookAt = new GameObject("lookAt");
        GameObject neckControl = new GameObject("neckControl");

        spineControl.transform.SetParent(rig_core.transform);
        neckControl.transform.SetParent(spineControl.transform);

        // Main Spine Constraints
        var spine_chainIK = spineControl.AddComponent<ChainIKConstraint>();
        var spine_multiAim = spineControl.AddComponent<MultiAimConstraint>();
        // var spine_multiRotation = spineControl.AddComponent<TwistChainConstraint>();

        // Chain IK
        spine_chainIK.data.root = spineRoot;
        spine_chainIK.data.tip = spineTip;
        spine_chainIK.data.target = threeDPoints[(int)PoseFormat.Keypoint.Neck].transform;
        spine_chainIK.data.maxIterations = 15;
        spine_chainIK.data.tolerance = 0.0001f;
        spine_chainIK.data.chainRotationWeight = 1.0f;
        spine_chainIK.data.tipRotationWeight = 0.0f;

        // Multi-Aim
        spine_multiAim.data.constrainedObject = threeDPoints[8].transform;

        spine_multiAim.weight = 1.0f;
        var sources1 = spine_multiAim.data.sourceObjects;
        sources1.Add(new WeightedTransform(threeDPoints[numBodyJoints + (int)PoseFormat.MoreTargetKeypoint.BodyFacing].transform, 1.0f));
        spine_multiAim.data.sourceObjects = sources1;

        spine_multiAim.data.aimAxis = MultiAimConstraintData.Axis.Y_NEG;
        spine_multiAim.data.upAxis = MultiAimConstraintData.Axis.Y;
        spine_multiAim.data.maintainOffset = false;
        spine_multiAim.data.constrainedXAxis = true;
        spine_multiAim.data.constrainedYAxis = true;
        spine_multiAim.data.constrainedZAxis = true;
        spine_multiAim.data.limits = new Vector2(-5, 5);

        // Multi-Rotation

        // spine_multiRotation.weight = 1.0f;

        // spine_multiRotation.data.root = spineRoot;
        // spine_multiRotation.data.tip = spineTip;

        // spine_multiRotation.data.rootTarget = targetThreeDPoints[0].transform;
        // spine_multiRotation.data.tipTarget = targetThreeDPoints[8].transform;

        // Look Rig Setup
        // Look At Contraints
        lookAt.transform.SetParent(rig_look.transform);

        var lookAt_multiAim = lookAt.AddComponent<MultiAimConstraint>();
        lookAt_multiAim.weight = 1.0f;
        lookAt_multiAim.data.constrainedObject = nose;
        lookAt_multiAim.data.aimAxis = MultiAimConstraintData.Axis.Y_NEG;
        lookAt_multiAim.data.upAxis = MultiAimConstraintData.Axis.Y;

        var sources2 = lookAt_multiAim.data.sourceObjects;
        sources2.Add(new WeightedTransform(threeDPoints[(int)PoseFormat.Keypoint.Nose].transform, 1.0f));
        lookAt_multiAim.data.sourceObjects = sources2;

        lookAt_multiAim.data.maintainOffset = false;
        lookAt_multiAim.data.constrainedXAxis = true;
        lookAt_multiAim.data.constrainedYAxis = true;
        lookAt_multiAim.data.constrainedZAxis = true;
        lookAt_multiAim.data.limits = new Vector2(-50, 50);
        lookAt_multiAim.data.offset = new Vector3(0.0f, 0.0f, 0.0f);

        // Neck Contraints
        var neck_twoBone = neckControl.AddComponent<TwoBoneIKConstraint>();

        // neck_twoBone.data.root;
        neck_twoBone.data.root = spineTip;
        neck_twoBone.data.mid = neck;
        neck_twoBone.data.tip = nose;

        neck_twoBone.data.target = threeDPoints[10].transform;
        neck_twoBone.data.hint = threeDPoints[9].transform;

        neck_twoBone.data.targetPositionWeight = 1.0f;
        neck_twoBone.data.targetRotationWeight = 0.0f;
        neck_twoBone.data.hintWeight = 0.0f;

        // Setup Arm Rig
        GameObject leftShoulderControl = new GameObject("leftShoulderControl");
        GameObject leftArmControl = new GameObject("leftArmControl");
        GameObject leftHandControl = new GameObject("leftHandControl");
        GameObject rightShoulderControl = new GameObject("rightShoulderControl");
        GameObject rightArmControl = new GameObject("rightArmControl");
        GameObject rightHandControl = new GameObject("rightHandControl");

        leftShoulderControl.transform.SetParent(rig_arms.transform);
        leftArmControl.transform.SetParent(leftShoulderControl.transform);
        leftHandControl.transform.SetParent(leftArmControl.transform);

        rightShoulderControl.transform.SetParent(rig_arms.transform);
        rightArmControl.transform.SetParent(rightShoulderControl.transform);
        rightHandControl.transform.SetParent(rightArmControl.transform);

        // Setup Left
        // Left Shoulder
        var leftShoulder_twoBone = leftShoulderControl.AddComponent<TwoBoneIKConstraint>();

        leftShoulder_twoBone.data.root = leftShoulder;
        leftShoulder_twoBone.data.mid = leftArm;
        leftShoulder_twoBone.data.tip = leftForeArm;

        leftShoulder_twoBone.data.target = threeDPoints[12].transform;
        leftShoulder_twoBone.data.hint = threeDPoints[11].transform;

        leftShoulder_twoBone.data.targetPositionWeight = 0.2f;
        leftShoulder_twoBone.data.targetRotationWeight = 0.0f;
        leftShoulder_twoBone.data.hintWeight = 0.7f;

        // Left Arm
        var leftArm_twoBone = leftArmControl.AddComponent<TwoBoneIKConstraint>();

        leftArm_twoBone.data.root = leftArm;
        leftArm_twoBone.data.mid = leftForeArm;
        leftArm_twoBone.data.tip = leftWrist;

        leftArm_twoBone.data.target = threeDPoints[13].transform;
        leftArm_twoBone.data.hint = threeDPoints[12].transform;

        leftArm_twoBone.data.targetPositionWeight = 1.0f;
        leftArm_twoBone.data.targetRotationWeight = 0.0f;
        leftArm_twoBone.data.hintWeight = 1.0f;

        // Left Hand
        var leftHand_multiAim = leftHandControl.AddComponent<MultiAimConstraint>();

        leftHand_multiAim.weight = 1.0f;
        leftHand_multiAim.data.constrainedObject = leftWrist;
        leftHand_multiAim.data.aimAxis = MultiAimConstraintData.Axis.X_NEG;
        leftHand_multiAim.data.upAxis = MultiAimConstraintData.Axis.X_NEG;

        var sources3 = leftHand_multiAim.data.sourceObjects;
        sources3.Add(new WeightedTransform(threeDPoints[numBodyJoints + (int)PoseFormat.MoreTargetKeypoint.LhandPoint].transform, 1.0f));
        leftHand_multiAim.data.sourceObjects = sources3;

        leftHand_multiAim.data.maintainOffset = false;
        leftHand_multiAim.data.constrainedXAxis = true;
        leftHand_multiAim.data.constrainedYAxis = true;
        leftHand_multiAim.data.constrainedZAxis = true;
        leftHand_multiAim.data.limits = new Vector2(-10, 10);

        // Right Shoulder
        var rightShoulder_twoBone = rightShoulderControl.AddComponent<TwoBoneIKConstraint>();

        rightShoulder_twoBone.data.root = rightShoulder;
        rightShoulder_twoBone.data.mid = rightArm;
        rightShoulder_twoBone.data.tip = rightForeArm;

        rightShoulder_twoBone.data.target = threeDPoints[15].transform;
        rightShoulder_twoBone.data.hint = threeDPoints[14].transform;

        rightShoulder_twoBone.data.targetPositionWeight = 0.2f;
        rightShoulder_twoBone.data.targetRotationWeight = 0.0f;
        rightShoulder_twoBone.data.hintWeight = 0.7f;

        // Right Arm
        var rightArm_twoBone = rightArmControl.AddComponent<TwoBoneIKConstraint>();

        rightArm_twoBone.data.root = rightArm;
        rightArm_twoBone.data.mid = rightForeArm;
        rightArm_twoBone.data.tip = rightWrist;

        rightArm_twoBone.data.target = threeDPoints[16].transform;
        rightArm_twoBone.data.hint = threeDPoints[15].transform;

        rightArm_twoBone.data.targetPositionWeight = 1.0f;
        rightArm_twoBone.data.targetRotationWeight = 0.0f;
        rightArm_twoBone.data.hintWeight = 1.0f;

        // Right Hand
        var rightHand_multiAim = rightHandControl.AddComponent<MultiAimConstraint>();

        rightHand_multiAim.weight = 1.0f;
        rightHand_multiAim.data.constrainedObject = rightWrist;
        rightHand_multiAim.data.aimAxis = MultiAimConstraintData.Axis.X_NEG;
        rightHand_multiAim.data.upAxis = MultiAimConstraintData.Axis.X_NEG;

        var sources4 = rightHand_multiAim.data.sourceObjects;
        sources4.Add(new WeightedTransform(threeDPoints[numBodyJoints + (int)PoseFormat.MoreTargetKeypoint.RhandPoint].transform, 1.0f));
        rightHand_multiAim.data.sourceObjects = sources3;

        rightHand_multiAim.data.maintainOffset = false;
        rightHand_multiAim.data.constrainedXAxis = true;
        rightHand_multiAim.data.constrainedYAxis = true;
        rightHand_multiAim.data.constrainedZAxis = true;
        rightHand_multiAim.data.limits = new Vector2(-10, 10);

        // Build Rig
        rigBuilder.Build();
    }

    private float distAtoB(Vector3 a, Vector3 b)
    {
        float dist = Vector3.Distance(a, b);
        return dist;
    }

    private Vector3 vectorFromAtoB(Vector3 a, Vector3 b)
    {
        Vector3 a_to_b = b - a;
        return a_to_b;
    }

    Vector3 calculateForwardDirection(Vector3 leftShoulder, Vector3 rightShoulder, Vector3 hip)
    {
        Vector3 hipToLeftShoulder = leftShoulder - hip;
        Vector3 hipToRightShoulder = rightShoulder - hip;

        Vector3 perpendicularVector = Vector3.Cross(hipToRightShoulder, hipToLeftShoulder).normalized;

        return perpendicularVector.normalized;
    }

    void OnDestroy()
    {

    }

}
