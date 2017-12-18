using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
public class FirstPersonControler : MonoBehaviour {

    public new Camera camera;
    public float speed = 5f;
    public float jumpHight = 5f;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;

    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 20;

    Quaternion originalRotation;

    private Rigidbody playerRigidBody;

    public GameObject Hand;
    private HandGrab HandScript;


    public GameObject TravelToThis;

    public float HandSpeed = 5;
    public float grappleSpeed = 4;

    private bool Firing = false;

    public GameObject bullet;


    GameObject BlockToHit = null;

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        if (playerRigidBody)
            playerRigidBody.freezeRotation = true;
        originalRotation = transform.localRotation;
        //playerRigidBody.position = Vector3.zero;
        HandScript = Hand.GetComponent<HandGrab>();
    }

    void Update()
    {

        if (axes == RotationAxes.MouseXAndY)
        {
            rotAverageY = 0f;
            rotAverageX = 0f;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;

            rotArrayY.Add(rotationY);
            rotArrayX.Add(rotationX);


            if (rotArrayY.Count >= frameCounter)
            {
                rotArrayY.RemoveAt(0);
            }
            if (rotArrayX.Count >= frameCounter)
            {
                rotArrayX.RemoveAt(0);
            }

            for (int j = 0; j < rotArrayY.Count; j++)
            {
                rotAverageY += rotArrayY[j];
            }
            for (int i = 0; i < rotArrayX.Count; i++)
            {
                rotAverageX += rotArrayX[i];
            }

            rotAverageY /= rotArrayY.Count;
            rotAverageX /= rotArrayX.Count;

            rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
            rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotAverageX = 0f;

            rotationX += Input.GetAxis("Mouse X") * sensitivityX;

            rotArrayX.Add(rotationX);

            if (rotArrayX.Count >= frameCounter)
            {
                rotArrayX.RemoveAt(0);
            }
            for (int i = 0; i < rotArrayX.Count; i++)
            {
                rotAverageX += rotArrayX[i];
            }
            rotAverageX /= rotArrayX.Count;

            rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotAverageY = 0f;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotArrayY.Add(rotationY);

            if (rotArrayY.Count >= frameCounter)
            {
                rotArrayY.RemoveAt(0);
            }
            for (int j = 0; j < rotArrayY.Count; j++)
            {
                rotAverageY += rotArrayY[j];
            }
            rotAverageY /= rotArrayY.Count;

            rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            transform.localRotation = originalRotation * yQuaternion;
        }

        if (Input.GetKey("up") || Input.GetKey("w"))
            transform.position += transform.forward * (Time.deltaTime * speed);

        if (Input.GetKey("down") || Input.GetKey("s"))
            transform.position -= transform.forward * (Time.deltaTime * speed);

        if (Input.GetKey("right") || Input.GetKey("d"))
            transform.position += transform.right * (Time.deltaTime * speed);

        if (Input.GetKey("left") || Input.GetKey("a"))
            transform.position -= transform.right * (Time.deltaTime * speed);

        if (Input.GetKey("space") && playerRigidBody.velocity.y == 0)
        {
            playerRigidBody.AddForce(Vector3.up * jumpHight);
        }


        RaycastHit Hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(.5f, .5f, .5f));

        if (Physics.Raycast(ray, out Hit))
        {
            if (Hit.transform.gameObject.tag!= "Untagged")
            BlockToHit = Hit.transform.gameObject;
        }


        if (Input.GetKey("e") && BlockToHit != null)
        {
            Hand.GetComponent<Rigidbody>().detectCollisions = true;
            if (HandScript.Throwing!=null && HandScript.canFling)
            {
                Hand.GetComponent<Rigidbody>().detectCollisions = false;
                HandScript.canFling = false;
                StartCoroutine(ThrowingObject(HandScript.Throwing, BlockToHit));
                //HandScript.Throwing.GetComponent<Rigidbody>().AddForce(transform.forward * 20);
                HandScript.Throwing = null;
            }
            else if (HandScript.canFling)
            {
                Hand.transform.position = Vector3.MoveTowards(Hand.transform.position, BlockToHit.transform.position, HandSpeed);
            }
            else if (TravelToThis != null)
            {

                if (Vector3.Distance(TravelToThis.transform.position, transform.position) > 2)
                {
                    transform.position = Vector3.MoveTowards(transform.position, TravelToThis.transform.position, grappleSpeed);
                    Hand.transform.position= TravelToThis.transform.position;
                }
                else
                {
                    TravelToThis = null;
                }
            }
        }
        else
        {

            if (HandScript.Throwing != null)
                {
                    Hand.transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up*1.2f, HandSpeed);
                }
                else
                {
                Hand.transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, HandSpeed);
                }
                HandScript.canFling = true;
                TravelToThis = null;
                Hand.GetComponent<Rigidbody>().detectCollisions = false;
        }
      

        if (Input.GetKey("q") && !Firing)
        {
            if (BlockToHit!= null)
                StartCoroutine(ShootingBullet(BlockToHit));
            else
                StartCoroutine(ShootingBullet());

        }

        

    }
    IEnumerator ShootingBullet(GameObject MovingTo)
    {
        Firing = true;
        GameObject Projectile = Instantiate(bullet, transform.position,Quaternion.identity) as GameObject;
        Projectile.transform.parent = this.gameObject.transform;
        Projectile.GetComponent<Bullet>().MovinToObj = MovingTo;
         yield return new WaitForSeconds(.25f);
        Firing = false;
    }
    IEnumerator ShootingBullet()
    {
        Firing = true;
        GameObject Projectile = Instantiate(bullet, transform.position, Quaternion.identity) as GameObject;
        Projectile.transform.parent = this.gameObject.transform;
        Projectile.GetComponent<Bullet>().MovinTo = transform.forward*1000;
        yield return new WaitForSeconds(.25f);
        Firing = false;
    }
    IEnumerator ThrowingObject(GameObject ObjectFlinging, GameObject MovingTo)
    {
        ThrowingObject ThrowScript = ObjectFlinging.GetComponent<ThrowingObject>();

        while (ObjectFlinging != null)
        {
                //Debug.Log(ThrowScript.TriggerCheck.name);

            if (ThrowScript.TriggerCheck!= null &&ThrowScript.TriggerCheck== MovingTo)
            {
                Destroy(ObjectFlinging.gameObject);
                Destroy(MovingTo.gameObject);
            }
                ObjectFlinging.transform.position =  Vector3.Lerp(ObjectFlinging.transform.position,MovingTo.transform.position, .05f); 
            yield return new WaitForFixedUpdate();
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Throwable")
        {
            if (HandScript.Throwing!= null)
            Physics.IgnoreCollision(this.GetComponent<Collider>(), HandScript.Throwing.GetComponent<Collider>());
        }
    }

}
