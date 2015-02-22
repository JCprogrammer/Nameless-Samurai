using UnityEngine;
using System.Collections;

public class CenterRotateMotor: Motor{

	public float speed = -4;
	public bool freeToRotate = true;
	Vector2 lastPosition;
	protected override void Cycle ()
	{
				base.Cycle ();
				if (!switchedOn)
						return;
                GravityMotor comp = GetComponent<GravityMotor>();
                if (comp != null)
                {
                    if (comp.gravityForce > GlobalVariables.baseAccelerator)
                        FreeToRotate();
                }
                else
                    FreeToRotate();
                
				if (freeToRotate)
						transform.Rotate (0, 0, speed * 
			                  GlobalVariables.deltaTimeConst / 
			                  GlobalVariables.minifier);
		}
	void FixRotationDistortion()
	{
		float degree = transform.rotation.eulerAngles.z;
		if (degree < 0)
			degree = 360 + degree;
		if (degree >= 0 && degree < 45)
			degree = 0;
		else if (degree >= 45 && degree < 90)
			degree = 90;
		else if (degree >= 90 && degree < 135)
			degree = 90;
		else if (degree >= 135 && degree < 180)
			degree = 180;
		else if (degree >= 180 && degree <225 )
			degree = 180;
		else if (degree >= 225 && degree < 270)
			degree = 270;
		else if (degree >= 270 && degree < 315)
			degree = 270;
		else if (degree >= 315 && degree < 360)
			degree = 0;
		transform.rotation = Quaternion.Euler (0, 0, degree);
		freeToRotate = false;
	}
	void OnCollisionEnter2D(Collision2D coll) {
		//FixRotationDistortion ();
	}
	public void FreeToRotate()
	{
		freeToRotate = true;
	}

}
