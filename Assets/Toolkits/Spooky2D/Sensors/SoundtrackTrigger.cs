using UnityEngine;
using System.Collections;

public class SoundtrackTrigger : MonoBehaviour {

    public Behavior[] floakBehaviors;
    public AudioSource soundtrack;
    public bool isTriggered = false;
    public float TriggerTime;
    public bool sendMessageByTag;
    public string targetsTag;
    public string tagSenderMessage = "";
    void FixedUpdate()
    {
        if (!isTriggered)
        {
            if (soundtrack.GetComponent<AudioSource>().time >= TriggerTime)
            {
                if (!sendMessageByTag)
                {
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
                }
                else
                {
                    GameObject[] foundObjectsByTag = GameObject.FindGameObjectsWithTag(targetsTag);
                    foreach (var item in foundObjectsByTag)
                    {
                        item.SendMessage(tagSenderMessage);
                    }
                }
                isTriggered = true;
                this.enabled = false;
            }

        }
    }

}
