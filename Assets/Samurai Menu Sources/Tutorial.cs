using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
    public float tutorialDisappearanceTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeSinceLevelLoad > tutorialDisappearanceTime)
            Application.LoadLevel("FirstLevel");
	}
}
