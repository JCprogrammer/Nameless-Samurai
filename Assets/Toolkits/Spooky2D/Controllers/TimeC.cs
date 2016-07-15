using UnityEngine;
using System.Collections;

public class TimeC : MonoBehaviour {

    bool isPaused = false;
	public float deltaTime = 1;
    float stopAt = 0;
	void Start () {
	}
	void Update () {

        GlobalVariables.deltaTimeConst = 0.002f * deltaTime;
        //StopTime();
    }

    void StopTime()
    {
        if (GetComponent<AudioSource>().GetComponent<AudioSource>().time >= stopAt)
            deltaTime = 0;
    }
    public void Increase()
    {
        if (deltaTime < 1)
            deltaTime += 0.003f;
        else
            deltaTime = 1;
    }

    
    public void Decrease()
    {
        if (deltaTime - 0.03f > -1)
            deltaTime -= 0.003f;
        else
            deltaTime =-1;
    }

    public void Pause()
    {
        if (!isPaused)
        {
            deltaTime = 0;
            isPaused = true;
        }
        else
        {
            deltaTime = 1;
            isPaused = false;
        }
    }
}
