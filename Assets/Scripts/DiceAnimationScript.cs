using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RollDice()
    {
        animator.SetBool("IsRoling", true);
    }

    public void StopDiceRoll()
    {
        animator.SetBool("IsRoling", false);
    }
}
