using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{	
	void Start(){
		//iTween.MoveBy(gameObject, iTween.Hash("x", -7, "easeType", "easeInOutExpo", "loopType", "once", "delay", 1));
		iTween.MoveBy(gameObject,new Vector3(10,0,0),5);
	}
}

