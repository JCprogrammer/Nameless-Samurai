using UnityEngine;
using System.Collections;

[RequireComponent (typeof(RigidBodyEstablisher))]
[RequireComponent(typeof(ObjectIdentity))]
public class GravityMotor: Motor {

    Vector3 meshSize;
	bool gravityOn;
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
        //
        if (gameObject.transform.position.y - meshSize.y / 2 + 0.5f >
            coll.transform.position.y + coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2)
        {
            gameObject.transform.position = new Vector3(transform.position.x,
                coll.transform.position.y + coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y,
                transform.position.z);
            gravityForce = 0;
            collState = CollisionState.onTop;
            SendMessage("FixRotationDistortion");
        }

            
       
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (collState == CollisionState.onTop)
        {
            gravityForce = 0;
            if (!(gameObject.transform.position.y - meshSize.y / 2 + 0.5f >
             coll.transform.position.y + coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2))
            {
                collState = CollisionState.none;
            }
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        
    }

   
    public enum CollisionState
    {
        none,
        onTop,
        onSide
    }
   }
