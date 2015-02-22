using UnityEngine;
using System.Collections;

public class ObjectIdentity : MonoBehaviour {

    [HideInInspector]
    public Vector3 meshSize;
	void Start () {
        //meshSize = gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size;
	}
	
	
}
