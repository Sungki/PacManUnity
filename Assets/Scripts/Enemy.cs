using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float startChasing = 5.0f;
    public float speed = 4.0f;

    [HideInInspector] public int mapX = 0;
    [HideInInspector] public int mapY = 0;
    [HideInInspector] public Color color;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Runaway,
        Goback
    }
    public EnemyState enemyState;

    Renderer[] allRend;
    BackPosition[] allOriginPos;
    Rigidbody[] allRigidbody;
    bool isNewState = false;
    bool isMoving = false;

    void Start()
    {
        allRigidbody = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Rigidbody>();
        allOriginPos = this.gameObject.transform.GetChild(0).GetComponentsInChildren<BackPosition>();
        allRend = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in allRend)
            rend.material.color = color;

        SetState(EnemyState.Patrol);
        StartCoroutine(FSMMain());
    }

    void SetState(EnemyState newState)
    {
        isNewState = true;
        enemyState = newState;
    }

    IEnumerator FSMMain()
    {
        while (true)
        {
            isNewState = false;
            yield return StartCoroutine(enemyState.ToString());
        }
    }

    IEnumerator Goback()
    {
        foreach (Rigidbody rb in allRigidbody)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddExplosionForce(50f, transform.position, 0.6f, 0.5f);
            speed = 0f;
        }
        yield return new WaitForSeconds(3.0f);

        foreach (Rigidbody rb in allRigidbody)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        foreach (BackPosition bp in allOriginPos)
        {
            bp.back = true;
        }

        yield return new WaitForSeconds(3.0f);

        ScriptLocator.gamemanager.GetComponent<GameManager>().SpawnEnemy(color);
        Destroy(gameObject);
    }

    IEnumerator Runaway()
    {
        float timer = 0.0f;
        int countWait = 0;
        float colorTime = 0.25f;

        foreach (Renderer rend in allRend)
        {
            rend.material.color = Color.blue;
        }

        do
        {
            yield return null;
            if (isNewState) break;
            // Action
            timer += Time.deltaTime;

            if (timer > colorTime)
            {
                countWait++;
                timer = 0.0f;
                if (countWait%2==0)
                {
                    foreach (Renderer rend in allRend)
                        rend.material.color = Color.blue;
                }
                else
                {
                    foreach (Renderer rend in allRend)
                        rend.material.color = Color.white;
                }
            }

            if (countWait>32)
            {
                timer = 0.0f;
                countWait = 0;
                SetState(EnemyState.Patrol);
            }

            if (!isMoving)
            {
                Vector2 pos = RandomPosition();
                PathFinding((int)pos.x, (int)pos.y);
            }

        } while (!isNewState);

        foreach (Renderer rend in allRend)
        {
            rend.material.color = color;
        }
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
                if (!isMoving)
                {
                    PathFinding(ScriptLocator.pacman.GetComponent<Pacman>().mapX, ScriptLocator.pacman.GetComponent<Pacman>().mapY);
                }

                if (Vector3.Distance(transform.position, ScriptLocator.pacman.transform.position) > startChasing)
                {
                    SetState(EnemyState.Patrol);
                }
            }
        } while (!isNewState);
    }

    IEnumerator Patrol()
    {
        int countMove = 0;
        Vector2 pos = Vector2.zero;

        do
        {
            yield return null;
            if (isNewState) break;
            // Action
            if (ScriptLocator.pacman != null)
            {
                if (!isMoving)
                {
                    countMove++;
                    if(countMove >= 5)
                    {
                        countMove = 0;
                        pos = RandomPosition();
                    }
                    PathFinding((int)pos.x, (int)pos.y);
                }

                if (Vector3.Distance(transform.position, ScriptLocator.pacman.transform.position) <= startChasing)
                {
                    SetState(EnemyState.Chase);
                }
            }
        } while (!isNewState);
    }

    void PathFinding(int _x, int _y)
    {
        if (_x > mapX && _y == mapY) WalkRight();
        else if (_x < mapX && _y == mapY) WalkLeft();
        else if (_x == mapX && _y < mapY) WalkUp();
        else if (_x == mapX && _y > mapY) WalkDown();
        else if (_x > mapX && _y > mapY) WalkRightDown();
        else if (_x < mapX && _y < mapY) WalkLeftUp();
        else if (_x > mapX && _y < mapY) WalkRightUp();
        else if (_x < mapX && _y > mapY) WalkLeftDown();
        else
        {
            if(enemyState!= EnemyState.Runaway)
                SetState(EnemyState.Patrol);
        }
    }

    void MoveMotor(Vector3 _direction)
    {
        if (_direction == Vector3.right) transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (_direction == Vector3.left) transform.rotation = Quaternion.Euler(0, -90, 0);
        else if (_direction == Vector3.forward) transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_direction == Vector3.back) transform.rotation = Quaternion.Euler(0, -180, 0);

        Vector3 movement = transform.position + _direction;
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

    Vector2 RandomPosition()
    {
        bool isPossition = false;
        int randX = 0;
        int randY = 0;

        do
        {
            randX = Random.Range(1, 20);
            randY = Random.Range(1, 20);

            if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[randX, randY])
            {
                isPossition = true;
            }
        } while (!isPossition);

        return new Vector2(randX, randY);
    }

    void WalkRight()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX + 1, mapY])
        {
            // walk on right first
            mapX++;
            MoveMotor(Vector3.right);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY + 1])
        {
            // if not, walk on down
            mapY++;
            MoveMotor(Vector3.back);
        }
        else
        {
            // if not, walk on up
            mapY--;
            MoveMotor(Vector3.forward);
        }
    }

    void WalkLeft()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
        {
            // walk on left first
            mapX--;
            MoveMotor(Vector3.left);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY - 1])
        {
            // if not, walk on up
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else
        {
            // if not, walk on down
            mapY++;
            MoveMotor(Vector3.back);
        }
    }

    void WalkUp()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY - 1])
        {
            // walk on up first
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
        {
            // if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
        else
        {
            // if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
    }

    void WalkDown()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY + 1])
        {
            // walk on down first
            mapY++;
            MoveMotor(Vector3.back);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX + 1, mapY])
        {
            // if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
        else
        {
            //if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
    }

    void WalkLeftUp()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY - 1])
        {
            // walk on up first
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
        {
            // if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
        else if(!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX + 1, mapY])
        {
            // if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
        else
        {
            // if not, walk on down
            mapY++;
            MoveMotor(Vector3.back);
        }
    }
    void WalkRightUp()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY - 1])
        {
            // walk on up first
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX + 1, mapY])
        {
            // if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY + 1])
        {
            // if not, walk on down
            mapY++;
            MoveMotor(Vector3.back);
        }
        else
        {
            // if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
    }

    void WalkRightDown()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY + 1])
        {
            // walk on down first
            mapY++;
            MoveMotor(Vector3.back);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX + 1, mapY])
        {
            // if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
        {
            // if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
        else
        {
            //if not, walk on up
            mapY--;
            MoveMotor(Vector3.forward);
        }
    }

    void WalkLeftDown()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY + 1])
        {
            // walk on down first
            mapY++;
            MoveMotor(Vector3.back);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
        {
            // if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY - 1])
        {
            // if not, walk on up
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else
        {
            //if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
    }

    /*void BasicMove()
    {
        if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY - 1])
        {
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
        {
            mapX--;
            MoveMotor(Vector3.left);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX + 1, mapY])
        {
            mapX++;
            MoveMotor(Vector3.right);
        }
        else if (!ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY + 1])
        {
            mapY++;
            MoveMotor(Vector3.back);
        }
    }
        void MoveDirection(int _direction)
        {
            if (_direction == 0 && !MakeLevel.collisionMap[mapX + 1, mapY])
            {
                mapX++;
                MoveMotor(Vector3.right);
            }
            else if (_direction == 1 && !MakeLevel.collisionMap[mapX - 1, mapY])
            {
                mapX--;
                MoveMotor(Vector3.left);
            }
            else if (_direction == 2 && !MakeLevel.collisionMap[mapX, mapY - 1])
            {
                mapY--;
                MoveMotor(Vector3.forward);
            }
            else if (_direction == 3 && !MakeLevel.collisionMap[mapX, mapY + 1])
            {
                mapY++;
                MoveMotor(Vector3.back);
            }
        }*/
}
