using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour
{

    public Vector3 activationTreshold = Vector3.zero;
    public Behavior[] floakBehaviors;

    public bool triggered = false;

    void OnTriggerStay2D(Collider2D coll)
    {
        //Debug.Log(coll.name);
        TriggerObjectsInBehaviors(coll);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        //Debug.Log(coll.name);
        TriggerObjectsInBehaviors(coll);
    }

    virtual protected void TriggerObjectsInBehaviors(Collider2D coll)
    {
        if (!triggered)
        {
            Vector3 poseDifferece = coll.transform.position -
                                    gameObject.transform.position;
            if ((Mathf.Abs(poseDifferece.x) <= activationTreshold.x) &&
                (Mathf.Abs(poseDifferece.y) <= activationTreshold.y))
            {
                //Debug.LogWarning("Item Activated");
                foreach (var folks in floakBehaviors)
                {
                    foreach (var item in folks.Objects)
                    {
                        foreach (var action in folks.Actions)
                        {
                            item.SendMessage(action);
                        }
                    }
                }
                triggered = true;
            }

        }
    }
}
[System.Serializable]
public class Behavior
{
    public string[] Actions;
    public GameObject[] Objects;
}