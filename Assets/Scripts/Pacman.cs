using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public float speed = 6.0f;

    [HideInInspector] public int mapX = 0;
    [HideInInspector] public int mapY = 0;

    BackPosition[] allOriginPos;
    Rigidbody[] allRigidbody;
    Vector3 movement;
    bool isMoving = false;
    bool isDead = false;

    void Start()
    {
        allRigidbody = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Rigidbody>();
        allOriginPos = this.gameObject.transform.GetChild(0).GetComponentsInChildren<BackPosition>();
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
                if (mapX == 19 && mapY == 9)
                {
                    // Warp Zone
                    transform.position = transform.position - new Vector3(17f, 0, 0);
                    mapX = 1;
                }
                else
                    MoveMotor(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !ScriptLocator.gamemanager.GetComponent<GameManager>().collisionMap[mapX - 1, mapY])
            {
                mapX--;
                if (mapX == 0 && mapY == 9)
                {
                    // Warp Zone
                    transform.position = transform.position + new Vector3(17f, 0, 0);
                    mapX = 18;
                }
                else
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

        // ESC: Quit Game
        if (Input.GetKey("escape")) Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if (other.GetComponent<Enemy>().enemyState == Enemy.EnemyState.Runaway)
            {
                other.gameObject.SendMessage("SetState", Enemy.EnemyState.Goback);
                ScriptLocator.gamemanager.GetComponent<GameManager>().SetScore(100);
            }
            else if (other.GetComponent<Enemy>().enemyState != Enemy.EnemyState.Goback)
            {
                this.gameObject.GetComponent<SphereCollider>().isTrigger = false;
                //                Rigidbody[] allCubes = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in allRigidbody)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    //                    rb.AddExplosionForce(50f, transform.position, 0.6f, 0.5f);
                    rb.AddExplosionForce(400f, transform.position, 1f, 1f);
                    speed = 0f;
//                    Destroy(gameObject, 3.0f);
                }

                if (!isDead)
                {
                    isDead = true;
                    ScriptLocator.gamemanager.GetComponent<GameManager>().numLife--;
                }

                if (ScriptLocator.gamemanager.GetComponent<GameManager>().numLife>0)
                    StartCoroutine(Revival());
            }
        }
    }

    IEnumerator Revival()
    {
        yield return new WaitForSeconds(3.0f);

        foreach (Rigidbody rb in allRigidbody)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        foreach (BackPosition bp in allOriginPos)
            bp.back = true;

        yield return new WaitForSeconds(3.0f);

        ScriptLocator.gamemanager.GetComponent<GameManager>().SpawnPacman();
        Destroy(gameObject);
    }
}
