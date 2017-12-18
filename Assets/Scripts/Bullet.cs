using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Vector3 MovinTo;
    public GameObject MovinToObj;

    private Vector3 StartPos;
    private const float Speed = .5f;

    private void Start()
    {
        StartPos = this.transform.position;
    }

    void Update()
    {
        if (MovinToObj != null)
            MovinTo = MovinToObj.transform.position;

        transform.rotation = Quaternion.LookRotation(MovinTo);

        transform.position = Vector3.MoveTowards(transform.position, MovinTo, Speed);
        

        if (Vector3.Distance (this.transform.position, StartPos)>100)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);

        if (other.tag == "Targetable")
        {
            Destroy(other.gameObject);
        }

        Destroy(this.gameObject);

    }

}
