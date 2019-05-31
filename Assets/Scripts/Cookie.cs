using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookie : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Pacman"))
        {
            GameManager.score+=10;
            Destroy(gameObject);
        }
    }
}
