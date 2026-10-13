/*using System.Collections;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Vector3 playerVelocity;
    [SerializeField] bool groundedPlayer;
    [SerializeField] float playerSpeed;
    [SerializeField] float gravityValue;
    [SerializeField] GameObject activeChar;
    [SerializeField] float moveHorizontal;
    [SerializeField] float moveVertical;
    [SerializeField] float speed = 1f;
    [SerializeField] float runMultiplier = 1.8f;
    [SerializeField] float rotateSpeed = 1f;
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] bool isJumping;
    // CHANGED: two independent fields (next to the other [SerializeField] lines)
    [SerializeField] float walkSpeed = 1f;
    [SerializeField] float runSpeed = 4f;

    Animator anim;
    string currentAnim = "";

    void Start()
    {
        playerSpeed = 5;
        gravityValue = -20;
        isJumping = false;      // CHANGED: never start in the "jumping" state

        if (controller == null) controller = GetComponent<CharacterController>();
        if (activeChar == null) activeChar = gameObject;
        anim = activeChar.GetComponentInChildren<Animator>();
    }

    void PlayAnim(string animName)
    {
        if (anim == null || animName == currentAnim) return;
        currentAnim = animName;

        Debug.Log("Anim -> " + animName);
        anim.CrossFade(animName, 0.15f);
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.Z)) v = 1f;
        else if (Input.GetKey(KeyCode.S)) v = -1f;
        if (Input.GetKey(KeyCode.Q)) h = -1f;
        else if (Input.GetKey(KeyCode.D)) h = 1f;

        bool isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
        bool isRunning = isMoving && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        controller.minMoveDistance = (isMoving || isJumping || !groundedPlayer) ? 0.001f : 0.901f;

        transform.Rotate(0, h * rotateSpeed, 0);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;   // CHANGED
        controller.SimpleMove(forward * (currentSpeed * v));

        // CHANGED: Space instead of E, GetKeyDown (one press = one jump), and no double jump
        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer && !isJumping)
        {
            isJumping = true;
            PlayAnim("jump");                       // CHANGED: same spelling as the Animator state
            playerVelocity.y += 10;
            StartCoroutine(ResetJump());
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (!isJumping)
        {
            if (isRunning) PlayAnim("run");
            else if (isMoving) PlayAnim("walk");
            else PlayAnim("breath");
        }
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.9f);
        isJumping = false;
    }
}*/
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject activeChar;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 1.5f;
    [SerializeField] float runSpeed = 4f;
    [SerializeField, Range(0.2f, 1f)] float backwardFactor = 0.6f;  // walking backwards is slower
    [SerializeField] float acceleration = 8f;                       // how fast the player reaches full speed
    [SerializeField] float deceleration = 12f;                      // how fast the player stops

    [Header("Rotation")]
    [SerializeField] float turnSpeed = 120f;                        // degrees per second
    [SerializeField] float turnSmoothing = 8f;                      // higher = snappier turning
    [SerializeField, Range(0.3f, 1f)] float runTurnFactor = 0.7f;   // wider turns while running

    [Header("Jump and gravity")]
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float gravityValue = -20f;
    [SerializeField] float coyoteTime = 0.1f;                       // jump still allowed just after leaving the ground

    [Header("Debug (read only)")]
    [SerializeField] bool groundedPlayer;
    [SerializeField] bool isJumping;
    [SerializeField] float currentSpeed;
    [SerializeField] float verticalVelocity;

    Animator anim;
    string currentAnim = "";
    float smoothH;
    float lastGroundedTime;
    float jumpStartTime;

    void Start()
    {
        if (gravityValue >= 0f) gravityValue = -20f;   // the Inspector may have kept an old value of 0
        isJumping = false;

        if (controller == null) controller = GetComponent<CharacterController>();
        if (activeChar == null) activeChar = gameObject;
        anim = activeChar.GetComponentInChildren<Animator>();

        controller.minMoveDistance = 0f;               // no more minMoveDistance trick
    }

    void PlayAnim(string animName)
    {
        if (anim == null || animName == currentAnim) return;
        currentAnim = animName;
        anim.CrossFade(animName, 0.15f);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // --- Ground check ---
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            lastGroundedTime = Time.time;
            if (verticalVelocity < 0f) verticalVelocity = -2f;   // small push keeps the player glued to the ground
            if (isJumping && Time.time > jumpStartTime + 0.15f) isJumping = false;   // landed
        }

        // --- Input (arrows through the axes, Z/S/Q/D read directly) ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.Z)) v = 1f;
        else if (Input.GetKey(KeyCode.S)) v = -1f;
        if (Input.GetKey(KeyCode.Q)) h = -1f;
        else if (Input.GetKey(KeyCode.D)) h = 1f;

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // --- Smooth rotation ---
        smoothH = Mathf.Lerp(smoothH, h, 1f - Mathf.Exp(-turnSmoothing * dt));
        float turn = turnSpeed * smoothH * dt;
        if (wantsToRun && Mathf.Abs(currentSpeed) > walkSpeed) turn *= runTurnFactor;
        transform.Rotate(0f, turn, 0f);

        // --- Smooth acceleration / deceleration ---
        float targetSpeed = v * (wantsToRun ? runSpeed : walkSpeed);
        if (v < 0f) targetSpeed *= backwardFactor;

        float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * dt);

        // --- Jump (Space), height driven by jumpHeight ---
        bool canJump = Time.time - lastGroundedTime <= coyoteTime;
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isJumping)
        {
            isJumping = true;
            jumpStartTime = Time.time;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            PlayAnim("jump");
        }

        // --- Gravity + a single Move() call ---
        verticalVelocity += gravityValue * dt;

        Vector3 move = transform.forward * currentSpeed;
        move.y = verticalVelocity;
        controller.Move(move * dt);

        // --- Animations, driven by the real (smoothed) speed ---
        if (!isJumping)
        {
            float absSpeed = Mathf.Abs(currentSpeed);
            float runThreshold = (walkSpeed + runSpeed) * 0.5f;

            if (absSpeed < 0.1f) PlayAnim("breath");
            else if (absSpeed > runThreshold) PlayAnim("run");
            else PlayAnim("walk");
        }
    }
}