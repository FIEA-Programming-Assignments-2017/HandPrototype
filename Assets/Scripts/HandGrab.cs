using UnityEngine;

public class HandGrab : MonoBehaviour {

    public GameObject Player;

    public bool canFling=true;

    public GameObject Throwing;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Grabbable")
        {
            Player.GetComponent<FirstPersonControler>().TravelToThis= other.gameObject;
            canFling = false;
        }

        if (other.gameObject.tag == "Throwable")
        {
            if (Throwing!=null && other.gameObject== Throwing)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), Throwing.GetComponent<Collider>());
            }
            else
            {
                Throwing = other.gameObject;
                canFling = false;
            }
        }

    }


    private void Update()
    {
        if (Throwing != null)
        {
            Throwing.transform.position = this.transform.position;
        }
    }

}
