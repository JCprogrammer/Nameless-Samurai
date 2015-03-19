using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChunkGenerator : Motor
{

    
    public List<GameObject> chunks;
    public GameObject currentChunk;
    [HideInInspector]
    public float timeForNextGeneration = 0;
    public float HorizontalSpeed;
    bool isFadingIn;

    public override void Ignite()
    {
        base.Ignite();
        Generate();
    }
    void Generate()
    {
        // Random.seed = System.DateTime.Now.Millisecond;
        int randomIndex = Random.Range(0, chunks.Count);
        Motor sample = (Instantiate(chunks[randomIndex], transform.position, Quaternion.identity) as GameObject).GetComponent<Motor>();
        if (sample != null)
        {
            //MainController.instance.addNewMotor(sample);
            Motor[] children = new Motor[0];
            if (sample.transform.childCount > 0)
                children = sample.GetComponentsInChildren<Motor>();
            foreach (var item in children)
            {
                MainController.instance.addNewMotor(item);
            }
        }
        currentChunk = chunks[randomIndex];
        timeForNextGeneration = (float)timeSinceIgnition + (currentChunk.GetComponent<ChunkData>().chunkLength / (HorizontalSpeed / GlobalVariables.minifier));
        Debug.Log((currentChunk.GetComponent<ChunkData>().chunkLength / (HorizontalSpeed / GlobalVariables.minifier)));
    }
    protected override void Cycle()
    {
        base.Cycle();
        if (timeSinceIgnition >= timeForNextGeneration)
        {
            Generate();
        }
    }
    public override void KillPower()
    {
        base.KillPower();
    }

    [HideInInspector]
    public bool[] chunksSelected = new bool[0];
    
}

