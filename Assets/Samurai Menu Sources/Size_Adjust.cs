using UnityEngine;
using System.Collections;

public class Size_Adjust : MonoBehaviour {
    public int fixed_width = 100, fixed_height = 100;

	// Use this for initialization
	void Start ()
    {
        adjust();
	}

	void Update ()
	{
		adjust();
	}
    public void adjust()
    {
        transform.localScale = new Vector3((float)fixed_width / Screen.width, (float)fixed_height / Screen.height, 1.0F);
    }
}
