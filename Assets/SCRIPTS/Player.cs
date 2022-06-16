using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

public class Player : MonoBehaviour
{
    //VARIABLES
    public bool isDead;
    [Header("Keys")]
    [SerializeField] private KeyCode KEY_Drag = KeyCode.Mouse0;
    [SerializeField] private KeyCode KEY_Left = KeyCode.A;
    [SerializeField] private KeyCode KEY_Right = KeyCode.D;
    [SerializeField] private KeyCode KEY_Jump = KeyCode.Space;
    [SerializeField] private KeyCode KEY_Crouch = KeyCode.LeftShift;
    [SerializeField] private KeyCode KEY_Dash = KeyCode.W;
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
    [Range(1, 100)] [SerializeField] private float maxSpeed;
    [Range(1, 100)] [SerializeField] private float jumpForce;
    [Range(1, 100)] [SerializeField] private float dashSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private int horizontalInput;
    [SerializeField] private bool space;
    [SerializeField] private bool spaceDown;
    [SerializeField] private bool crouch;
    [SerializeField] private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferCounter;
    [SerializeField] private Vector3 vel;
    [Range(1, 20)] [SerializeField] private float crouchJump;
    [Range(1, 20)] [SerializeField] private float crouchSpeed;
    [Range(1, 20)] [SerializeField] private float jumpSpeed;
    [Range(1, 20)] [SerializeField] private float gravityWhenJumping;
    [Range(1, 300)] [SerializeField] private float accelSpeed;
    [Range(.01f, 2)] [SerializeField] private float decelSpeed;
    [Range(.01f, 2)] [SerializeField] private float coyoteAmount;
    [Range(.01f, 2)] [SerializeField] private float bufferAmount;
    [Range(.01f, 2)] [SerializeField] private float edgeAmount;
    [Range(.01f, 2)] [SerializeField] private float maxJumpTime;
    [Range(.01f, 2)] [SerializeField] private float dashCooldown;
    [SerializeField] private bool isDashing;
    [SerializeField] private int dashesCount;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashStartTime;
    [SerializeField] private Vector2 dashVel;
    [SerializeField] private float lastDashCooldownTime;
    [Header("GRAB & PICKUP")]
    [SerializeField] private BoxCollider2D stopper;
    [SerializeField] private float canGrabRadius;
    [SerializeField] private Color canGrabColor;
    [SerializeField] private Color cantGrabColor;
    [SerializeField] private bool objectGrabbed;
    [SerializeField] private Rigidbody2D grabbedObject;
    [SerializeField] private float grabForce;
    [SerializeField] private float grabbedObjectDrag;
    [SerializeField] private string draggable;
    [SerializeField] private string draggablecoll;
    [SerializeField] private List<GameObject> pickups;
    [SerializeField] private int maxPickups;
    [SerializeField] private string pickupable;
    [Header("ANIMATIONS")]
    [SerializeField] private Animator animator;
    [Header("DEBUG")]
    public bool isDebugActive = false;
    [SerializeField] private Color colliderColor = Color.blue;
    [SerializeField] private Color jumpIndicatorColor = Color.yellow;
    [SerializeField] private Color edgeColor = Color.blue;
    [SerializeField] private Color hassmthAboveColor;
    [SerializeField] private Color isGroundedColor;
    public KeyCode slowDownKey;
    [SerializeField] private KeyCode debugModeKey;
    [SerializeField] private List<JumpIndicator> jumpIndicators = new List<JumpIndicator>();


    //VOIDS
    private void Update()
    {
        DeadCheck();

        DefineSpeeds();
        DefineKeys();
        Move();
        Jump();
        Crouch();
        CheckVelocities();
        Dash();

        DragObjects();

        Anims();

        DebugMode();
    }

    private void FixedUpdate()
    {
        EdgeCorrection();
    }

    #region DEAD

    private void DeadCheck()
    {
        //print(isDead);
        //add here
    }

    #endregion DEAD

    #region MOVE

