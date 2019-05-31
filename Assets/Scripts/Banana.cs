using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pacman"))
        {
            ScriptLocator.gamemanager.GetComponent<GameManager>().SetScore(50);
            Destroy(gameObject);
        }
    }
}
