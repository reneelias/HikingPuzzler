using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlController : MonoBehaviour
{
    Animator animator;
    [SerializeField] private OwlAnimState animState = OwlAnimState.IdleLookAroundGrounded;

    public enum OwlState{
        Idle,
        TakingOff,
        Flying,
        Landing
    }

    public OwlState owlState { get; private set; } = OwlState.Idle;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger(animState.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        switch (owlState){
            case OwlState.Idle:
                break;
            case OwlState.TakingOff:
                break;
            case OwlState.Flying:
                break;
            case OwlState.Landing:
                break;
        }
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
