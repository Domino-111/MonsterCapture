using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveCreature : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;

    public enum State
    {
        Patrol,
        Investigating,
        Fleeing,
        Captured,
    }

    public State state;

    Rigidbody rb;

    PlayerController player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        NextState();
    }

    void NextState()
    {
        switch (state)
        {
            case State.Patrol:
                StartCoroutine(PatrolState());
                break;
            case State.Investigating:
                StartCoroutine(InvestigatingState());
                break;
            case State.Fleeing:
                StartCoroutine(FleeingState());
                break;
            case State.Captured:
                StartCoroutine(CapturedState());
                break;
            default:
                Debug.LogError("State does not exist");
                break;
        }
    }

    IEnumerator PatrolState()
    {//Setup / Entry point
        Debug.Log("Entering Patrol State");

        while (state == State.Patrol) //"Update loop"
        {
            transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);

            //Direction from A to B is... B - A
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();

            //Dot product parameters should be "Normalised"
            float result = Vector3.Dot(transform.forward, directionToPlayer);

            if (result >= 0.95f)
            {
                state = State.Fleeing;
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
            deltaSum += Time.deltaTime;
            yield return null; //Wait for a frame
        }

        float endTime = Time.time - startTime;
        Debug.Log("Delta sum = " + deltaSum + " | End time = " + endTime);

        //Exit point/ Tear down
        Debug.Log("Exiting Investigating State");

        NextState();
    }

    IEnumerator FleeingState()
    {
        Debug.Log("Entering Fleeing State");

        while (state == State.Fleeing) //"Update loop"
        {
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

            else if (directionToPlayer.magnitude > 10f)
            {
                state = State.Patrol;
            }

            yield return new WaitForFixedUpdate(); //Wait for the next fixed update
        }

        Debug.Log("Exiting Fleeing State");
    }

    IEnumerator CapturedState()
    {//Setup / Entry point
        Debug.Log("Entering Captured State");

        while (state == State.Captured) //"Update loop"
        {
            yield return null; //Wait for a frame
        }

        //Exit point/ Tear down
        Debug.Log("Exiting Captured State");

        NextState();
    }
}
