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
    private Vector3 hitNormal = Vector3.up;


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
        JumpingControls();
        ResetPosition();
        TurningControls();
    }

    void FixedUpdate()
    {
        MovementControls();
    }

    void MovementControls(){
        movementVector = new Vector3();

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        movementVector = transform.right * moveX + transform.forward * moveZ;

        if(movementVector.magnitude == 0f){
            if(moveVelocity.magnitude > 0f){
                moveVelocity *= moveVelDampRate;
            }
            
        } else {
            movementVector = movementVector.normalized * movementSpeed * (grounded ? 1f : inAirMoveDamp);
            moveVelocity += movementVector;
            bool sprinting = Input.GetKey(KeyCode.LeftShift);
            float maxSpeed = moveVelMaxSpeed;
            maxSpeed *= sprinting ? sprintSpeedMultiplier : 1f;
            if(moveVelocity.magnitude > maxSpeed){
                moveVelocity = moveVelocity.normalized * maxSpeed;
            }
        }

        if(usesRigidbody){
            rigidbody.velocity += movementVector;
            rigidbody.rotation = Quaternion.Euler(0f, rigidbody.rotation.eulerAngles.y, 0f);
            rigidbody.angularVelocity = Vector3.zero;
        } else {
            print("Angle to ground normal: " + Vector3.Angle(Vector3.up, hitNormal));
            if(grounded && Vector3.Angle(Vector3.up, hitNormal) <= characterController.slopeLimit && movementVector.magnitude > 0f){
                moveVelocity.x += (1f - hitNormal.y) * hitNormal.x * (1f - slideFriction);
                moveVelocity.z += (1f - hitNormal.y) * hitNormal.z * (1f - slideFriction);
            }
            characterController.Move(moveVelocity);
            // grounded = Vector3.Angle(Vector3.up, hitNormal) <= characterController.slopeLimit;
            // if(!isGrounded){

            // }
        }
    }

    void JumpingControls(){
        int i = 0;
        foreach(Transform transform in raycastPointsParent.transform){
            if(Physics.Raycast(new Ray(transform.position, gravity.normalized), rayCastDistance)){
                if(!grounded){
                    // upVector = Vector3.zero;
                    upVector = gravity * .25f;
                }
                grounded = true;
                break;
            }
            i++;
        }
        if(i == raycastPointsParent.transform.childCount){
            grounded = false;
        }

        if(grounded){
            if(Input.GetKeyDown(KeyCode.Space)){
                upVector = new Vector3(-gravity.x, -gravity.y, -gravity.z).normalized * jumpSpeed;
                if(usesRigidbody){
                    rigidbody.velocity += upVector;
                }
            }
        } else {
            if(usesRigidbody){
                rigidbody.velocity += gravity * Time.deltaTime;
                print(rigidbody.velocity);
            } else {
                upVector += gravity * gravityScale * Time.deltaTime;
            }
        }
        
        if(usesRigidbody){
            // rigidbody.velocity = moveVelocity;
        } else {
            characterController.Move(upVector * Time.deltaTime);
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

    /// <summary>
    /// OnControllerColliderHit is called when the controller hits a
    /// collider while performing a Move.
    /// </summary>
    /// <param name="hit">The ControllerColliderHit data associated with this collision.</param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

}
