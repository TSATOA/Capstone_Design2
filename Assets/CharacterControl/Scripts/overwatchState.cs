using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overwatchState : StateMachineBehaviour
{
    Transform player;
    float chaseDistance = 8.0f;
    float visionAngle = 120.0f;
    Vector3 playerDirection;
    Vector3 forwardDirection;
    float angle;
    float timer;
    float timerThreshold = 6.0f;
    float overwatchAngle = 360.0f;
    float rotationSpeed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0.0f;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        rotationSpeed = overwatchAngle / timerThreshold;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        float rotationAmount = rotationSpeed * Time.deltaTime;
        animator.transform.Rotate(0f, rotationAmount, 0f);

        if (timer > timerThreshold)
        {
            animator.SetBool("isOverwatch", false);
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
