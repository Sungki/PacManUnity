using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public float speed = 5.0f;
    Vector3 movement;
    void Start()
    {
        movement = Vector3.zero;
    }

    void Update()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
