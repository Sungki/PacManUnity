using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public float speed = 5.0f;
    Vector3 movement;
    Rigidbody myRigidbody;
    void Start()
    {
        movement = Vector3.zero;
        myRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movement = Vector3.zero;
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

          
//        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        myRigidbody.MovePosition(transform.position + movement * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
