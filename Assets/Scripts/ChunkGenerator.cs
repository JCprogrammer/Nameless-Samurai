using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChunkGenerator : Motor
{
    //public List<GenerationSample> samples;
    //public  GenerationSample currentSample;
    public List<GameObject> chunks;
    public GameObject currentChunk;

    public float timeForNextGeneration = 0;
    public float HorizontalSpeed;
    bool isFadingIn;

    public override void Ignite()
    {

        base.Ignite();
    
        //currentSample = samples[0];
        //isFadingIn = currentSample.initiationType == GenerationInitiationEndingType.Fade_in;
        //Debug.Log(isFadingIn);
        Generate();
    }

    void Generate()
    {
        int randomIndex = Random.Range(0, chunks.Count - 1);
        Instantiate(chunks[randomIndex], transform.position, Quaternion.identity);
        currentChunk = chunks[randomIndex];
        timeForNextGeneration = (float)timeSinceIgnition + (currentChunk.GetComponent<ChunkData>().chunkLength / (HorizontalSpeed / GlobalVariables.minifier) );
        Debug.Log((currentChunk.GetComponent<ChunkData>().chunkLength / (HorizontalSpeed / GlobalVariables.minifier) ));
    }
    protected override void Cycle()
    {
        
        base.Cycle();
        //Debug.Log(timeSinceIgnition);
        if (timeSinceIgnition >= timeForNextGeneration)
        {
            Generate();
        }
    }

    protected override void KillPower()
    {
        base.KillPower();
    }
}

public enum GenerationInitiationEndingType
{
    Immediately,
    Fade_in,
    Fade_out
}
[System.Serializable]
public class GenerationSample
{
    public List<GameObject> sampleObjects;
    public float GenerationRange;
    public float GenerationExecutionInterval;
    public float period;
    public GenerationInitiationEndingType initiationType;
    public GenerationInitiationEndingType endingType;
    public float fadingTime;
}