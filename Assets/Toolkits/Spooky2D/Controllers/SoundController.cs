using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {


	public AudioSource backgroundMusicSource;
	public  float startFrom;
    float initiationPoint = 0;
    bool stopCounting = false;
    string fakeAutioTime = "0";
	// Use this for initialization
    float songProgress = 0;
    void Start () {
        initiationPoint = backgroundMusicSource.volume;
        backgroundMusicSource.volume = 0;
        //Debug.Log("ssssssssssssssssss : " + startFrom.ToString());
		backgroundMusicSource.audio.time = startFrom;
        songProgress = startFrom;
        
    }

    float sample1 = 0;
    float sample2 = 0;
	// Update is called once per frame
	void Update () {
        
            backgroundMusicSource.volume += 0.005f;
            //Debug.Log(backgroundMusicSource);
            if (!stopCounting)
                fakeAutioTime = backgroundMusicSource.audio.time.ToString();
            
        //backgroundMusicSource.audio.time = songProgress += GlobalVariables.deltaTime;
            sample1 = sample2;
            //Debug.Log(backgroundMusicSource.audio.timeSamples);
            sample2 = backgroundMusicSource.audio.time;
            //Debug.Log(sample2 - sample1);
 
	}
    void StopCounting()
    {
        stopCounting = true;
    }
    void OnGUI()
    {
       
            GUI.Box(new Rect(10, 10, 200, 40), "Song Time: " + fakeAutioTime);
    }
}
