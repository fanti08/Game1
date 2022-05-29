using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //VARIABLES
    [Header("COMPONENTS")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bc;
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
    [SerializeField] private bool A;
    [SerializeField] private bool D;
    [SerializeField] private bool space;
    [SerializeField] private bool shift;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float coyoteTime = .2f;
    [SerializeField] private float coyoteCounter;
    [Header("ANIMATIONS")]
    [SerializeField] private Animator anim;


    //VOIDS
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GameObject.Find("Sprites").GetComponent<Animator>();
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
        A = Input.GetKey(KeyCode.A) ? true : false;
        D = Input.GetKey(KeyCode.D) ? true : false;
        space = Input.GetKey(KeyCode.Space) ? true : false;
        shift = Input.GetKey(KeyCode.LeftShift) ? true : false;
    }

    private void Move()
    {
        if (!A && !D) Decelerate();
        if (A || D) Accelerate();
    }
    private void Accelerate()
    {
        Vector2 DspeedToAccel = new Vector2(rb.velocity.x + Time.deltaTime * 35, currentVerticalSpeed);
        Vector2 DspeedToAccelinAir = new Vector2(rb.velocity.x + Time.deltaTime * 35 / 1.5f, currentVerticalSpeed); 
        Vector2 AspeedToAccel = new Vector2(rb.velocity.x + Time.deltaTime * -35, currentVerticalSpeed);
        Vector2 AspeedToAccelinAir = new Vector2(rb.velocity.x + Time.deltaTime * -35 / 1.5f, currentVerticalSpeed);
        if (Mathf.Abs(currentHorizontalSpeed) < _maxSpeed)
        {
            if (A) rb.velocity = isGrounded ? AspeedToAccelinAir : AspeedToAccel;
            if (D) rb.velocity = isGrounded ? DspeedToAccelinAir : DspeedToAccel;
        }
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
            rb.velocity = new Vector2(currentHorizontalSpeed * 1.3f, _jumpForce);
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
            rb.gravityScale = 3;
        }
    }

    private void Crouch()
    {
        if (shift)
        {
            GameObject.Find("Sprites").transform.localScale = new Vector3(1, .5f, 1);
            GameObject.Find("Sprites").transform.localPosition = new Vector3(0, -5.125f - 7 / 6, 0);
            bc.size = new Vector2(6, 7.5f);
            _maxSpeed = maxSpeed / 4;
            _jumpForce = jumpForce / 1.5f;
            bc.offset = new Vector2(0, -3.725f);
        }
        else if (!hasSmthAboveHead)
        {
            GameObject.Find("Sprites").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("Sprites").transform.localPosition = new Vector3(0, -5.125f, 0);
            bc.size = new Vector2(6, 15);
            _maxSpeed = maxSpeed;
            _jumpForce = jumpForce;
            bc.offset = new Vector2(0, 0);
        }
    }

    #endregion MOVE
}