using UnityEngine;
using System.Collections.Generic;
using System;
using PoseInformation;
using RootMotion.FinalIK;
using UnityEngine.Animations.Rigging;
using RootMotion;

public class CharacterControl : MonoBehaviour
{
    // Game Objects
    private List<GameObject> threeDPoints;
    public bool visualizeKeypoints = false;
    public string poseName = "pose";

    private GameObject poseGroup;
    // Final IK
    private FullBodyBipedIK fullBodyIK;
    private FBBIKHeadEffector headEffector;

    // For joint control
    public Transform characterRoot;
    public Transform pelvis;
    public Transform rightHip;
    public Transform leftHip;
    public Transform spineRoot;
    public Transform spineMiddle;
    public Transform spineTip;
    public Transform neck;
    public Transform nose;
    public Transform leftShoulder;
    public Transform leftArm;
    public Transform leftForeArm;
    public Transform leftWrist;
    public Transform rightShoulder;
    public Transform rightArm;
    public Transform rightForeArm;
    public Transform rightWrist;

    // Character Evade Parameters
    public bool isEvading;

    void Start()
    {
        // IK setup
        init3DKeypoints(poseName);
        AddFullBodyIK(gameObject);
        addAimContraint();
        PoseFormat.BoneDistances[PoseFormat.Bone.RootToRhip] = distAtoB(characterRoot.position, rightHip.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.RootToLhip] = distAtoB(characterRoot.position, leftHip.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.RootToBelly] = distAtoB(characterRoot.position, spineMiddle.position);
        PoseFormat.BoneDistances[PoseFormat.Bone.BellyToNeck] = 0.9f * distAtoB(spineMiddle.position, neck.position);
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

        Draw3DJoints(scaledOutput, visualizeKeypoints);

        // if(isEvading)
        // {
        //     Debug.Log("Is Evading");
        // }

    }

    public void init3DKeypoints(string name)
    {
        threeDPoints = new List<GameObject>();
        poseGroup = new GameObject(name);
        poseGroup.transform.SetParent(characterRoot);
        // poseGroup.transform.localPosition = pelvis.localPosition;
        poseGroup.transform.localPosition = Vector3.zero;

        Array keypoints = Enum.GetValues(typeof(PoseFormat.Keypoint));

        foreach(var keypoint in keypoints)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            string jointName = Enum.GetName(typeof(PoseFormat.Keypoint), keypoint);
            sphere.name = String.Format("{0}_{1}", poseName, jointName.ToLower());

            sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            sphere.transform.localPosition = new Vector3(0, 0, 0);

            sphere.transform.SetParent(poseGroup.transform, false);

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

    public void Draw3DJoints(Vector3[] bodyJoints, bool visualizeKeypoints)
    {
        // poseGroup.transform.rotation = pelvis.rotation;
        for (int idx = 0; idx < bodyJoints.Length; idx++)
        {
            GameObject bodyJoint = threeDPoints[idx];
            bodyJoint.transform.localPosition = bodyJoints[idx];
            bodyJoint.SetActive(visualizeKeypoints);
        }
    }

    private void AddFullBodyIK(GameObject humanoid, BipedReferences references = null)
    {
        if(references == null)
        {
            BipedReferences.AutoDetectReferences(ref references, humanoid.transform, BipedReferences.AutoDetectParams.Default);
        }

        fullBodyIK = humanoid.AddComponent<FullBodyBipedIK>();
        fullBodyIK.SetReferences(references, null);
        fullBodyIK.solver.SetLimbOrientations(BipedLimbOrientations.UMA);

        fullBodyIK.solver.rootNode = pelvis;
        fullBodyIK.references.spine[1] = spineTip;

        // Body
        fullBodyIK.solver.bodyEffector.target = threeDPoints[(int)PoseFormat.Keypoint.Root].transform;
        fullBodyIK.solver.bodyEffector.positionWeight = 0.15f;
        fullBodyIK.solver.pullBodyVertical = 0.1f;
        fullBodyIK.solver.pullBodyHorizontal = 0.08f;

        // Left Arm
        fullBodyIK.solver.leftHandEffector.target = threeDPoints[(int)PoseFormat.Keypoint.Lwrist].transform;
        fullBodyIK.solver.leftHandEffector.positionWeight = 0.95f;
        
        fullBodyIK.solver.leftShoulderEffector.target = threeDPoints[(int)PoseFormat.Keypoint.Lshoulder].transform;
        fullBodyIK.solver.leftShoulderEffector.positionWeight = 0.7f;

        fullBodyIK.solver.chain[1].bendConstraint.bendGoal = threeDPoints[(int)PoseFormat.Keypoint.Lelbow].transform;
        fullBodyIK.solver.chain[1].bendConstraint.weight = 0.5f;

        // Right Arm
        fullBodyIK.solver.rightHandEffector.target = threeDPoints[(int)PoseFormat.Keypoint.Rwrist].transform;
        fullBodyIK.solver.rightHandEffector.positionWeight = 0.95f;

        fullBodyIK.solver.rightShoulderEffector.target = threeDPoints[(int)PoseFormat.Keypoint.Rshoulder].transform;
        fullBodyIK.solver.rightShoulderEffector.positionWeight = 0.5f;

        fullBodyIK.solver.chain[2].bendConstraint.bendGoal = threeDPoints[(int)PoseFormat.Keypoint.Relbow].transform;
        fullBodyIK.solver.chain[2].bendConstraint.weight = 0.8f;

        // Head
        // 머리 회전까지 반영할 필요는 없음
        addHeadEffector(threeDPoints[(int)PoseFormat.Keypoint.Head]);
    }

    private void addHeadEffector(GameObject headTarget)
    {
        headEffector = headTarget.AddComponent<FBBIKHeadEffector>();

        headEffector.ik = fullBodyIK;
        headEffector.positionWeight = 0.5f;
        headEffector.bodyWeight = 0.4f;
        headEffector.thighWeight = 0.2f;

        headEffector.rotationWeight = 0.0f;
        headEffector.bodyClampWeight = 0.5f;
        headEffector.headClampWeight = 0.5f;
        headEffector.bendWeight = 0.5f;

        headEffector.bendBones = new FBBIKHeadEffector.BendBone[2];

        headEffector.bendBones[0] = new FBBIKHeadEffector.BendBone(spineTip, 0.647f);
        headEffector.bendBones[1] = new FBBIKHeadEffector.BendBone(neck, 0.874f);

        headEffector.CCDWeight = 1.0f;
        headEffector.damper = 500.0f;

        headEffector.postStretchWeight = 1.0f;
        headEffector.maxStretch = 0.1f;
    }

    private void addAimContraint()
    {
        
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

    void OnDestroy()
    {

    }

}
