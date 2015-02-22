using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour {


    
    public static List<point>  checkpoints;

    void Start()
    {
        checkpoints = new List<point>();
    } 
    public void SetCheckpoint()
    {
        checkpoints.Add(new point(Camera.main.GetComponent<AudioSource>().audio.time,
                                  GameObject.FindGameObjectWithTag("Player").transform.position));
        Debug.Log("CheckPoint Added At: " + Time.timeSinceLevelLoad);

    }

    public void LoadFromLastCheckpoint()
    {
        if (checkpoints.Count == 0)
        {
            Application.LoadLevel(Application.loadedLevel);
            return;
        }
        point loadedPoint = checkpoints[checkpoints.Count - 1];
        GameObject.FindGameObjectWithTag("Player").transform.position = loadedPoint.playerPosition;
        Camera.main.GetComponent<AudioSource>().audio.time = loadedPoint.soundSeekerLocation;
        Camera.main.SendMessage("FindPlayer");
    }
    public class point
    {
        public float soundSeekerLocation;
        public Vector3 playerPosition;
        public point(float seekerLocation, Vector3 playerLocation)
        {
            soundSeekerLocation = seekerLocation;
            playerPosition = playerLocation;
        }
    }
}
