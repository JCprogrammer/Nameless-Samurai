using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
	public Texture2D[] backgrounds;
	private float nextBackgroundTimeStamp = 0.0F;
	public float backgroundInterval = 1.0F;
	private int currentBackgroundIndex = 0;
	public float fadeOutSpeed = 1.0F;
	private bool fadingOut = false;
	private bool fadedOut = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.timeSinceLevelLoad >= nextBackgroundTimeStamp)
		{
			int index = -1;

			while((index = Random.Range(0, 3)) == currentBackgroundIndex) { }
			guiTexture.texture = backgrounds[index];
			currentBackgroundIndex = index;
			nextBackgroundTimeStamp = Time.timeSinceLevelLoad + backgroundInterval;
		}

		if(fadingOut)
		{
			if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
			{
				foreach(GUIText item in FindObjectsOfType<GUIText>())
				{
					if(item.name != "Narrative")
					{
						item.color = new Color(item.color.r, item.color.g, item.color.b, 0.0F);
						
						if(item.color.a <= 0.0F)
						{
							item.color = new Color(item.color.r, item.color.g, item.color.b, 0.0F);
							fadedOut = true;
						}
					}
				}
			}

			foreach(GUIText item in FindObjectsOfType<GUIText>())
			{
				if(item.name != "Narrative")
				{
					item.color = new Color(item.color.r, item.color.g, item.color.b, item.color.a - fadeOutSpeed * Time.deltaTime);
					
					if(item.color.a <= 0.0F)
					{
						item.color = new Color(item.color.r, item.color.g, item.color.b, 0.0F);
						fadedOut = true;
					}
				}
			}
			
			if(fadedOut)
			{
				fadingOut = false;
				
				GameObject.Find("Narrative").GetComponent<NarrativeController>().enabled = true;
				//GameObject.Find("Narrative").GetComponent<NarrativeController>().Start();
				GameObject.Find("Narrative Background").guiTexture.enabled = true;
			}
		}
	}

	public void StartGame()
	{
		fadingOut = true;
	}
}
