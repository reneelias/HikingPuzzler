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
    [Header("TakeOff")]
    private Vector3 takeOffTargetPosition;
    private Vector3? takeOffTargetRotation;
    [SerializeField] private float takeOffSpeed = 3f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotationVelocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger(animState.ToString());
        InitiateTakeOff(new Vector3(0f, 30f, 0f), Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        switch (owlState){
            case OwlState.Idle:
                break;
            case OwlState.TakingOff:
                TakingOff();
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

    public void InitiateTakeOff(Vector3 takeOffOffset, Vector3? takeOffTargetRotation = null, OwlAnimState takeOffType = OwlAnimState.TakeOffGrounded){
        takeOffTargetPosition = transform.position + takeOffOffset;
        this.takeOffTargetRotation = takeOffTargetRotation; 
        animState = takeOffType;
        owlState = OwlState.TakingOff;
    }

    private void TakingOff(){
        transform.position = Vector3.SmoothDamp(transform.position, takeOffTargetPosition, ref velocity, takeOffSpeed);
        if(takeOffTargetRotation != null){
            transform.localEulerAngles = Vector3.SmoothDamp(transform.localEulerAngles, takeOffTargetRotation.Value, ref rotationVelocity, takeOffSpeed);
        }
        if(Vector3.Distance(transform.position, takeOffTargetPosition) < .05f){
            InitiateFlying();
        }
    }

    public void InitiateFlying(){
        animState = OwlAnimState.Fly;
        owlState = OwlState.Flying;
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
