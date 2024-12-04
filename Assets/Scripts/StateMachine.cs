using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Video;

public class StateMachine : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;

    public enum State
    {
        Patrol,
        Investigating,
        Chasing,
        Attack,
        Captured,
    }

    public State state;

    protected Rigidbody rb;

    protected PlayerController player;

    public Renderer rend;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
    }

    protected void Start()
    {
        NextState();
    }

    protected void NextState()
    {
        switch (state)
        {
            case State.Patrol:
                StartCoroutine(PatrolState());
                break;
            case State.Investigating:
                StartCoroutine(InvestigatingState());
                break;
            case State.Chasing:
                StartCoroutine(ChasingState());
                break;
            case State.Attack:
                StartCoroutine(AttackState());
                break;
            case State.Captured:
                StartCoroutine(CapturedState());
                break;
            default:
                Debug.LogError("State does not exist");
                break;
        }
    }

    protected virtual IEnumerator PatrolState()
    {//Setup / Entry point
        Debug.Log("Entering Patrol State");

        while (state == State.Patrol) //"Update loop"
        {
            rend.material.color = Color.blue;

            transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);

            //Direction from A to B is... B - A
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();

            //Dot product parameters should be "Normalised"
            float result = Vector3.Dot(transform.forward, directionToPlayer);

            if (result >= 0.95f)
            {
                state = State.Chasing;
            }

            yield return null; //Wait for a frame
        }

        //Exit point/ Tear down
        Debug.Log("Exiting Patrol State");

        NextState();
    }

    IEnumerator InvestigatingState()
    {
        //Setup / Entry point
        Debug.Log("Entering Investigating State");

        float startTime = Time.time;
        float deltaSum = 0;

        while (state == State.Investigating) //"Update loop"
        {
            rend.material.color = Color.green;

            deltaSum += Time.deltaTime;
            yield return null; //Wait for a frame
        }

        float endTime = Time.time - startTime;
        Debug.Log("Delta sum = " + deltaSum + " | End time = " + endTime);

        //Exit point/ Tear down
        Debug.Log("Exiting Investigating State");

        NextState();
    }

    protected virtual IEnumerator ChasingState()
    {//Setup / Entry point
        Debug.Log("Entering Chasing State");

        while (state == State.Chasing) //"Update loop"
        {
            rend.material.color = Color.red;

            float wave = Mathf.Sin(Time.time * 20f) * 0.1f + 1f;
            float wave2 = Mathf.Cos(Time.time * 20f) * 0.1f + 1f;

            transform.localScale = new Vector3(wave, wave2, wave);

            float shimmy = Mathf.Cos(Time.time * 30f) * minSpeed + maxSpeed;

            //Choose transform movement or rigidbody movement
            //transform.position += transform.right * shimmy * Time.deltaTime;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            //directionToPlayer.Normalize();

            float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

            if (angle > 0)
            {
                transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);
            }

            else
            {
                transform.rotation *= Quaternion.Euler(0f, -50f * Time.deltaTime, 0f);
            }

            rb.AddForce(transform.forward * shimmy, ForceMode.Acceleration);

            if (directionToPlayer.magnitude < 2f)
            {
                state = State.Attack;
            }

            else if (directionToPlayer.magnitude > 10f)
            {
                state = State.Patrol;
            }

            yield return new WaitForFixedUpdate(); //Wait for the next fixed update
        }

        //Exit point/ Tear down
        Debug.Log("Exiting Chasing State");

        NextState();
    }

    IEnumerator AttackState()
    {//Setup / Entry point
        Debug.Log("Entering Attack State");

        while (state == State.Attack) //"Update loop"
        {
            rend.material.color = Color.red;

            Vector3 scale = transform.localScale;
            scale.z = Mathf.Cos(Time.time * 20f) * 0.1f + 1f;
            transform.localScale = scale;

            Vector3 directionToPlayer = transform.position - transform.position;
            if (directionToPlayer.magnitude > 3f)
            {
                state = State.Chasing;
            }

            yield return null; //Wait for a frame
        }

        //Exit point/ Tear down
        Debug.Log("Exiting Attack State");

        NextState();
    }

    IEnumerator CapturedState()
    {//Setup / Entry point
        Debug.Log("Entering Captured State");

        while (state == State.Captured) //"Update loop"
        {
            rend.material.color = Color.white;

            yield return null; //Wait for a frame
        }
    }

    public void Captured()
    {
        state = State.Captured;
    }
}