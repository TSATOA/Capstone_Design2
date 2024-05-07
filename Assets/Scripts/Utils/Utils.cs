using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomSpawnPoint(){
        return new Vector3(Random.Range(31,67),4,Random.Range(-60,-24));
    }

    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach(Transform trans in transform.GetComponentInChildren<Transform>(true)){
            trans.gameObject.layer = layerNumber;
        }
    }

    public static string GetRandomSessionName(){
        return "Session_" + Random.Range(1,100).ToString();
    }
}
