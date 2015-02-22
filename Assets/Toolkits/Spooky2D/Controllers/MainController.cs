using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    public Motor[] motors;
	// Use this for initialization
	
    void Start () {
        motors = FindObjectsOfType<Motor>();  
	}
	
	
	void FixedUpdate () {
        foreach (var item in motors)
        {
            if (item != null)
            {
                if (item.enabled == true)
                    item.MotoUpdate();
            }
        }
	}
}
