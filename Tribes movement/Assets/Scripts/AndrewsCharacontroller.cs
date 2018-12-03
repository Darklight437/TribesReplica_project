using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct FPSControllerSettings
{
    // movement
    public float maxWalkSpeed;
    public float maxRunSpeed;
    public float acceleration;
    public float deceleration;
    //public int jumpCount;
    //public float jumpForce;
    public float gravityScale;
    public float fallSpeedMultiplier;
    public float lowJumpFallSpeedMultiplier;
    public float maxFallSpeed;

    // camera
    public float lookSensitivity;
    public float cameraLimitMax;
    public float cameraLimitMin;
}

[RequireComponent(typeof(CharacterController))]
public class AndrewsCharacontroller : MonoBehaviour
{
    public FPSControllerSettings movementSettings;

    private Vector3 velocity;
    public bool running = false;
    private CharacterController characterController;
    private Camera cam;
    private float cameraAngle;

    private Vector3 startPos;

    private RaycastHit hitInfo;

    private int currentJumps;

    // inputs
    float xAxis = 0;
    float yAxis = 0;
    int jumpAxis = 0;
    bool jumpHeld = false;

    [SerializeField]
    private bool isGrounded;

    void Awake()
    {
        // get component references
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;

        snapToFloor();
    }

    void FixedUpdate()
    {
        isGrounded = characterController.isGrounded;

        // lock and hide mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // update inputs
        updateInputs();

        // update movement
        updateMovement();

        // update camera
        updateCamera();
    }

    void updateInputs()
    {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        jumpAxis = (int)Input.GetAxisRaw("Jump");

        running = Input.GetAxisRaw("Run") == 1;
    }

    void updateMovement()
    {


        // get normalized horizontal and vertical inputs
        Vector2 normalizedXYInput = new Vector2(xAxis, yAxis).normalized * (running ? movementSettings.maxRunSpeed : movementSettings.maxWalkSpeed);

        velocity.x = normalizedXYInput.x;
        velocity.z = normalizedXYInput.y;

        if (characterController.isGrounded)
        {
            //currentJumps = movementSettings.jumpCount;

            velocity.y = -0.1f;
        }
        else
        {
            if (velocity.y > movementSettings.maxFallSpeed)
            {
                // regular gravity
                velocity.y += Physics.gravity.y * movementSettings.gravityScale * Time.fixedDeltaTime;

                // fall speed multiplied gravity
                if (velocity.y < 0)
                {
                    velocity.y += Physics2D.gravity.y * movementSettings.fallSpeedMultiplier * Time.fixedDeltaTime;
                }

                if (jumpAxis == 0 && velocity.y > 0)
                {
                    velocity.y += Physics2D.gravity.y * movementSettings.lowJumpFallSpeedMultiplier * Time.fixedDeltaTime;
                }
            }
        }

        // update jump held
        //if (!jumpHeld && jumpAxis == 1)
        //{
        //    jumpHeld = true;

        //    if (currentJumps > 0)
        //    {
        //        velocity.y = movementSettings.jumpForce;
        //        currentJumps--;
        //    }
        //}
        //if (jumpHeld && jumpAxis == 0)
        //{
        //    jumpHeld = false;
        //}

        // slide with jump button
        if(jumpAxis == 1 && characterController.isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
            {
                velocity += hitInfo.normal;
                if (Physics.Raycast(transform.position + velocity, Vector3.down, out hitInfo))
                {
                    velocity.y -= 100;
                }
            }
        }

        // move the character controller
        characterController.Move(transform.TransformDirection(velocity) * Time.fixedDeltaTime);
    }

    void updateCamera()
    {
        // get mouse x and y
        float mouseX = Input.GetAxis("Camera X") * movementSettings.lookSensitivity;
        float mouseY = Input.GetAxis("Camera Y") * movementSettings.lookSensitivity;

        // rotate character with mouse x
        transform.eulerAngles += Vector3.up * mouseX;

        // rotate camera angle with mouse Y
        cameraAngle -= mouseY;

        // clamp camera angle between min and max
        cameraAngle = Mathf.Clamp(cameraAngle, movementSettings.cameraLimitMin, movementSettings.cameraLimitMax);

        cam.transform.localEulerAngles = new Vector3(cameraAngle, cam.transform.localEulerAngles.y, 0);
    }

    // snap to the closest surface downwards
    void snapToFloor()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            transform.position = hitInfo.point + Vector3.up * (characterController.height / 2);
            startPos = transform.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn")
        {
            transform.position = startPos;
        }
    }


}
