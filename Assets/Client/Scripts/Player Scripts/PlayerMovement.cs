using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scripts.PlayerScripts
{
  public class PlayerMovement : MonoBehaviour
  {
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce, jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    public Transform orientation;



    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Start()
    {
      rb = GetComponent<Rigidbody>();
      rb.freezeRotation = true;
    }

    private void Update()
    {
      // Ground Check
      grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
      if (grounded)
        rb.drag = groundDrag;
      else
        rb.drag = 0;

      MyInput();
      SpeedControl();
    }

    private void MyInput()
    {
      horizontalInput = Input.GetAxisRaw("Horizontal");
      verticalInput = Input.GetAxisRaw("Vertical");

      if (Input.GetKey(jumpKey) && readyToJump && grounded)
      {
        readyToJump = false;
        Jump();
        Invoke(nameof(ResetJump), jumpCooldown);
      }
    }

    private void ResetJump()
    {
      readyToJump = true;
    }

    private void Jump()
    {
      rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
      rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void MovePlayer()
    {
      moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
      if (grounded)
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
      else
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void FixedUpdate()
    {
      MovePlayer();
    }

    private void SpeedControl()
    {
      Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
      if (flatVel.magnitude > moveSpeed)
      {
        Vector3 limitedVel = flatVel.normalized * moveSpeed;
        rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
      }
    }
  }
}