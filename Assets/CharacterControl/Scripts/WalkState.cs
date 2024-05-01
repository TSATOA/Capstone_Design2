using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkState : StateMachineBehaviour
{
    float timer;
    float timerThreshold = 10;
    float chaseDistance = 8;
    List<Transform> wayPoints = new List<Transform> ();
    NavMeshAgent agent;
    Transform player;
    Vector3 playerDirection;
    Vector3 forwardDirection;
    float angle;
    float visionAngle = 120.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        timer = 0;
       
        GameObject go = GameObject.FindGameObjectWithTag("Waypoint");
        foreach (Transform t in go.transform)
            wayPoints.Add(t);

        agent.SetDestination(wayPoints[Random.Range(0, wayPoints.Count)].position);
        agent.speed = 1.8f;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
            agent.SetDestination(wayPoints[Random.Range(0, wayPoints.Count)].position);

        if (timer > 100000)
            timer -= 100000;
        timer += Time.deltaTime;
        if (timer > timerThreshold)
        {
            animator.SetBool("isPatrol", false);
            animator.SetBool("isOverwatch", true);
        }

        float distance = Vector3.Distance(player.position, animator.transform.position);
        playerDirection = player.position - animator.transform.position;
        forwardDirection = animator.transform.forward;
        angle = Vector3.Angle(forwardDirection, playerDirection);
        if (distance < chaseDistance && angle <= (visionAngle / 2.0f))
        {
            animator.SetBool("isChase", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
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
