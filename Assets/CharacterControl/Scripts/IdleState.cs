using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateMachineBehaviour
{
    float timer;
    float timerThreshold = 2;
    float chaseDistance = 8;
    Transform player;
    float angle;
    float visionAngle = 120.0f;
    Vector3 playerDirection;
    Vector3 forwardDirection;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if(timer > timerThreshold)
        {
            animator.SetBool("isPatrol", true);
        }

        float distance = Vector3.Distance(player.position, animator.transform.position);
        playerDirection = player.position - animator.transform.position;
        forwardDirection = animator.transform.forward;
        angle = Vector3.Angle(forwardDirection, playerDirection);
        if (distance < chaseDistance && angle <= visionAngle / 2.0f)
        {
            animator.SetBool("isChase", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
   override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
