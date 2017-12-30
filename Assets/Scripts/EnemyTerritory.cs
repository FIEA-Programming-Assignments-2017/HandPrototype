using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTerritory : MonoBehaviour
{
    public GameObject enemyGameObject;
    private Enemy enemyObject;

    private void Start()
    {
        enemyObject = enemyGameObject.GetComponent<Enemy>();
        if (enemyObject == null)
        {
            Debug.Log("in StationaryEnemyTerritory.cs Start: null return");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            enemyObject.setPlayerIsInTerritory(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            enemyObject.setPlayerIsInTerritory(false);
        }
    }
}
