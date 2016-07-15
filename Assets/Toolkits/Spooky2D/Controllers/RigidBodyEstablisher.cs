using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class RigidBodyEstablisher : MonoBehaviour {
	
	void Start () {
		GetComponent<Rigidbody2D>().angularDrag = 0;
		GetComponent<Rigidbody2D>().gravityScale = 0;
		GetComponent<Rigidbody2D>().fixedAngle = true;
	}
}
