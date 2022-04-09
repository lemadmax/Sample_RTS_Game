using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float motionTime;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("attackSpeedMultiplier", motionTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            animator.SetFloat("speed", 2);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            animator.SetFloat("speed", 0);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            animator.SetInteger("states", 2);
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            animator.SetInteger("states", 0);
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            animator.SetInteger("states", 3);
        }
    }
    public void DealDamage()
    {

    }
    public void AttackEnd()
    {
        animator.SetInteger("states", 0);
    }
}
