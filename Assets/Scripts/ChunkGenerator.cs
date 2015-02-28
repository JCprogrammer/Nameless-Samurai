using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChunkGenerator : Motor
{
    public List<GenerationSample> samples;
    public  GenerationSample currentSample;
    bool isFadingIn;

    public override void Ignite()
    {

        base.Ignite();
        currentSample = samples[0];
        isFadingIn = currentSample.initiationType == GenerationInitiationEndingType.Fade_in;
        Debug.Log(isFadingIn);
    }

    protected override void Cycle()
    {
        
        base.Cycle();
        Debug.Log("asdas");
        if (isFadingIn)
        {
            Debug.Log("Chunk Generator Time: " + timeSinceIgnition);
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