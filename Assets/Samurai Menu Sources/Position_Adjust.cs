using UnityEngine;
using System.Collections;

public class Position_Adjust : MonoBehaviour {
    public int fixed_left = 0, fixed_top = 0;
    public bool reverse_top = false;
    public bool reverse_left = false;
    public bool use_normalized_left = false;
    public bool use_normalized_top = false;
    public float normalized_left = 0.0F, normalized_top = 0.0F;

	// Use this for initialization
	void Start()
    {
        adjust();
	}
	
	void Update ()
	{
		adjust();
	}

    public void adjust()
    {
        float left = (use_normalized_left)?(normalized_left):((float)fixed_left / Screen.width);
        float top = (use_normalized_top)?(normalized_top):((float)fixed_top / Screen.height);
        if(reverse_top) top = 1.0F - top;
        if(reverse_left) left = 1.0F - left;
        transform.localPosition = new Vector3(left, top);
    }
}
