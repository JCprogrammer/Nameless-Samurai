using UnityEngine;
using System.Collections;

public class NarrativeController : MonoBehaviour
{
	public TextAsset narrativeText;
	private int textCounter = 0;
	private float nextTextWriteTimeStamp = 0.0F;
	public float timeWriteInterval = 0.01F;
	private bool writing = false;
	private bool loadLevelSoon = false;
	private float loadLevelTimeStamp = 0.0F;
	public float loadLevelTime = 2.0F;
	
	public void Start()
	{
		writing = true;
	}
	
	void Update()
	{
		if(Time.timeSinceLevelLoad >= nextTextWriteTimeStamp && writing)
		{
			textCounter++;
			
			if(textCounter <= narrativeText.text.Length)
			{
				guiText.text = narrativeText.text.Substring(0, textCounter);
			}
			
			if(textCounter == narrativeText.text.Length)
			{
				loadLevelTimeStamp = Time.timeSinceLevelLoad + loadLevelTime;
				loadLevelSoon = true;
				
				writing = false;
			}
			
			nextTextWriteTimeStamp = Time.timeSinceLevelLoad + timeWriteInterval;
		}
		
		if(writing && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
			Application.LoadLevel("Tutorial");
			//textCounter = narrativeText.text.Length - 1;

		/*if(textCounter >= narrativeText.text.Length - 1 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
			Application.LoadLevel("FirstLevel");*/
		
		if(Time.timeSinceLevelLoad >= loadLevelTimeStamp && loadLevelSoon)
		{
			Application.LoadLevel("FirstLevel");
		}
	}
}
