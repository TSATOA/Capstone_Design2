using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeshControl : MonoBehaviour
{
    public GameObject[] disableMesh;
    public float disableDelay = 2.0f;

    public void ControlMesh(bool isAim)
    {
        foreach(GameObject obj in disableMesh)
        {
            if (obj != null)
            {
                StartCoroutine(DisableGameObjectAfterDelay(obj, disableDelay, isAim));
            }
        }
    }

    IEnumerator DisableGameObjectAfterDelay(GameObject obj, float delay, bool isAim)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(!isAim);
    }
}
