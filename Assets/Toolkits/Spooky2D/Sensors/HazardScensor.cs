﻿using UnityEngine;
using System.Collections;

public class HazardScensor : MonoBehaviour
{
    Vector3 meshSize;
    public HazardAction[] hazardActions;
    void Start()
    {
        if (hazardActions == null)
            this.enabled = false;
        meshSize = gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size;
    }
    public void Destruct()
    {
        Camera.main.SendMessage("KillPower");
        Destroy(this.gameObject);
    }
    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void LoadLastCheckPoint()
    {
        Camera.main.SendMessage("LoadFromLastCheckpoint");
    }
    public void KillPowerAll()
    {
        SendMessage("KillPower");
        Camera.main.SendMessage("KillPower");
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (hazardActions == null)
            return;
        foreach (var hazard in hazardActions)
        {
            //if (transform.position.y - (GetComponent<BoxCollider2D>().size.y / 2 + (hazard.sensationThreshold * transform.localScale.y)) >
            //coll.transform.position.y + (coll.transform.GetComponent<BoxCollider2D>().size.y / 2))
            //{
            //if (!(gameObject.transform.position.y - meshSize.y + hazard.sensationThreshold>
            //  coll.transform.position.y + coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2))
            //{
                Vector2 contactPoint =  coll.contacts[0].point;
                Debug.Log(Vector3.Magnitude((contactPoint - (Vector2)transform.position)));
                if (Vector3.Magnitude((contactPoint - (Vector2)transform.position)) < GetComponent<BoxCollider2D>().size.x / 2 - hazard.sensationThreshold)
                {
                    if (coll.collider.tag == hazard.hazardTag)
                    {
                        foreach (var item in hazard.actions)
                        {
                            SendMessage(item, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            //}    
        }
        
    }
}

[System.Serializable]
public class HazardAction
{
    public string[] actions;
    public string hazardTag;
    public float sensationThreshold = 0.1f;
}
