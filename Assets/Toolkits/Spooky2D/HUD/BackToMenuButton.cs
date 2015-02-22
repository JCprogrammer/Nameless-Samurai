using UnityEngine;
using System.Collections;

public class BackToMenuButton : MonoBehaviour {

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 70, 20, 50, 30), "Menu"))
        {
            Application.LoadLevel("MainMenu");
        }
    }
}
