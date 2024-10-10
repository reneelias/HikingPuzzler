using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlController : MonoBehaviour
{
    Animator animator;
    [SerializeField] private OwlAnimState animState = OwlAnimState.IdleLookAroundGrounded;
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

    public void SetAnimState(OwlAnimState animState){
        this.animState = animState;
        animator.SetTrigger(animState.ToString());
    }
}
public enum OwlAnimState{
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