    private void DefineSpeeds()
    {
        currentHorizontalSpeed = rb.velocity.x;
        currentVerticalSpeed = rb.velocity.y;
        isGrounded = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, .1f, ground);
        isShifting = crouch || (hasSmthAboveHead && bc.size == new Vector2(6, 7.5f));
        if (currentHorizontalSpeed > .01f) 
            wasGoingRight = true;
        if (currentHorizontalSpeed < -.01f) 
            wasGoingRight = false;
        hasSmthAboveHead = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.up, 1.5f, ground);
    }
    private void DefineKeys()
    {
        if (Time.timeScale != 0)
        {
            horizontalInput = (Input.GetKey(KEY_Right) ? 1 : 0) - (Input.GetKey(KEY_Left) ? 1 : 0);
            space = Input.GetKey(KEY_Jump);
            spaceDown = Input.GetKeyDown(KEY_Jump);
            crouch = Input.GetKey(KEY_Crouch);
        }
    }

    private void Move()
    {
        if (horizontalInput == 0)
            Decelerate();
        else
            Accelerate();
    }
    private void Accelerate()
    {
        float xVelToAdd;
        if (!isGrounded) 
            xVelToAdd = Time.deltaTime * accelSpeed * horizontalInput / 1.5f;
        else 
            xVelToAdd = Time.deltaTime * accelSpeed * horizontalInput;
        Vector2 speedToAccel = new Vector2(rb.velocity.x + xVelToAdd, currentVerticalSpeed);
        if (Mathf.Abs(currentHorizontalSpeed) < _maxSpeed + 0.3f) 
            rb.velocity = speedToAccel;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -_maxSpeed, _maxSpeed), currentVerticalSpeed);
    }
    private void Decelerate()
    {
        float xVelToReduce = isGrounded ? 1 : 1.5f;
        Vector2 speedToDecel = new Vector2(rb.velocity.x * decelSpeed / xVelToReduce, currentVerticalSpeed);
        if (Mathf.Abs(currentHorizontalSpeed) > 0) 
            rb.velocity = speedToDecel;
        if (Mathf.Abs(currentHorizontalSpeed) < .05f) 
            rb.velocity = new Vector2(0, currentVerticalSpeed);
    }

    private void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && currentVerticalSpeed <= 0)
        {
            StartJumping();
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }

        JumpBuffer();
        CoyoteTime();
    }
    private async void StartJumping()
    {
        float lastTime = Time.time;
        while (space && lastTime + maxJumpTime > Time.time)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpForce);
            await Task.Yield();
        }

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
        if (spaceDown) 
            jumpBufferCounter = bufferAmount;
    }

    private void Crouch()
    {
        if (isShifting)
        { 
            bc.size = new Vector2(9, 7.5f);
            bc.offset = new Vector2(0, -3.725f);
            _jumpForce = jumpForce / crouchJump;
        }
        else
        {
            bc.size = new Vector2(9, 15);
            bc.offset = new Vector2(0, 0);
            _jumpForce = jumpForce;
        }
        
        stopper.offset = bc.offset;
        stopper.size = new Vector2(bc.size.x - .01f, bc.size.y - .01f);
    }

    private void CheckVelocities()
    {
        if (isGrounded && !isShifting) 
            _maxSpeed = maxSpeed;
        if (!isGrounded && !isShifting) 
            _maxSpeed = maxSpeed / jumpSpeed;
        if (isGrounded && isShifting) 
            _maxSpeed = maxSpeed / crouchSpeed;
        if (!isGrounded && isShifting) 
            _maxSpeed = maxSpeed / crouchSpeed / jumpSpeed;
    }

    private void EdgeCorrection()
    {
        Vector3 offset = new Vector3(bc.offset.x, bc.offset.y, transform.position.z) / 5;
        Vector3 ROrigin = offset + transform.position + new Vector3(bc.size.x / 10 + .05f, bc.size.y / 10 + 0.25f, transform.position.z);
        Vector3 LOrigin = offset + transform.position + new Vector3(-bc.size.x / 10 - .05f, bc.size.y / 10 + 0.25f, transform.position.z);

        Vector2 RDir = Vector2.left;
        Vector2 LDir = Vector2.right;
        float dist = (1 - edgeAmount) * (bc.size.x * .2f);

        RaycastHit2D Rhit = Physics2D.Raycast(ROrigin, RDir, dist, ground);
        RaycastHit2D Lhit = Physics2D.Raycast(LOrigin, LDir, dist, ground);

        Vector3 point1 = new Vector3(transform.position.x - .2f - bc.bounds.size.x / 2, transform.position.y - bc.bounds.size.y / 2 + .1f, transform.position.z) + offset;
        Vector3 point2 = new Vector3(transform.position.x + .2f + bc.bounds.size.x / 2, transform.position.y - bc.bounds.size.y / 2 + .1f, transform.position.z) + offset;
        Vector3 point3 = new Vector3(point1.x, bc.bounds.size.y / 2 + transform.position.y, transform.position.z) + offset;
        Vector3 point4 = new Vector3(point2.x, bc.bounds.size.y / 2 + transform.position.y, transform.position.z) + offset;

        RaycastHit2D Lray = Physics2D.Linecast(point1, point3, ground);
        RaycastHit2D Rray = Physics2D.Linecast(point2, point4, ground);

        if (Lhit.collider == null && Rhit.collider == null)
            vel = rb.velocity;
        if (!isGrounded && vel.y > .01f)
        {
            if (Lhit.collider != null && Rhit.collider == null && vel.x > -.3f && Rray.collider == null)
            {
                transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
                rb.velocity = vel;
            }
            else if (Lhit.collider == null && Rhit.collider != null && vel.x < .3f && Lray.collider == null)
            {
                transform.position = new Vector3(transform.position.x - .2f, transform.position.y, transform.position.z);
                rb.velocity = vel;
            }
        }
    }

    private void Dash()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();
        if (Input.GetKeyDown(KEY_Dash) && dashesCount > 0)
        {
            dashVel = dashSpeed * dir;
            isDashing = true;
            dashesCount = 0;
            dashStartTime = Time.time;
        }
        if (isDashing)
        {
            if (dashStartTime + dashDuration < Time.time)
            {
                isDashing = false;
                dashesCount = 0;
            }
            else
                rb.velocity = dashVel;
        }

        RecoverDash();
    }
    private void RecoverDash()
    {
        if (dashesCount < 1 && lastDashCooldownTime + dashCooldown < Time.time && isGrounded)
        {
            dashesCount = 1;
            lastDashCooldownTime = Time.time;
        }
    }


    #endregion MOVE

    #region DRAG & PICKUP

    private void DragObjects()
    {
        RaycastHit2D[] canGrabHits = Physics2D.CircleCastAll(transform.position, canGrabRadius, Vector2.zero, 0);
        GameObject[] draggables = GameObject.FindGameObjectsWithTag(draggable);
        for (int i = 0; i < draggables.Length; i++)
            draggables[i].GetComponentInParent<SpriteRenderer>().color = cantGrabColor;
        for (int i = 0; i < canGrabHits.Length; i++)
        {
            if (canGrabHits[i].collider.CompareTag(draggable))
            {
                Transform draggableObj = canGrabHits[i].transform;
                SpriteRenderer sprite = draggableObj.GetComponentInParent<SpriteRenderer>();

                sprite.color = canGrabColor;
            }
        }
        Rigidbody2D[] canGrabObjects = canGrabHits.Select(i => i.collider.GetComponent<Rigidbody2D>()).ToArray();

        if (Input.GetKeyDown(KEY_Drag))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction, Mathf.Infinity);
            if (hit.collider == null) 
                return;
            if (hit.collider.CompareTag(draggable))
            {
                grabbedObject = hit.collider.GetComponentInParent<Rigidbody2D>();
                if (!canGrabObjects.Contains(grabbedObject)) 
                    return;
                grabbedObject.drag = grabbedObjectDrag;
                grabbedObject.gravityScale = 0;
                objectGrabbed = true;
            }
        }
        if (!objectGrabbed) 
            return;
        if (Input.GetKey(KEY_Drag))
        {
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = targetPos - grabbedObject.position;
            grabbedObject.AddForce(dir * grabForce * Time.deltaTime * Vector2.Distance(targetPos, grabbedObject.position));
        }
        if (grabbedObject != null && Input.GetKeyUp(KEY_Drag))
        {
            grabbedObject.drag = 0;
            grabbedObject.gravityScale = 6;
            grabbedObject = null;
            objectGrabbed = false;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(pickupable) && pickups.Count < maxPickups)
        {
            pickups.Add(col.gameObject);
            Destroy(col.gameObject);
        }
    }

    #endregion DRAG & PICKUP

    #region ANIMATIONS

    private void Anims()
    {
        if (horizontalInput == 0) 
            animator.SetInteger("direction", 0);
        else 
            animator.SetInteger("direction", horizontalInput);
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && currentVerticalSpeed <= 0) 
            animator.SetTrigger("jump");
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

    private void DebugMode()
    {
        if (Input.GetKeyDown(debugModeKey)) 
            isDebugActive = !isDebugActive;

        if (isDebugActive)
            print("HORIZONTAL VELOCITY = " + ((float)Mathf.Round(rb.velocity.x * 100) / 100).ToString() + "VERTICAL VELOCITY = " + ((float)Mathf.Round(rb.velocity.y * 100) / 100).ToString());

        if (Input.GetKeyDown(KeyCode.Space)) 
            jumpIndicators.Add(new JumpIndicator(bc.bounds.center + Vector3.down * (bc.bounds.size.y / 2), 5));

        for (int i = 0; i < jumpIndicators.Count; i++)
        {
            jumpIndicators[i].coolDowm -= Time.deltaTime;
            if (jumpIndicators[i].coolDowm < 0) 
                jumpIndicators.Remove(jumpIndicators[i]);
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

            Vector3 point1 = new Vector3(transform.position.x - .2f - bc.bounds.size.x / 2, transform.position.y - bc.bounds.size.y / 2, transform.position.z) + offset;
            Vector3 point2 = new Vector3(transform.position.x + .2f + bc.bounds.size.x / 2, transform.position.y - bc.bounds.size.y / 2, transform.position.z) + offset;
            Vector3 point3 = new Vector3(point1.x, bc.bounds.size.y / 2 + transform.position.y, transform.position.z) + offset;
            Vector3 point4 = new Vector3(point2.x, bc.bounds.size.y / 2 + transform.position.y, transform.position.z) + offset;

            Gizmos.DrawLine(point1, point3);
            Gizmos.DrawLine(point2, point4);

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