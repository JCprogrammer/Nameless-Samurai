using UnityEngine;
using System.Collections;

[RequireComponent (typeof(RigidBodyEstablisher))]
[RequireComponent(typeof(ObjectIdentity))]
public class GravityMotor: Motor {

    Vector3 meshSize;
	public bool gravityOn;
	public float gravityForce = 0;
    public CollisionState collState;
	public float MaxGravitySpeed;
	public override void Ignite ()
	{
		gravityOn = true;
		base.Ignite ();
        meshSize = GetComponent<ObjectIdentity>().meshSize;
	}

	void GravityMotorKillPower(){
		KillPower ();
		gravityOn = false;
	}
	
	void GravityMotorIgnite(){
		Ignite ();
		gravityOn = true;
	}

	protected override void Cycle (){
				base.Cycle ();
		if (!switchedOn)
			return;
				if (gravityOn) {
						transform.position += (new Vector3 (0, -1, 0)) * 
						((gravityForce * GlobalVariables.deltaTimeConst) / 
			    		(float)GlobalVariables.minifier);
						if(gravityForce < MaxGravitySpeed)
                        gravityForce += GlobalVariables.baseAccelerator * (GlobalVariables.deltaTimeConst / GlobalVariables.deltaTime);                         
				}
	}
    void OnCollisionEnter2D(Collision2D coll)
    {
        //if (gameObject.transform.position.y - ((transform.localScale.y * meshSize.y / 2) + (0.5f * transform.localScale.y)) >
        //    coll.transform.position.y + (transform.localScale.y * coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2))
        //{
        //    Debug.Log("Object Hit");
        //    gameObject.transform.position = new Vector3(transform.position.x,
        //        coll.transform.position.y + (coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y * coll.transform.localScale.y) + (meshSize.y * transform.localScale.y),
        //        transform.position.z);
        //    gravityForce = 0;
        //    collState = CollisionState.onTop;
        //    SendMessage("FixRotationDistortion");
        //}

        if (transform.position.y + ((GetComponent<BoxCollider2D>().size.y/2 + 0.1f) ) >
       coll.transform.position.y + (coll.transform.GetComponent<BoxCollider2D>().size.y / 2) - coll.transform.GetComponent<BoxCollider2D>().offset.y)
        {
            gravityOn = false;
            gravityForce = 0;
        }
       
    }
    //void OnCollisionStay2D(Collision2D coll)
    //{
    //    if (collState == CollisionState.onTop)
    //    {
    //        gravityForce = 0;
    //        if (!(gameObject.transform.position.y - meshSize.y / 2 + (0.5f * transform.localScale.y) >
    //         coll.transform.position.y + coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2))
    //        {
    //            collState = CollisionState.none;
    //        }
    //    }
    //}
    void OnCollisionExit2D(Collision2D coll)
    {

        if (Physics2D.Raycast(transform.position - new Vector3(0,GetComponent<BoxCollider2D>().size.y/2 + 0.1f,0),Vector3.forward))
        {
            print("There is something in front of the object!");
        }
        else
        {
            SendMessage("FreeToRotate",SendMessageOptions.DontRequireReceiver);
            gravityOn = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position - new Vector3(0, GetComponent<BoxCollider2D>().size.y/2 + 0.5f, 0), transform.position - new Vector3(0, GetComponent<BoxCollider2D>().size.y/2 + 0.5f, 0) + (Vector3.forward * 100));
    }
   
    public enum CollisionState
    {
        none,
        onTop,
        onSide
    }
   }
