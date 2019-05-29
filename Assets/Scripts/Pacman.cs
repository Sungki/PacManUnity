using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public float speed = 5.0f;
    Vector3 movement;
    Rigidbody myRigidbody;
    bool isMoving = false;

   // Vector3 direction;
    void Start()
    {
        movement = Vector3.zero;
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void MoveMotor(Vector3 _direction)
    {
        if (isMoving) return;

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
        //        movement = Vector3.zero;
        //            movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //            movement.x = 1.0f;

//        transform.position = Vector3.MoveTowards(transform.position, transform.position+direction, speed * Time.deltaTime);

        //        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
//        myRigidbody.MovePosition(transform.position + movement * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
