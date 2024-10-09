using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private bool usesRigidbody = true;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Color materialColor;

    [Header("Movement")]
    [Tooltip("How quickly the player picks up speed.")]
    [SerializeField] private float movementSpeed = .05f;
    [Tooltip("Scale by which movement speed gets dampened when player is in the air.")]
    [SerializeField] private float inAirMoveDamp = .5f;
    [Tooltip("That maximum speed that the player can move it using directional input.")]
    [SerializeField] private float moveVelMaxSpeed = 1f;
    [SerializeField] private float sprintSpeedMultiplier = 2f;
    [Tooltip("Scale by which velocity will be multiplied each update when no movement input detected.")]
    [SerializeField] private float moveVelDampRate = .1f;
    [SerializeField] private float slopeLimit = 80f;
    [SerializeField] private float slideFriction = .3f;
    private Vector3 moveVelocity = Vector3.zero;

    [Header("Gravity")]
    [SerializeField] private float gravityScale = -9.81f;
    [SerializeField] private float jumpSpeed = 3f;
    private Vector3 gravity = Vector3.down;
    private Vector3 upVector = Vector3.up;

    [Header("Jumping Raycast")]
    [SerializeField] private GameObject raycastPointsParent;
    [SerializeField] private float rayCastDistance = .25f;
    private bool grounded = false;

    [Header("Turning")]
    [SerializeField] private float turningSpeed = 1f;
    [SerializeField] private float mouseTurnSpeed = 5f;
    [SerializeField] private GameObject cameraParent; 
    [SerializeField] private float upTurnMaxAngle = 45f;
    private float upVelocity = 0f;
    private Vector3 movementVector = new Vector3();
    private Vector3 originalPosition;
    private float cameraUpAngle = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        GetComponent<Renderer>().material.color = materialColor;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MovementControls();
        ResetPosition();
        TurningControls();
    }

    void FixedUpdate()
    {
    }

    void MovementControls(){
        int i = 0;
        foreach(Transform transform in raycastPointsParent.transform){
            RaycastHit raycastHit;
            if(Physics.Raycast(new Ray(transform.position, gravity.normalized), out raycastHit, rayCastDistance) && raycastHit.collider.name != "Player"){
                if(!grounded){
                    // upVector = Vector3.zero;
                    upVector = gravity * .0125f;
                }
                grounded = true;
                break;
            }
            i++;
        }
        if(i == raycastPointsParent.transform.childCount){
            grounded = false;
        }

        movementVector = new Vector3();

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        movementVector = transform.right * moveX + transform.forward * moveZ;
        Vector3 xzMoveVel = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        if(movementVector.magnitude == 0f){
            if(xzMoveVel.magnitude > 0f){
                xzMoveVel *= moveVelDampRate * Time.deltaTime;
            }
            
        } else {
            movementVector = movementVector.normalized * movementSpeed * (grounded ? 1f : inAirMoveDamp * Time.deltaTime);
            movementVector = AdjustVelocityToSlope(movementVector);
           
            xzMoveVel += movementVector;
            bool sprinting = Input.GetKey(KeyCode.LeftShift);
            float maxSpeed = moveVelMaxSpeed;
            maxSpeed *= sprinting ? sprintSpeedMultiplier : 1f;

            if(xzMoveVel.magnitude > maxSpeed){
                xzMoveVel = xzMoveVel.normalized * maxSpeed;
            }
        }

        if(grounded){
            if(Input.GetKeyDown(KeyCode.Space)){
                upVector = new Vector3(-gravity.x, -gravity.y, -gravity.z).normalized * jumpSpeed;
                moveVelocity.y = 0;
                if(usesRigidbody){
                    rigidbody.velocity += upVector;
                }
            }
        } else {
            if(usesRigidbody){
                rigidbody.velocity += gravity * Time.deltaTime;
                print(rigidbody.velocity);
            } else {
                upVector = gravity * gravityScale * Time.deltaTime;
            }
        }

        xzMoveVel.y = moveVelocity.y + upVector.y;
        moveVelocity = xzMoveVel;

        if(usesRigidbody){
            rigidbody.velocity += movementVector;
            rigidbody.rotation = Quaternion.Euler(0f, rigidbody.rotation.eulerAngles.y, 0f);
            rigidbody.angularVelocity = Vector3.zero;
        } else {
            characterController.Move(moveVelocity);
            if(!grounded){
                print(moveVelocity);
            }
        }
    }
    void ResetPosition(){
        if(Input.GetKeyDown(KeyCode.R)){
            if(usesRigidbody){
                transform.position = originalPosition;
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            } else {
                characterController.enabled = false;
                transform.position = originalPosition;
                characterController.enabled = true;
            }
            transform.eulerAngles = Vector3.zero;
        }
    }

    void TurningControls(){
        float axisMouseX = Input.GetAxis("Mouse X");
        float axisMouseY = Input.GetAxis("Mouse Y");

        if(axisMouseX != 0){
            transform.eulerAngles += new Vector3(0, axisMouseX * mouseTurnSpeed * Time.deltaTime, 0f);
        }
        if(axisMouseY != 0){
            cameraUpAngle -= axisMouseY * mouseTurnSpeed * Time.deltaTime;
            cameraUpAngle = Mathf.Clamp(cameraUpAngle, -upTurnMaxAngle, upTurnMaxAngle);
            cameraParent.transform.localEulerAngles = new Vector3(cameraUpAngle, 0f, 0f);
        }
    }


    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

}