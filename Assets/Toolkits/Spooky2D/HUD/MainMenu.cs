using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnGUI()
    {
        GUI.Box(new Rect((Screen.width / 2) - 125, (Screen.height / 2) -60, 250, 200), GUIContent.none);
        if (GUI.Button(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 15, 200, 30), "Synch Demo"))
        {
            Application.LoadLevel("Level1F");
        }

        if (GUI.Button(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 15 + 40, 200, 30), "Moving Platforms Demo"))
        {
            Application.LoadLevel("Farid");
        }

        if (GUI.Button(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 15 + 80, 200, 30), "Loop Demo"))
        {
            Application.LoadLevel("MonsterLimb");
        }
        GUI.Box(new Rect((Screen.width / 2) - 225, (Screen.height / 2) + 150, 450, 60), "! Remember, the only mechanic you have is \"jump\" and its triggered\n by hitting Space button. \nEnjoy!");
    }
}
