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
            if (Input.GetKey(KeyCode.RightArrow) && !MakeLevel.collisionMap[mapX + 1, mapY])
            {
                mapX++;
                MoveMotor(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !MakeLevel.collisionMap[mapX - 1, mapY])
            {
                mapX--;
                MoveMotor(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.UpArrow) && !MakeLevel.collisionMap[mapX, mapY - 1])
            {
                mapY--;
                MoveMotor(Vector3.forward);
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !MakeLevel.collisionMap[mapX, mapY + 1])
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
            Destroy(gameObject);
        }
    }
}
