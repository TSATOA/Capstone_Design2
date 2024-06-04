using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class PlayerEvadeMotion : MonoBehaviour
{
    public CharacterControl characterControl = null;
    private FullBodyBipedIK fullBodyBipedIK = null;
    private FBBIKHeadEffector headEffector = null;
    private bool newdirectionChecked = false;
    private float startTime = 0.0f;
    private float runningTime = 2.0f;
    private Animator animator;
    private Transform characterRoot;
    private Transform head;

    void Start()
    {
        characterRoot = characterControl.characterRoot;
        head = characterControl.nose;
        animator = gameObject.GetComponent<Animator>();
        newdirectionChecked = false;
    }

    void Update()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(characterControl.isEvading && stateInfo.IsName("Idle") && !animator.IsInTransition(0))
        {
            fullBodyBipedIK = gameObject.GetComponent<FullBodyBipedIK>();
            if(fullBodyBipedIK != null) fullBodyBipedIK.enabled = true;
            if(startTime == 0.0f)
            {
                startTime = Time.time;
            }
            runningTime = Time.time - startTime;
            if(runningTime >= 3.0f)
            {
                characterEvade();
            }
        }
        else 
        {
            startTime = 0.0f;
            if(stateInfo.IsName("Jump Away") && fullBodyBipedIK != null)
            {
                if(stateInfo.normalizedTime >= 1.0f)
                {
                    animator.SetBool("Evade", false);
                    newdirectionChecked = false;
                }
            }
        }

    }

    void characterEvade()
    {
        if(fullBodyBipedIK.enabled && !animator.IsInTransition(0) && fullBodyBipedIK != null)
        {
            // headEffector = head.gameObject.GetComponent<FBBIKHeadEffector>();

            fullBodyBipedIK.enabled = false;
            
            if(newdirectionChecked == false){
                var headGlobal = head.transform.position;
                headGlobal.y = gameObject.transform.position.y;

                gameObject.transform.LookAt(headGlobal);
                gameObject.transform.Rotate(0, -90, 0);

                newdirectionChecked = true;
            }

            animator.SetBool("Evade", true);

        }
    }
}
