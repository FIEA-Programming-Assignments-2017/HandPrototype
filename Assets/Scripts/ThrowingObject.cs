using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingObject : MonoBehaviour {

    public GameObject TriggerCheck;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Targetable")
        {
            TriggerCheck = other.gameObject;
        }
        
    }
}
