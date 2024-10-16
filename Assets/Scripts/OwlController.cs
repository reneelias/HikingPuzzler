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
    [SerializeField] private Vector3 takeOffOffset = new Vector3(0f, 30f, 0f);
    [SerializeField] private Vector3 takeOffTargetRotation = new Vector3(0f, 360f, 0f);
    [Tooltip("The amount of time the entire take off sequence will take.")]
    [SerializeField] private float takeOffDuration = 3f;
    [Tooltip("The max amount of degrees allowed to turn on the Z axis.")]
    [SerializeField] private float maxRotationZ = 80f;
    [Tooltip("The highest y velocity magnitude considered when being used to lerp z rotation.")]
    [SerializeField] private float yVelocityTurnThreshold = 60f;
    [SerializeField] private float rotationSpeedScaler = .125f;
    private Vector3 takeOffTargetPosition;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotationVelocity = Vector3.zero;
    private float takeOffProgress = 0f;
    private Vector3 takeOffStartingPosition;
    private float takeOffStartTime;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(1f);
        animator = GetComponent<Animator>();
        animator.SetTrigger(animState.ToString());
        yield return new WaitForSecondsRealtime(1f);
        InitiateTakeOff(takeOffOffset, takeOffTargetRotation);
        // InitiateTakeOff(takeOffOffset);
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

    public void InitiateTakeOff(Vector3 takeOffOffset, Vector3 takeOffTargetRotation, OwlAnimState takeOffType = OwlAnimState.TakeOffGrounded){
        takeOffTargetPosition = transform.position + takeOffOffset;
        this.takeOffTargetRotation = takeOffTargetRotation; 
        animState = takeOffType;
        owlState = OwlState.TakingOff;
        takeOffStartingPosition = transform.position;
        takeOffStartTime = Time.time;
    }

    public void InitiateTakeOff(Vector3 takeOffOffset, OwlAnimState takeOffType = OwlAnimState.TakeOffGrounded){
        takeOffTargetPosition = transform.position + takeOffOffset;
        takeOffTargetRotation = transform.eulerAngles; 
        animState = takeOffType;
        owlState = OwlState.TakingOff;
        takeOffStartingPosition = transform.position;
        takeOffStartTime = Time.time;
    }

    private void TakingOff(){
        transform.position = Vector3.Lerp(takeOffStartingPosition, takeOffTargetPosition, takeOffProgress);
        takeOffProgress = Mathf.Sin((Time.time - takeOffStartTime)/takeOffDuration * Mathf.PI / 2f);

        animator.speed = Mathf.Max(velocity.magnitude / 8f, .5f);

        transform.localEulerAngles = Vector3.SmoothDamp(transform.localEulerAngles, takeOffTargetRotation, ref rotationVelocity, takeOffDuration * rotationSpeedScaler);
        if(rotationVelocity.y != 0f){
            float zRotation = Mathf.Lerp(0f, maxRotationZ, Mathf.Clamp(Mathf.Abs(rotationVelocity.y / yVelocityTurnThreshold), 0f, 1f));
            zRotation *= rotationVelocity.y < 0 ? 1 : -1;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, zRotation);
        }

        if(Vector3.Distance(transform.position, takeOffTargetPosition) < .05f){
            transform.eulerAngles = takeOffTargetRotation;
            transform.position = takeOffTargetPosition;
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
