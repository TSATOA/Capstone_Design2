using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ManualupdateCM : MonoBehaviour
{
    CinemachineBrain brain;
    void Start()
    {
        brain = GetComponent<CinemachineBrain>();
        brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.ManualUpdate;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        brain.ManualUpdate();
    }
}
