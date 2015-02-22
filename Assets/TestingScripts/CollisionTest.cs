using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {


    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("<Enter>Collider Name On Collision: " + coll.collider.name);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        Debug.Log("<Stay>Collider Name On Collision: " + coll.collider.name);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("<Enter> Trigger Collider Name: " + coll.name);
    }
    void OnTriggerStay2D(Collider2D coll)
    {
        //Debug.Log("<Stay> Trigger Collider Name: " + coll.name);
    }
}
