using UnityEngine;
using System.Collections;

public class FSMDiagram : MonoBehaviour {

    public TextMesh[] stateNodes;
    public static FSMDiagram instance;
    // Use this for initialization
	void Start () {
        instance = this;
	}
    public void ChangeState(string stateName)
    {
        foreach (var item in stateNodes)
        {
            if (item.text == stateName)
            {
                item.color = Color.green;
            }
            else
                item.color = Color.white;
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
