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

    Vector3 dampVelocity;
    Vector2 airDampVelocity;

    public float airControlMultiplier = 1.6f;
    public float maxSpeed = 10f;

    [SerializeField] private Camera camera;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (camera == null)
        {
            camera = Camera.main ? Camera.main : FindAnyObjectByType<Camera>(); //Nested ifelse
        }
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 inputTransformed = camera.transform.TransformDirection(input);
        inputTransformed.y = 0f;

        input = inputTransformed.normalized * input.magnitude;

        if (input.magnitude > 1)
        {
            input.Normalize();
        }
        input *= speed * Time.deltaTime;

        if (isGrounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(input.x, rb.velocity.y, input.z), ref dampVelocity, 0.1f);

            airDampVelocity = Vector2.zero;
        }

        else
        {
            dampVelocity = Vector3.zero;

            rb.AddForce(new Vector3(input.x, 0f, input.z) * airControlMultiplier, ForceMode.Acceleration);

            Vector2 xzMovement = new Vector2(rb.velocity.x, rb.velocity.z); //Creating new air movement only affecting our horizontal movement and not our vertical fall
            if (rb.velocity.magnitude > maxSpeed)
            {
                xzMovement = Vector2.SmoothDamp(xzMovement, xzMovement.normalized * maxSpeed, ref airDampVelocity, 0.1f); //Clamping the max speed of air speed

                rb.velocity = new Vector3(xzMovement.x, rb.velocity.y, xzMovement.y); //Implimenting new movement into controller
            }
        }
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
