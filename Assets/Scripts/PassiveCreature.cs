using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Video;

public class PassiveCreature : StateMachine
{
    protected override IEnumerator PatrolState()
    {//Setup / Entry point
        Debug.Log("Entering Patrol State");

        while (state == State.Patrol) //"Update loop"
        {
            rend.material.color = Color.green;

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

    protected override IEnumerator ChasingState()
    {
        Debug.Log("Entering Chasing State");

        while (state == State.Chasing) //"Update loop"
        {
            rend.material.color = Color.yellow;

            float wave = Mathf.Sin(Time.time * 20f) * 0.1f + 1f;
            float wave2 = Mathf.Cos(Time.time * 20f) * 0.1f + 1f;

            transform.localScale = new Vector3(wave, wave2, wave);

            float shimmy = Mathf.Cos(Time.time * 30f) * minSpeed + maxSpeed;

            //Choose transform movement or rigidbody movement
            //transform.position += transform.right * shimmy * Time.deltaTime;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer = -directionToPlayer;
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

            if (directionToPlayer.magnitude > 10f)
            {
                state = State.Patrol;
            }

            yield return new WaitForFixedUpdate(); //Wait for the next fixed update
        }

        //Exit point/ Tear down
        Debug.Log("Exiting Chasing State");

        NextState();
    }
}