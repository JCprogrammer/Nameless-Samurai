using UnityEngine;
using System.Collections;

public class LogoController : MonoBehaviour {
	public enum State
	{
		Nothing,
		FadingIn,
		InBetween,
		FadingOut,
		Done,
	}
	public State state = State.Nothing;
	public float fadeInSpeed = 1.0F;
	public float fadeOutSpeed = 1.0F;
	public float inBetweenTime = 2.0F;
	private float inBetweenEndTimeStamp = 0.0F;

	// Use this for initialization
	void Start ()	
	{
		state = State.FadingIn;
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch(state)
		{
		case State.FadingIn:
			GetComponent<GUITexture>().color = new Color(GetComponent<GUITexture>().color.r, GetComponent<GUITexture>().color.g, GetComponent<GUITexture>().color.b, GetComponent<GUITexture>().color.a + fadeInSpeed * Time.deltaTime);

			if(GetComponent<GUITexture>().color.a >= 1.0F)
			{
				GetComponent<GUITexture>().color = new Color(GetComponent<GUITexture>().color.r, GetComponent<GUITexture>().color.g, GetComponent<GUITexture>().color.b, 1.0F);

				inBetweenEndTimeStamp = Time.timeSinceLevelLoad + inBetweenTime;
				state = State.InBetween;
			}
			break;
		case State.InBetween:
			if(Time.timeSinceLevelLoad >= inBetweenEndTimeStamp)
			{
				state = State.FadingOut;
			}
			break;
		case State.FadingOut:
			GetComponent<GUITexture>().color = new Color(GetComponent<GUITexture>().color.r, GetComponent<GUITexture>().color.g, GetComponent<GUITexture>().color.b, GetComponent<GUITexture>().color.a - fadeOutSpeed * Time.deltaTime);

			if(GetComponent<GUITexture>().color.a <= 0.0F)
			{
				GetComponent<GUITexture>().color = new Color(GetComponent<GUITexture>().color.r, GetComponent<GUITexture>().color.g, GetComponent<GUITexture>().color.b, 0.0F);

				state = State.Done;
			}
			break;

		case State.Done:
			Application.LoadLevel("TitleScene");
			break;
		}
	}
}
