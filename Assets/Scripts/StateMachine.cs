using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Investigating,
        Chasing,
        Attack,
        Captured,
    }

    public State state;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

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

    IEnumerator PatrolState()
    {//Setup / Entry point
        Debug.Log("Entering Patrol State");

        while (state == State.Patrol) //"Update loop"
        {
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

    IEnumerator ChasingState()
    {//Setup / Entry point
        Debug.Log("Entering Chasing State");

        while (state == State.Chasing) //"Update loop"
        {
            float wave = Mathf.Sin(Time.time * 20f) * 0.1f + 1f;
            float wave2 = Mathf.Cos(Time.time * 20f) * 0.1f + 1f;

            transform.localScale = new Vector3(wave, wave2, wave);

            float shimmy = Mathf.Cos(Time.time * 30f) * 0.9f + 0.3f;

            //Choose transform movement or rigidbody movement
            //transform.position += transform.right * shimmy * Time.deltaTime;
            
            rb.AddForce(Vector3.right * shimmy);

            yield return null; //Wait for a frame
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
            yield return null; //Wait for a frame
        }

        //Exit point/ Tear down
        Debug.Log("Exiting Captured State");

        NextState();
    }
}