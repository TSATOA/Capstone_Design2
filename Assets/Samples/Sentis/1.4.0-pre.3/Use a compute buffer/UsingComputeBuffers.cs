using Unity.Sentis;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class UsingComputeBuffers : MonoBehaviour
{
    [SerializeField]
    ModelAsset modelAsset;
    [SerializeField]
    ComputeShader computeProducer;
    [SerializeField]
    Texture2D textureInput;

    TensorFloat m_TensorA;
    TensorFloat m_TensorB;
    Dictionary<string, Tensor> m_Inputs;
    IWorker m_Engine;
    int m_ComputeKernelIndex;
    int m_ComputeResultIndex;

    void OnEnable()
    {
        // Everything that can be statically assigned is setup during Start to avoid memory churn.

        var model = ModelLoader.Load(modelAsset);

        // The first input to the Adder, using 32*32*3 because that's how many values are in the texture.
        m_TensorA = TensorFloat.AllocZeros(new TensorShape(1, 3, textureInput.height, textureInput.width));

        // The values of B will be added to all values of A.
        m_TensorB = TextureConverter.ToTensor(textureInput);

        Debug.Assert(m_TensorB.shape == new TensorShape(1, 3, textureInput.height, textureInput.width));
        Debug.Assert(m_TensorB.shape[-1] == textureInput.width);
        Debug.Assert(m_TensorB.shape[-2] == textureInput.height);

        m_Inputs = new Dictionary<string, Tensor>
        {
            { "A", m_TensorA },
            { "B", m_TensorB },
        };

        m_ComputeKernelIndex = computeProducer.FindKernel("CSMain");
        m_ComputeResultIndex = Shader.PropertyToID("Result");

        m_Engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, model);

        Debug.Assert(m_TensorA.shape.length == 3 * textureInput.width * textureInput.height);
    }

    void OnDisable()
    {
        m_TensorA.Dispose();
        m_TensorB.Dispose();
        m_Engine.Dispose();
    }

    void Update()
    {
        var gpuTensorX = ComputeTensorData.Pin(m_TensorA);

        // The fastest path is to dispatch compute directly on this tensor's compute buffer.
        computeProducer.SetBuffer(m_ComputeKernelIndex, m_ComputeResultIndex, gpuTensorX.buffer);

        // This shader sets every float element to the value of 41.
        // For simplicity, the shader is setup to index only using id.x, so dispatch must be (x=m_TensorX.shape.length, y=1, z=1).
        computeProducer.Dispatch(m_ComputeKernelIndex, m_TensorA.shape.length, 1, 1);

        // Sentis executes the Adder model, which takes two tensors and adds them.
        // In this case, all elements of tensor X are set to 41 and the value of tensor Y is set to 1 (from the white texture, where all values are 1.0).
        // The expected output of this call is 42.
        m_Engine.Execute(m_Inputs);

        // Peek the value from Sentis, without taking ownership of the Tensor (see PeekOutput docs for details).
        var outputTensor = m_Engine.PeekOutput() as TensorFloat;

        // Calling MakeReadable will trigger a blocking download of the data to the CPU cache.
        // See the AsyncReadback sample for how to access the data in a non-blocking way.
        outputTensor.CompleteOperationsAndDownload();
        // Use -1 index to read the last value in the tensor.
        Debug.Assert(outputTensor[0, 2, textureInput.height - 1, textureInput.width - 1] == 42f);
        Debug.Log(outputTensor[outputTensor.shape.length - 1]);
    }
}
