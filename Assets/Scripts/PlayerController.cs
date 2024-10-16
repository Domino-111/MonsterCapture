using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public float gravity;

    private bool hasCoyoted = false;

    private bool isGrounded;
    private float lastGroundedTime = float.NegativeInfinity;
    private float jumpInputTime = float.NegativeInfinity;

    public Rigidbody rb;
    public LayerMask groundMask;

    public Vector2 input;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Jump();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputTime = Time.time;
        }

        if (isGrounded || !hasCoyoted && (Time.time - lastGroundedTime) < 0.5f)
        {
            if ((Time.time - jumpInputTime) < 0.5f)
            {
                hasCoyoted = true;

                lastGroundedTime = float.NegativeInfinity;
                jumpInputTime = float.NegativeInfinity;

                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            }

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int goLayer = 1 << collision.gameObject.layer;

        if ((groundMask & goLayer) != 0)
        {
            isGrounded = true;

            lastGroundedTime = Time.time;
        }

        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int goLayer = 1 << collision.gameObject.layer;

        if ((groundMask & goLayer) != 0)
        {

            isGrounded = false;
        }
    }
}
