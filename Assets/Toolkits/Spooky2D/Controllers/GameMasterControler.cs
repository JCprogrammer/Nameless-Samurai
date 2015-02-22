using UnityEngine;
using System.Collections;

public class GameMasterControler : MonoBehaviour {

	TimeC timeController;
	// Use this for initialization
	void Start () {
		timeController = GetComponent<TimeC> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.Label (new Rect (20, 20, 100, 30), "Time Controller");
		GUI.Label (new Rect (20, 40, 10, 20), "0");
		GUI.Label (new Rect (185, 40, 25, 20), "50");
		GUI.Label (new Rect (345, 40, 25, 20), "100");
		timeController.deltaTime = GUI.HorizontalScrollbar (new Rect (20, 60, 350, 25), timeController.deltaTime * 100, 1, 0, 100)/100.0f;
		GUI.Box (new Rect (10, 10, 400, 150),"");

		GUI.Label (new Rect (20, 100, 100, 30), "Restart Game: ");
		if (GUI.Button (new Rect (125, 100, 100, 30), "Restart"))
						Application.LoadLevel (Application.loadedLevel);
	}
}
