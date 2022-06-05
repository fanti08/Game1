using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    //VARIABLES
    [Header("MOVE")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float currentHorizontalSpeed;
    [SerializeField] private float currentVerticalSpeed;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isShifting;
    [SerializeField] private bool hasSmthAboveHead;
    [SerializeField] private bool wasGoingRight;
    [Range(1, 100)] public float maxSpeed;
    [Range(1, 100)] public float jumpForce;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private int horizontalInput;
    [SerializeField] private bool space;
    [SerializeField] private bool shift;
    [SerializeField] private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferCounter;
    [SerializeField] private Vector3 vel;
    [Range(1, 10)] public float crouchJump;
    [Range(1, 10)] public float crouchSpeed;
    [Range(1, 10)] public float jumpSpeed;
    [Range(1, 300)] public float accelSpeed;
    [Range(1, 300)] public float decelSpeed;
    [Range(.01f, 2)] public float coyoteAmount;
    [Range(.01f, 2)] public float bufferAmount;
    [Range(.01f, 2)] public float edgeAmount;
    [Range(1, 20)] public float gravityWhenJumping;
    [Header("ANIMATIONS")]
    public GameObject idle;
    public GameObject side;
    public Animator animator;
    [Header("DEBUG")]
    [SerializeField] private bool isDebugActive = false;
    [SerializeField] public Color colliderColor = Color.blue;
    [SerializeField] public Color jumpIndicatorColor = Color.yellow;
    [SerializeField] public Color edgeColor = Color.blue;
    [SerializeField] private Color hassmthAboveColor;
    [SerializeField] private Color isGroundedColor;
    [SerializeField] public KeyCode slowDownKey;
    [SerializeField] public KeyCode debugModeKey;
    [SerializeField] private List<JumpIndicator> jumpIndicators = new List<JumpIndicator>();


    //VOIDS
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        ground = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        DefineSpeeds();
        DefineKeys();
        Move();
        Jump();
        Crouch();
        CheckVelocities();

        Anims();

        Debug();
    }

    private void FixedUpdate()
    {
        EdgeCorrection();
    }


    #region MOVE

    private void DefineSpeeds()
    {
        currentHorizontalSpeed = rb.velocity.x;
        currentVerticalSpeed = rb.velocity.y;
        isGrounded = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, .1f, ground);
        isShifting = shift || (hasSmthAboveHead && idle.transform.localScale != new Vector3(1, 1, 1));
        if (currentHorizontalSpeed > .01f) wasGoingRight = true;
        if (currentHorizontalSpeed < -.01f) wasGoingRight = false;
        hasSmthAboveHead = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.up, 1.5f, ground);
    }
    private void DefineKeys()
    {
        horizontalInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        space = Input.GetKeyDown(KeyCode.Space) ? true : false;
        shift = Input.GetKey(KeyCode.LeftShift) ? true : false;
    }

    private void Move()
    {
        if (horizontalInput == 0) Decelerate();
        else Accelerate();
    }
    private void Accelerate()
    {
        float xVelToAdd;
        if (!isGrounded) xVelToAdd = Time.deltaTime * accelSpeed * horizontalInput / 1.5f;
        else xVelToAdd = Time.deltaTime * accelSpeed * horizontalInput;
        Vector2 speedToAccel = new Vector2(rb.velocity.x + xVelToAdd, currentVerticalSpeed);
        if (Mathf.Abs(currentHorizontalSpeed) < _maxSpeed) rb.velocity = speedToAccel;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -_maxSpeed, _maxSpeed), currentVerticalSpeed);
    }
    private void Decelerate()
    {
        int xDir = wasGoingRight ? 1 : -1;
        float xVelToReduce;
        if (!isGrounded) xVelToReduce = Time.deltaTime * decelSpeed * xDir / 1.5f;
        else xVelToReduce = Time.deltaTime * decelSpeed * xDir;
        Vector2 speedToDecel = new Vector2(rb.velocity.x - xVelToReduce, currentVerticalSpeed);
        if (Mathf.Abs(currentHorizontalSpeed) > 0) rb.velocity = speedToDecel;
        if (Mathf.Abs(currentHorizontalSpeed) < .05f) rb.velocity = new Vector2(0, currentVerticalSpeed);
    }

    private void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && currentVerticalSpeed <= 0)
        {
            jumpBufferCounter = 0;
            rb.velocity = new Vector2(currentHorizontalSpeed, _jumpForce);
            coyoteTimeCounter = 0;
        }

        JumpBuffer();
        CoyoteTime();
    }
    private void CoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteAmount;
            rb.gravityScale = 1;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            rb.gravityScale = gravityWhenJumping;
        }
    }
    private void JumpBuffer()
    {
        jumpBufferCounter -= Time.deltaTime;
        if (space) jumpBufferCounter = bufferAmount;
    }

    private void Crouch()
    {
        if (isShifting)
        {
            idle.transform.localScale = new Vector3(1, .5f, 1);
            idle.transform.localPosition = new Vector3(0, -5.25f - 7.5f / 6, 0);
            side.transform.localScale = new Vector3(1, .5f, 1);
            side.transform.localPosition = new Vector3(0, -5.25f - 7.5f / 6, 0);
            bc.size = new Vector2(6, 7.5f);
            bc.offset = new Vector2(0, -3.725f);
            _jumpForce = jumpForce / crouchJump;
        }
        else
        {
            idle.transform.localScale = new Vector3(1, 1, 1);
            idle.transform.localPosition = new Vector3(0, -5.25f, 0);
            side.transform.localScale = new Vector3(1, 1, 1);
            side.transform.localPosition = new Vector3(0, -5.25f, 0);
            bc.size = new Vector2(6, 15);
            bc.offset = new Vector2(0, 0);
            _jumpForce = jumpForce;
        }
    }

    private void CheckVelocities()
    {
        if (isGrounded && !isShifting) _maxSpeed = maxSpeed;
        if (!isGrounded && !isShifting) _maxSpeed = maxSpeed / jumpSpeed;
        if (isGrounded && isShifting) _maxSpeed = maxSpeed / crouchSpeed;
        if (!isGrounded && isShifting) _maxSpeed = maxSpeed / crouchSpeed / jumpSpeed;
    }

    private void EdgeCorrection()
    {
        Vector3 offset = new Vector3(bc.offset.x, bc.offset.y, transform.position.z) / 5;
        Vector3 ROrigin = offset + transform.position + new Vector3(bc.size.x / 10 + .05f, (bc.size.y / 10) + 0.25f, transform.position.z);
        Vector3 LOrigin = offset + transform.position + new Vector3(-bc.size.x / 10 - .05f, (bc.size.y / 10) + 0.25f, transform.position.z);

        Vector2 RDir = Vector2.left;
        Vector2 LDir = Vector2.right;
        float dist = (1 - edgeAmount) * (bc.size.x * .2f);

        RaycastHit2D Rhit = Physics2D.Raycast(ROrigin, RDir, dist, ground);
        RaycastHit2D Lhit = Physics2D.Raycast(LOrigin, LDir, dist, ground);

        if (Lhit.collider == null && Rhit.collider == null) vel = rb.velocity;
        if (!isGrounded && vel.y > .01f)
        {
            if (Lhit.collider != null && Rhit.collider == null && vel.x > -.3f)
            {
                transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
                rb.velocity = vel;
            }
            else if (Lhit.collider == null && Rhit.collider != null && vel.x < .3f)
            {
                transform.position = new Vector3(transform.position.x - .2f, transform.position.y, transform.position.z);
                rb.velocity = vel;
            }
        }
    }

    #endregion MOVE

    #region ANIMATIONS

    private void Anims()
    {
        if (horizontalInput == 0) animator.SetInteger("direction", 0);
        else animator.SetInteger("direction", horizontalInput);
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && currentVerticalSpeed <= 0) animator.SetTrigger("jump");
        animator.SetBool("inair", !isGrounded);
    }

    #endregion ANIMATIONS

    #region DEBUG

    class JumpIndicator
    {
        public Vector3 position;
        public float coolDowm;
        public JumpIndicator(Vector3 _position, float _coolDowm)
        {
            position = _position;
            coolDowm = _coolDowm;
        }
    }

    private void Debug()
    {
        if (Input.GetKeyDown(debugModeKey)) isDebugActive = !isDebugActive;
        if (!isDebugActive) Time.timeScale = 1;

        if (isDebugActive)
        {
            if (Input.GetKey(slowDownKey)) Time.timeScale = 0.25f;
            else Time.timeScale = 1;
            print("HORIZONTAL VELOCITY = " + ((float)Mathf.Round(rb.velocity.x * 100) / 100).ToString() + "VERTICAL VELOCITY = " + ((float)Mathf.Round(rb.velocity.y * 100) / 100).ToString());
        }

        if (Input.GetKeyDown(KeyCode.Space)) jumpIndicators.Add(new JumpIndicator(bc.bounds.center + Vector3.down * (bc.bounds.size.y / 2), 5));

        for (int i = 0; i < jumpIndicators.Count; i++)
        {
            jumpIndicators[i].coolDowm -= Time.deltaTime;
            if (jumpIndicators[i].coolDowm < 0) jumpIndicators.Remove(jumpIndicators[i]);
        }

        hassmthAboveColor = hasSmthAboveHead ? Color.green : Color.red;
        isGroundedColor = isGrounded ? Color.green : Color.red;
    }

    private void OnDrawGizmos()
    {
        if (isDebugActive)
        {
            Gizmos.color = isGroundedColor;
            Gizmos.DrawWireCube(bc.bounds.center + (bc.bounds.size.y / 2 + .04f / 2) * Vector3.down, new Vector2(bc.bounds.size.x, .04f));
            Gizmos.color = hassmthAboveColor;
            Gizmos.DrawWireCube(bc.bounds.center + (bc.bounds.size.y / 2 + 1.5f / 2) * Vector3.up, new Vector2(bc.bounds.size.x, 1.5f));

            Vector3 offset = new Vector3(bc.offset.x, bc.offset.y, transform.position.z) / 5;
            Vector3 ROrigin = offset + transform.position + new Vector3(bc.size.x / 10 + .005f, (bc.size.y / 10) + .3f, transform.position.z);
            Vector3 LOrigin = offset + transform.position + new Vector3(-bc.size.x / 10 - .005f, (bc.size.y / 10) + .3f, transform.position.z);

            Vector2 RDir = Vector2.left;
            Vector2 LDir = Vector2.right;

            Gizmos.color = edgeColor;
            Gizmos.DrawRay(ROrigin, RDir * edgeAmount);
            Gizmos.DrawRay(LOrigin, LDir * edgeAmount);

            Gizmos.color = jumpIndicatorColor;
            for (int i = 0; i < jumpIndicators.Count; i++)
            {
                Gizmos.DrawLine(jumpIndicators[i].position - jumpIndicators[i].coolDowm / 4 * Vector3.right, jumpIndicators[i].position + jumpIndicators[i].coolDowm / 4 * Vector3.right);
                Gizmos.DrawCube(jumpIndicators[i].position, new Vector3(0.4f, 0.2f, 0f));
            }

            Gizmos.color = colliderColor;
            Gizmos.DrawCube(bc.bounds.center, bc.bounds.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);
        }
    }

    #endregion DEBUG
}