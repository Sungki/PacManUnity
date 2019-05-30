using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float startChasing;
    public float speed = 5.0f;

    [HideInInspector] public int mapX = 0;
    [HideInInspector] public int mapY = 0;
    [HideInInspector] public Color color;

    [SerializeField] private LayerMask layerMask;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Runaway
    }

    Renderer rend;
    Rigidbody myRigidbody;
    public EnemyState enemyState;
    RaycastHit hitInfo = new RaycastHit();
    bool isNewState = false;
    Vector3 movement;
    bool isMoving = false;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        rend.material.color = color;
        movement = Vector3.zero;
        SetState(EnemyState.Patrol);
        StartCoroutine(FSMMain());
    }

    void SetState(EnemyState newState)
    {
        isNewState = true;
        enemyState = newState;
    }

//    void Update()
//    {
//    }

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
/*                transform.LookAt(ScriptLocator.pacman.transform);
                Physics.Raycast(transform.position + transform.forward*1.0f, ScriptLocator.pacman.transform.position - transform.position, out hitInfo, layerMask);

                if (hitInfo.transform == ScriptLocator.pacman.transform)
                {
                    myRigidbody.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
//                    transform.position = Vector3.MoveTowards(transform.position, pacman.transform.position + transform.forward*1.0f, speed * Time.deltaTime);
                }*/

                if (Vector3.Distance(transform.position, ScriptLocator.pacman.transform.position) > startChasing)
                {
                    SetState(EnemyState.Patrol);
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
                //                myRigidbody.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

                if (!isMoving && !MakeLevel.collisionMap[mapX - 1, mapY])
                {
                    mapX--;
                    MoveMotor(Vector3.left);
                }
            }
        } while (!isNewState);
    }

    void MoveMotor(Vector3 _direction)
    {
        movement = transform.position + _direction;
        StartCoroutine(Movement(movement));
    }

    IEnumerator Movement(Vector3 _dest)
    {
        isMoving = true;
        do
        {
            yield return null;
            if (transform.position == _dest)
            {
                isMoving = false;
                break;
            }
            transform.position = Vector3.MoveTowards(transform.position, _dest, speed * Time.deltaTime);

        } while (isMoving);
    }
}
