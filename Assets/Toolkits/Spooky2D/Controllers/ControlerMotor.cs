using UnityEngine;
using System.Collections;

public class ControlerMotor : MonoBehaviour {

    string buttonPressed = "No Button";
    void OnGUI()
    {
        buttonPressed = "No Button";
        if (Input.GetButton("Jump"))
        {
            buttonPressed = "Jump Button";
        }
        GUI.Box(new Rect(10, 10, 100, 20), buttonPressed);
    }
}
