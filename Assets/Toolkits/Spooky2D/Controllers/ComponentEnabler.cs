using UnityEngine;
using System.Collections;

public class ComponentEnabler : MonoBehaviour {

    public MonoBehaviour[] components;
    public void EnableComponents()
    {
        foreach (var item in components)
        {
            item.enabled = true;
        }
    }

}
