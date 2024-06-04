using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvadeTrigger : MonoBehaviour
{
    public CapsuleCollider head;
    public CapsuleCollider enclosure;
    public CharacterControl characterControl;
    void OnTriggerExit(Collider other)
    {
        if(other == enclosure)
        {
            characterControl.isEvading = true;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other == enclosure)
        {
            characterControl.isEvading = false;
        }
    }
    void Start()
    {
        characterControl.isEvading = false;
    }

}
