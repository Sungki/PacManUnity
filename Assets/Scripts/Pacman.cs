using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public float speed = 6.0f;

    [HideInInspector] public int mapX = 0;
    [HideInInspector] public int mapY = 0;

    Vector3 movement;
    bool isMoving = false;

    void Start()
    {
        movement = Vector3.zero;
    }

    void MoveMotor(Vector3 _direction)
    {
        if(_direction==Vector3.right) transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (_direction == Vector3.left) transform.rotation = Quaternion.Euler(0, -90, 0);
        else if (_direction == Vector3.forward) transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_direction == Vector3.back) transform.rotation = Quaternion.Euler(0, -180, 0);

        movement = transform.position + _direction;
        StartCoroutine(Movement(movement));
    }

    IEnumerator Movement(Vector3 _dest)
    {
        isMoving = true;
        do
        {
            yield return null;
            if(transform.position == _dest)
            {
                isMoving = false;
                break;
            }
            transform.position = Vector3.MoveTowards(transform.position, _dest, speed * Time.deltaTime);

        } while (isMoving);
    }

    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.RightArrow) && !ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX + 1, mapY])
            {
                mapX++;
                MoveMotor(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
            {
                mapX--;
                MoveMotor(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.UpArrow) && !ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY - 1])
            {
                mapY--;
                MoveMotor(Vector3.forward);
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX, mapY + 1])
            {
                mapY++;
                MoveMotor(Vector3.back);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if (other.GetComponent<Enemy>().enemyState == Enemy.EnemyState.Runaway)
            {
                other.gameObject.SendMessage("SetState", Enemy.EnemyState.Goback);
                ScriptLocator.gamemanager.GetComponent<GameManager>().score += 100;
            }
            else if (other.GetComponent<Enemy>().enemyState != Enemy.EnemyState.Goback)
            {
                Rigidbody[] allCubes = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Rigidbody>();
                foreach(Rigidbody cube in allCubes)
                {
                    cube.isKinematic = false;
                    cube.useGravity = true;
                    cube.AddExplosionForce(500f, transform.position, 1f, 1f);
                    speed = 0f;
                    Destroy(gameObject, 3.0f);
                }
            }
        }
    }
}
