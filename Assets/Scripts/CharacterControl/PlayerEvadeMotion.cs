using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class PlayerEvadeMotion : MonoBehaviour
{
    public CharacterControl characterControl = null;
    private FullBodyBipedIK fullBodyBipedIK = null;
    private FBBIKHeadEffector headEffector = null;
    private bool directionChecked = false;
    private float startTime = 0.0f;
    private float runningTime = 3.0f;
    private Animator animator;
    private Transform characterRoot;
    private Transform head;

    void Start()
    {
        characterRoot = characterControl.characterRoot;
        head = characterControl.nose;
        animator = gameObject.GetComponent<Animator>();
        directionChecked = false;
    }

    void Update()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(characterControl.isEvading && stateInfo.IsName("Idle") && !animator.IsInTransition(0))
        {
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
                    fullBodyBipedIK.enabled = true;
                    directionChecked = false;
                }
            }
        }

    }

    void characterEvade()
    {
        fullBodyBipedIK = gameObject.GetComponent<FullBodyBipedIK>();
        if(fullBodyBipedIK.enabled && !animator.IsInTransition(0) && fullBodyBipedIK != null)
        {
            // headEffector = head.gameObject.GetComponent<FBBIKHeadEffector>();

            fullBodyBipedIK.enabled = false;
            
            if(directionChecked == false){
                var headGlobal = head.transform.position;
                headGlobal.y = gameObject.transform.position.y;

                gameObject.transform.LookAt(headGlobal);
                gameObject.transform.Rotate(0, -90, 0);

                directionChecked = true;
            }

            animator.SetBool("Evade", true);

        }
    }
}
