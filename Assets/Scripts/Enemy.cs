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

    [SerializeField] private LayerMask layerMask;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Runaway,
        Goback
    }

    Renderer rend;
    public EnemyState enemyState;
    bool isNewState = false;
    bool isMoving = false;
//    Vector3 movement;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = color;
//        movement = Vector3.zero;
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
        rend.material.color = Color.white;
        speed = 8.0f;
        yield return new WaitForSeconds(1.0f);

        do
        {
            yield return null;
            if (isNewState) break;

            if (!isMoving)
            {
                PathFinding(10, 9);
            }

        } while (!isNewState);
    }

    IEnumerator Runaway()
    {
        float timer = 0.0f;
        float waitingTime = 8f;
        rend.material.color = Color.blue;

        do
        {
            yield return null;
            if (isNewState) break;
            // Action
            timer += Time.deltaTime;

            if (timer > waitingTime)
            {
                SetState(EnemyState.Patrol);
            }

            if (!isMoving)
            {
                Vector2 pos = RandomPosition();
                PathFinding((int)pos.x, (int)pos.y);
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
        rend.material.color = color;
        speed = 4.0f;

        do
        {
            yield return null;
            if (isNewState) break;
            // Action
            if (ScriptLocator.pacman != null)
            {
                if(!isMoving)
                {
                    Vector2 pos = RandomPosition();
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
            SetState(EnemyState.Patrol);
        }
    }

    void MoveMotor(Vector3 _direction)
    {
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
            randY = Random.Range(1, 16);
            if (!MakeLevel.collisionMap[randX, randY])
            {
                isPossition = true;
            }
        } while (!isPossition);

        return new Vector2(randX, randY);
    }

    void WalkRight()
    {
        print("WalkRight");
        if (!MakeLevel.collisionMap[mapX + 1, mapY])
        {
            // walk on right first
            mapX++;
            MoveMotor(Vector3.right);
        }
        else if (!MakeLevel.collisionMap[mapX, mapY + 1])
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
        print("WalkLeft");
        if (!MakeLevel.collisionMap[mapX - 1, mapY])
        {
            // walk on left first
            mapX--;
            MoveMotor(Vector3.left);
        }
        else if (!MakeLevel.collisionMap[mapX, mapY - 1])
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
        print("WalkUp");
        if (!MakeLevel.collisionMap[mapX, mapY - 1])
        {
            // walk on up first
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else if (!MakeLevel.collisionMap[mapX - 1, mapY])
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
        print("WalkDown");
        if (!MakeLevel.collisionMap[mapX, mapY + 1])
        {
            // walk on down first
            mapY++;
            MoveMotor(Vector3.back);
        }
        else if (!MakeLevel.collisionMap[mapX + 1, mapY])
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
        print("WalkLeftUp");
        if (!MakeLevel.collisionMap[mapX, mapY - 1])
        {
            // walk on up first
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else if (!MakeLevel.collisionMap[mapX - 1, mapY])
        {
            // if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
        else if(!MakeLevel.collisionMap[mapX + 1, mapY])
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
        print("WalkRightUp");
        if (!MakeLevel.collisionMap[mapX, mapY - 1])
        {
            // walk on up first
            mapY--;
            MoveMotor(Vector3.forward);
        }
        else if (!MakeLevel.collisionMap[mapX + 1, mapY])
        {
            // if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
        else if (!MakeLevel.collisionMap[mapX, mapY + 1])
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
        print("WalkRightDown");
        if (!MakeLevel.collisionMap[mapX, mapY + 1])
        {
            // walk on down first
            mapY++;
            MoveMotor(Vector3.back);
        }
        else if (!MakeLevel.collisionMap[mapX + 1, mapY])
        {
            // if not, walk on right
            mapX++;
            MoveMotor(Vector3.right);
        }
        else if (!MakeLevel.collisionMap[mapX - 1, mapY])
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
        print("WalkLeftDown");
        if (!MakeLevel.collisionMap[mapX, mapY + 1])
        {
            // walk on down first
            mapY++;
            MoveMotor(Vector3.back);
        }
        else if (!MakeLevel.collisionMap[mapX - 1, mapY])
        {
            // if not, walk on left
            mapX--;
            MoveMotor(Vector3.left);
        }
        else if (!MakeLevel.collisionMap[mapX, mapY - 1])
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

    /*    void MoveDirection(int _direction)
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
