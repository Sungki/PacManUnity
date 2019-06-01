using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pacman"))
        {
            SetStateAllRunaway();
            Destroy(gameObject);
        }
    }

    void SetStateAllRunaway()
    {
        GameObject[] allEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in allEnemy)
            e.gameObject.SendMessage("SetState",Enemy.EnemyState.Runaway);
    }
}
