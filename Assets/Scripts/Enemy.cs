using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float startChasing;
    public float speed = 5.0f;

    [SerializeField] private LayerMask layerMask;

    enum EnemyState
    {
        Patrol,
        Chase,
        Runaway
    }

//    GameObject pacman;
    Rigidbody myRigidbody;
    EnemyState enemyState;
    RaycastHit hitInfo = new RaycastHit();
    bool isNewState = false;
 
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        SetState(EnemyState.Patrol);
//        pacman = GameObject.FindGameObjectWithTag("Pacman");
        StartCoroutine(FSMMain());
    }

    void SetState(EnemyState newState)
    {
        isNewState = true;
        enemyState = newState;
    }

    void Update()
    {
    }

    IEnumerator FSMMain()
    {
        while (true)
        {
            isNewState = false;
            yield return StartCoroutine(enemyState.ToString());
        }
    }

    IEnumerator Runaway()
    {
        float timer = 0.0f;
        float waitingTime = 5f;

        do
        {
            yield return null;
            if (isNewState) break;
            // Action
            timer += Time.deltaTime;

            if (timer > waitingTime)
            {
                SetState(EnemyState.Patrol);
                timer = 0;
            }
        } while (!isNewState);
    }

    IEnumerator Chase()
    {
        do
        {
            yield return null;
            if (isNewState) break;
            // Action
            if (ScriptLocator.pacman != null)
            {
                transform.LookAt(ScriptLocator.pacman.transform);
                Physics.Raycast(transform.position + transform.forward*1.0f, ScriptLocator.pacman.transform.position - transform.position, out hitInfo, layerMask);

                if (hitInfo.transform == ScriptLocator.pacman.transform)
                {
                    myRigidbody.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
//                    transform.position = Vector3.MoveTowards(transform.position, pacman.transform.position + transform.forward*1.0f, speed * Time.deltaTime);
                }
            }
            else
            {
                SetState(EnemyState.Patrol);
            }

        } while (!isNewState);
    }

    IEnumerator Patrol()
    {
        do
        {
            yield return null;
            if (isNewState) break;
            // Action
            if (ScriptLocator.pacman != null)
            {
                if (Vector3.Distance(transform.position, ScriptLocator.pacman.transform.position) <= startChasing)
                {
                    SetState(EnemyState.Chase);
                }
                myRigidbody.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
            }
        } while (!isNewState);
    }
}
