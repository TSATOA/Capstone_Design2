using UnityEngine;
using Unity.Sentis;
using FF = Unity.Sentis.Functional;
using Unity.VisualScripting;
using System;

public class PoseEstimator : IDisposable {
    
    private TextureTransform textureTransform;

    private IWorker twoDPoseWorker;
    private IWorker threeDPoseWorker;
    private BackendType backend;
    public const int numJoints = 17;

    private TensorFloat inputTensor = null;

    private float iouThreshold = 0.5f;
    private float scoreThreshold = 0.5f;

    private Vector3[] threeDJointsVector; // Store 3D joints as Vector3 array

    public PoseEstimator(int resizedSquareImageDim, ref ModelAsset twoDPoseModelAsset, ref ModelAsset threeDPoseModelAsset, BackendType backend) {

            this.backend = backend;

            textureTransform = new TextureTransform().SetDimensions(width: resizedSquareImageDim, height: resizedSquareImageDim, channels: 3);

            LoadModel(resizedSquareImageDim, ref twoDPoseModelAsset, ref threeDPoseModelAsset);

            threeDJointsVector = new Vector3[numJoints];

    }

    private void LoadModel(int resizedSquareImageDim, ref ModelAsset twoDPoseModelAsset, ref ModelAsset threeDPoseModelAsset) {

        var twoDPoseModel = ModelLoader.Load(twoDPoseModelAsset);
        var threeDPoseModel = ModelLoader.Load(threeDPoseModelAsset);

        TensorFloat centersToCorners = new TensorFloat(new TensorShape(4, 4), new float[]{
            1,0,1,0,
            0,1,0,1,
            -0.5f, 0, 0.5f, 0,
            0, -0.5f, 0, 0.5f
        });

        twoDPoseModel = FF.Compile(
            input => {
                var modelOutput = twoDPoseModel.Forward(input)[0];
                var boxCoords = modelOutput[0, 0..4, ..].Transpose(0, 1);                   // shape=(8400,4)
                var jointsCoords = modelOutput[0, 5.., ..].Transpose(0, 1);                 // shape=(8400,51)
                var scores = modelOutput[0, 4, ..] - scoreThreshold;
                var boxCorners = FF.MatMul(boxCoords, FunctionalTensor.FromTensor(centersToCorners));
                var indices = FF.NMS(boxCorners, scores, iouThreshold);                     // shape=(1)
                var indices_joints = indices.Unsqueeze(-1).BroadcastTo(new int[] { 51 });   // shape=(1,51)
                var joints_coords = FF.Gather(jointsCoords, 0, indices_joints);             // shape=(1,51)
                var joints_reshaped = joints_coords.Reshape(new int[] { 1, 1, 17, -1 });    // shape=(1,1,17,3)
                return joints_reshaped;
            },
            InputDef.FromModel(twoDPoseModel)[0]
        );

        /*
            COCO:
            0: nose 1: Leye 2: Reye 3: Lear 4Rear
            5: Lsho 6: Rsho 7: Lelb 8: Relb 9: Lwri
            10: Rwri 11: Lhip 12: Rhip 13: Lkne 14: Rkne
            15: Lank 16: Rank
            
            H36M:
            0: root, 1: rhip, 2: rkne, 3: rank, 4: lhip,
            5: lkne, 6: lank, 7: belly, 8: neck, 9: nose,
            10: head, 11: lsho, 12: lelb, 13: lwri, 14: rsho,
            15: relb, 16: rwri
        */

        threeDPoseModel = FF.Compile(
            input => {
                input[..,..,..,..2] -= (float)resizedSquareImageDim / 2;
                input[..,..,..,..2] /= (float)resizedSquareImageDim;
                var y = input.Clone();
                y[..,..,0,..] = (input[..,..,11,..] + input[..,..,12,..]) * 0.5f;
                y[..,..,1,..] = input[..,..,12,..];
                y[..,..,2,..] = input[..,..,14,..];
                y[..,..,3,..] = input[..,..,16,..];
                y[..,..,4,..] = input[..,..,11,..];
                y[..,..,5,..] = input[..,..,13,..];
                y[..,..,6,..] = input[..,..,15,..];
                y[..,..,8,..] = (input[..,..,5,..] + input[..,..,6,..]) * 0.5f;
                y[..,..,7,..] = (y[..,..,0,..] + y[..,..,8,..]) * 0.5f;
                y[..,..,9,..] = input[..,..,0,..];
                y[..,..,10,..] = (input[..,..,1,..] + input[..,..,2,..]) * 0.5f;
                y[..,..,11,..] = input[..,..,5,..];
                y[..,..,12,..] = input[..,..,7,..];
                y[..,..,13,..] = input[..,..,9,..];
                y[..,..,14,..] = input[..,..,6,..];
                y[..,..,15,..] = input[..,..,8,..];
                y[..,..,16,..] = input[..,..,10,..];
                var output = threeDPoseModel.Forward(y)[0];
                output[..,..,..,..] *= -1;
                return output;
            },
            InputDef.FromTensor(TensorFloat.AllocNoData(new TensorShape(1,1,17,3)))
        );

        centersToCorners.Dispose();

        twoDPoseWorker = WorkerFactory.CreateWorker(backend, twoDPoseModel);
        threeDPoseWorker = WorkerFactory.CreateWorker(backend, threeDPoseModel);

    }

    public bool RunML(WebCamTexture webcamTexture) {

        bool hasPredicted = false;

        inputTensor?.Dispose();

        inputTensor = TextureConverter.ToTensor(webcamTexture, textureTransform);

        twoDPoseWorker.Execute(inputTensor);
        
        var twoDJointsTensor = twoDPoseWorker.PeekOutput() as TensorFloat;
        
        twoDJointsTensor.CompleteOperationsAndDownload();

        if(twoDJointsTensor.shape[2] == numJoints && twoDJointsTensor.shape[3] == 3) {

            threeDPoseWorker.Execute(twoDJointsTensor);

            var threeDJointsTensor = threeDPoseWorker.PeekOutput() as TensorFloat;

            threeDJointsTensor.CompleteOperationsAndDownload();

            for (int idx = 0; idx < numJoints; idx++) {

                threeDJointsVector[idx].x = threeDJointsTensor[0,0,idx,0];
                threeDJointsVector[idx].y = threeDJointsTensor[0,0,idx,1];
                threeDJointsVector[idx].z = threeDJointsTensor[0,0,idx,2];

            }

            hasPredicted = true;

        }

        return hasPredicted;

    }

    public Vector3[] getThreeDPose() {

        return threeDJointsVector;
        
    }

    public void Dispose() {

        inputTensor?.Dispose();
        twoDPoseWorker?.Dispose();
        threeDPoseWorker?.Dispose();

    }

    ~PoseEstimator() {

        Dispose();

    }

}