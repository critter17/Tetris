using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("State Enter");

        Destroy(animator.gameObject, stateInfo.length);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetTrigger("Blink");
    }
}
