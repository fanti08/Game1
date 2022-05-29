using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //VARIABLES
    [Header("COMPONENTS")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private LayerMask ground;
    [Header("MOVE")]
    [SerializeField] private float currentHorizontalSpeed;
    [SerializeField] private float currentVerticalSpeed;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool hasSmthAboveHead;
    [SerializeField] private bool wasGoingRight;
    [Range(1, 100)] public int maxSpeed;
    [Range(1, 100)] public float jumpForce;
    [SerializeField] private int _maxSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float horizontalInput;
    [SerializeField] private bool space;
    [SerializeField] private bool shift;
    [SerializeField] private float coyoteTime = .2f;
    [SerializeField] private float coyoteCounter;
    [Header("ANIMATIONS")]
    [SerializeField] public GameObject idle;
    [SerializeField] public GameObject side;


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
    }

    #region MOVE

    private void DefineSpeeds()
    {
        currentHorizontalSpeed = rb.velocity.x;
        currentVerticalSpeed = rb.velocity.y;
        isGrounded = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, .02f, ground);
        if (currentHorizontalSpeed > 0) wasGoingRight = true;
        if (currentHorizontalSpeed < 0) wasGoingRight = false;
        hasSmthAboveHead = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.up, .75f, ground);
    }

    private void DefineKeys()
    {
        horizontalInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        space = Input.GetKey(KeyCode.Space) ? true : false;
        shift = Input.GetKey(KeyCode.LeftShift) ? true : false;
    }

    private void Move()
    {
        if (horizontalInput == 0) Decelerate();
        else Accelerate();
    }
    private void Accelerate()
    {
        Vector2 DspeedToAccel = new Vector2(rb.velocity.x + Time.deltaTime * 35, currentVerticalSpeed);
        Vector2 DspeedToAccelinAir = new Vector2(rb.velocity.x + Time.deltaTime * 35 / 1.5f, currentVerticalSpeed);
        Vector2 AspeedToAccel = new Vector2(rb.velocity.x + Time.deltaTime * -35, currentVerticalSpeed);
        Vector2 AspeedToAccelinAir = new Vector2(rb.velocity.x + Time.deltaTime * -35 / 1.5f, currentVerticalSpeed);
        if (Mathf.Abs(currentHorizontalSpeed) < _maxSpeed)
        {
            if (horizontalInput < 0) rb.velocity = isGrounded ? AspeedToAccelinAir : AspeedToAccel;
            if (horizontalInput > 0) rb.velocity = isGrounded ? DspeedToAccelinAir : DspeedToAccel;
        }
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -_maxSpeed, _maxSpeed), currentVerticalSpeed);
        if (currentHorizontalSpeed > _maxSpeed - .05f) rb.velocity = new Vector2(_maxSpeed, currentVerticalSpeed);
        if (currentHorizontalSpeed < -_maxSpeed + .05f) rb.velocity = new Vector2(-_maxSpeed, currentVerticalSpeed);
    }
    private void Decelerate()
    {
        Vector2 DspeedToDecel = new Vector2(rb.velocity.x - Time.deltaTime * 35, currentVerticalSpeed);
        Vector2 DspeedToDecelinAir = new Vector2(rb.velocity.x - Time.deltaTime * 35 / 1.5f, currentVerticalSpeed);
        Vector2 AspeedToDecel = new Vector2(rb.velocity.x + Time.deltaTime * 35, currentVerticalSpeed);
        Vector2 AspeedToDecelinAir = new Vector2(rb.velocity.x + Time.deltaTime * 35 / 1.5f, currentVerticalSpeed);
        if (Mathf.Abs(currentHorizontalSpeed) > 0)
        {
            if (!wasGoingRight) rb.velocity = isGrounded ? AspeedToDecelinAir : AspeedToDecel;
            else rb.velocity = isGrounded ? DspeedToDecelinAir : DspeedToDecel;
        }
        if (Mathf.Abs(currentHorizontalSpeed) < .05f) rb.velocity = new Vector2(0, currentVerticalSpeed);
    }

    private void Jump()
    {
        if (space && coyoteCounter > 0)
        {
            rb.velocity = new Vector2(currentHorizontalSpeed, _jumpForce);
            coyoteCounter = 0;
        }
        CoyoteTime();
    }
    private void CoyoteTime()
    {
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            rb.gravityScale = 1;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
            rb.gravityScale = 6;
        }
    }

    private void Crouch()
    {
        if (shift)
        {
            idle.transform.localScale = new Vector3(1, .5f, 1);
            idle.transform.localPosition = new Vector3(0, -5.125f - 7.5f / 6, 0);
            bc.size = new Vector2(6, 7.5f);
            bc.offset = new Vector2(0, -3.725f);
            _maxSpeed = maxSpeed / 4;
            _jumpForce = jumpForce / 1.3f;
        }
        else if (!hasSmthAboveHead)
        {
            idle.transform.localScale = new Vector3(1, 1, 1);
            idle.transform.localPosition = new Vector3(0, -5.125f, 0);
            bc.size = new Vector2(6, 15);
            bc.offset = new Vector2(0, 0);
            _maxSpeed = maxSpeed;
            _jumpForce = jumpForce;
        }
    }

    #endregion MOVE
}