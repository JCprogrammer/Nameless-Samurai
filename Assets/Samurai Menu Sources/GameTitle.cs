using UnityEngine;
using System.Collections;

public class GameTitle : MonoBehaviour {

    public float TitleDisappearanceTime = 0.1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeSinceLevelLoad > TitleDisappearanceTime)
            Application.LoadLevel("menu");
	}
}
