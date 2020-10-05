using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    private Rigidbody playerRB;
    private Vector3 isoForward;// forward vector of the isometric view
    private Vector3 isoRight;// right vector of the isometric view

    [Header("Dash")]
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private int numberOfConsecutiveDashes = 2;
    [SerializeField] private float consecutiveDashBuffer = 0.8f;
    [SerializeField] private float dashCooldown = 2f;
    #region Input
    [Header("Input")]
    [Range(-1, 1)] [SerializeField] private float inputH;
    [Range(-1, 1)] [SerializeField] private float inputV;
    #endregion

    #region Checks
    private bool isGrounded;
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region Booleans
    public bool canMove = true;
    public bool canDash = true;
    public bool isDashing = false;
    public bool dashInCooldown = false;
    private bool dashRequested = false;
    private bool doDash = false;
    #endregion

    #region Auxiliars
    private float dashTimer = 0;
    private float consecutiveDashTimer = 0;
    private int dashCounter = 0;
    private Vector3 requestedDashDirection;
    #endregion

    public override void OnStartAuthority() //it is called when the client starts having authority over the player object
    {
        enabled = true;//enable this script on the instance
    }

    [ClientCallback]
    void Start()
    {
        SetIsometricDirections();//sets the isometric forward and right vectors
        playerRB = GetComponent<Rigidbody>();//gets the player's rigidbody
    }

    [ClientCallback]
    void Update()
    {
        /////Debugs/////
        //Debug.Log(dashCounter);
        ///////////////

        if (hasAuthority)
        {
            MyInput();
        }

        //Check if can dash
        if (dashInCooldown)
        {
            canDash = false;
        }

        //if requested dash, doDash = true
        if (dashRequested)
        {
            doDash = true;
            dashRequested = false;
        }

        //Consecutive Dashes Buffer
        if (dashCounter > 0)
        {
            consecutiveDashTimer += Time.deltaTime;
            if (consecutiveDashTimer >= 1)
            {
                dashCounter = 0;
                consecutiveDashTimer = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        Movement();

        if (canDash && doDash)
        {
            Dash();
        }
    }

    [Client]
    private void SetIsometricDirections()//uses the camera forward vector to set the isometric directions
    {
        isoForward = Camera.main.transform.forward;
        isoForward.y = 0;
        isoForward.Normalize();
        isoRight = Quaternion.Euler(0, 90, 0) * isoForward;
    }

    [Client]
    private void Movement()
    {
        Vector3 directionToMove = isoForward * inputV + isoRight * inputH;
        directionToMove = Vector3.ClampMagnitude(directionToMove, 1);
        if (isGrounded && canMove && !isDashing) // conditions for player to be able to move
        {
            playerRB.velocity = directionToMove * speed;
        }
    }


    [Client]
    private void Dash()
    {
        isDashing = true;
        Vector3 dashDirection = requestedDashDirection;
        dashDirection.y = 0;//make sure dash only happens in the arena playne
        dashDirection.Normalize();
        if (dashTimer < dashDuration)
        {
            dashTimer += Time.fixedDeltaTime;
            playerRB.velocity = dashDirection * dashSpeed;
        }
        else
        {
            StopDash();
            if (dashCounter == numberOfConsecutiveDashes)
            {
                StartCoroutine("DashCooldown");
            }
        }
    }

    private void StopDash()
    {
        playerRB.velocity = Vector3.zero;
        isDashing = false;
        doDash = false;
    }

    IEnumerator DashCooldown()
    {
        dashInCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        dashCounter = 0;
        dashInCooldown = false;
        canDash = true;
    }

    [Client]
    private void MyInput()//Function that will handle input
    {
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Dash"))
        {
            if (canDash && dashCounter < numberOfConsecutiveDashes)
            {
                dashRequested = true;
                if (consecutiveDashTimer < consecutiveDashBuffer)
                {
                    dashCounter++;
                    consecutiveDashTimer = 0;
                }
                requestedDashDirection = isoForward * inputV + isoRight * inputH;//dash direction based on player input when it is requested
                dashTimer = 0; // Reset dash timer each time request dash
            }
            else //if can't dash, send "error"
            {
                if (dashInCooldown)
                    throw new System.ArgumentException("dash is in cooldown");
                else
                    throw new System.ArgumentException("can't dash");
            }
        }
    }

    //Ground Checks
    [ClientCallback]
    private void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (groundLayer == (groundLayer | (1 << layer)))  //  returns true if the collision.contact layer is groundLayer
        {
            isGrounded = true;
        }
    }
    [ClientCallback]
    private void OnCollisionExit(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (groundLayer == (groundLayer | (1 << layer)))  //  returns true if the collision.contact layer is groundLayer
        {
            isGrounded = false;
        }
    }
}
