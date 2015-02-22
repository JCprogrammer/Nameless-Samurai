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
			guiTexture.color = new Color(guiTexture.color.r, guiTexture.color.g, guiTexture.color.b, guiTexture.color.a + fadeInSpeed * Time.deltaTime);

			if(guiTexture.color.a >= 1.0F)
			{
				guiTexture.color = new Color(guiTexture.color.r, guiTexture.color.g, guiTexture.color.b, 1.0F);

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
			guiTexture.color = new Color(guiTexture.color.r, guiTexture.color.g, guiTexture.color.b, guiTexture.color.a - fadeOutSpeed * Time.deltaTime);

			if(guiTexture.color.a <= 0.0F)
			{
				guiTexture.color = new Color(guiTexture.color.r, guiTexture.color.g, guiTexture.color.b, 0.0F);

				state = State.Done;
			}
			break;

		case State.Done:
			Application.LoadLevel("TitleScene");
			break;
		}
	}
}
