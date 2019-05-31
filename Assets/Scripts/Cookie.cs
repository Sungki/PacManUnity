using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookie : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Pacman"))
        {
            ScriptLocator.gamemanager.GetComponent<GameManager>().score+=10;
            ScriptLocator.gamemanager.GetComponent<GameManager>().SetScore();
            Destroy(gameObject);
        }
    }
}
