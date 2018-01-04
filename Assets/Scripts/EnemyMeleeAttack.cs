using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour {

    private bool currentlyHittingPlayer = false;

    public bool wasAttackSuccessful()
    {
        if (currentlyHittingPlayer)
        {
            currentlyHittingPlayer = false;
            return true;
        }
        return false;
    }

	void Start () {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<FirstPersonControler>();//deal damage to player here
            currentlyHittingPlayer = true;
        }
    }
}
