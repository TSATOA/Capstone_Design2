using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using RootMotion.FinalIK;
using UnityEngine;

public class PlayerAnimState : MonoBehaviour
{
    public CharacterControl characterControl = null;
    private FullBodyBipedIK fullBodyBipedIK = null;
    private bool newdirectionChecked = false;
    private bool gameOver = false;
    private float startTime = 0.0f;
    private float runningTime = 0.5f;
    private Animator animator;
    public Animator enemyAnimator;
    private Transform head;
    public GameObject idleCamera;
    public GameObject winCamera;

    void Start()
    {
        head = characterControl.nose;
        animator = gameObject.GetComponent<Animator>();
        newdirectionChecked = false;
    }

    void Update()
    {
        var playerStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        var enemyStateInfo =  enemyAnimator.GetCurrentAnimatorStateInfo(0);
        if(enemyStateInfo.IsName("Death") && !gameOver)
        {
            gameOver = true;
            Debug.Log("Character Win");
            characterWin();
        }
        else{
            if(characterControl.isEvading && playerStateInfo.IsName("Idle") && !animator.IsInTransition(0))
            {
                fullBodyBipedIK = gameObject.GetComponent<FullBodyBipedIK>();
                if(fullBodyBipedIK != null) fullBodyBipedIK.enabled = true;
                if(startTime == 0.0f)
                {
                    startTime = Time.time;
                }
                runningTime = Time.time - startTime;
                if(runningTime >= 1.0f)
                {
                    characterEvade();
                }
            }
            else 
            {
                startTime = 0.0f;
                if(playerStateInfo.IsName("Jump Away") && !animator.IsInTransition(0) && fullBodyBipedIK != null)
                {
                    if(playerStateInfo.normalizedTime >= 1.3f)
                    {
                        animator.SetBool("Evade", false);
                        newdirectionChecked = false;
                    }
                }
            }
        }
    }

    void characterEvade()
    {
        if(fullBodyBipedIK.enabled && fullBodyBipedIK != null)
        {
            fullBodyBipedIK.enabled = false;
            
            if(newdirectionChecked == false){
                var headGlobal = head.transform.position;
                headGlobal.y = gameObject.transform.position.y;

                gameObject.transform.LookAt(headGlobal);

                newdirectionChecked = true;
            }

            animator.SetBool("Evade", true);

        }
    }

    void characterWin()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        if(fullBodyBipedIK.enabled && fullBodyBipedIK != null)
        {
            fullBodyBipedIK.enabled = false;

            idleCamera.SetActive(false);
            winCamera.SetActive(true);

            animator.SetBool("Evade", false);
            animator.SetTrigger("Win");
        }
    }
}
