using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class RigidBodyEstablisher : MonoBehaviour {
	
	void Start () {
		rigidbody2D.angularDrag = 0;
		rigidbody2D.gravityScale = 0;
		rigidbody2D.fixedAngle = true;
	}
}
