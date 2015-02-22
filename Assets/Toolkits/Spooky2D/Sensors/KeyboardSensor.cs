using UnityEngine;
using System.Collections;

public class KeyboardSensor : MonoBehaviour {

	public KeyBasedActoin[] actions;
    void FixedUpdate()
    {

        
        foreach (var item in actions)
        {
            if (Input.GetKey(item.keycode))
                foreach (var itm in item.Actions)
                {
                    SendMessage(itm);
                }
        }

        if (Input.touchCount > 0)
        {
            SendMessage("Jump");
        }


    }
}
[System.Serializable]
public class KeyBasedActoin
{
	public KeyCode keycode;
	public string[] Actions;
}