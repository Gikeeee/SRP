using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlToCatBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PoolManager.GetInstance().GetGObj("Role/Cat", (newRole) =>
        {
            newRole.transform.parent = animator.transform.parent;
            newRole.transform.position = animator.transform.position;
            newRole.transform.localScale = animator.transform.localScale;
            PoolManager.GetInstance().PushGObj(animator.gameObject.name, animator.gameObject);
            AnimateManager.Instance.SetAnimator(newRole.GetComponent<Animator>());
        });
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
