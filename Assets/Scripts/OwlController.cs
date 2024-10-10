using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlController : MonoBehaviour
{
    Animator animator;
    public AnimState animState = AnimState.IdleLookAroundGrounded;
    public enum AnimState{
        Falling,
        Fly,
        Glide,
        IdleLookAroundGrounded,
        IdleLookAroundBranch,
        IdleStareBranch,
        IdleStareGrounded,
        LandingBranch,
        LandingGrounded,
        TakeOffBranch,
        TakeOffGrounded,
        Walk,
        Walk_RM
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger(animState.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
