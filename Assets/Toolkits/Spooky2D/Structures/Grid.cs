using UnityEngine;
using System.Collections;

public class Grid
{
    public float basePatchHeight;
    public float basePatchWidth;
	public float patchHeight;
	public float patchWidth;
	public Rect rect;
	public int contentWidth;
	public int contentHeight;
	public float xMin;
	public float yMin;
	GUIContent content;
	public Grid(int contentWidth, 
	            int contentHeight , 
	            Rect initiationPosition, 
	            float patchWidth, 
	            float patchHeight)
	{
		this.patchHeight = patchHeight;
		this.patchWidth = patchWidth;
        this.basePatchHeight = patchHeight;
        this.basePatchWidth = patchWidth;
		rect = initiationPosition;
		content = GUIContent.none;
		yMin = (int)rect.yMin;
		xMin = (int)rect.xMin;
		this.contentWidth = contentWidth;
		this.contentHeight = contentHeight;
	}
	public Vector2 FilterPosition(Vector2 position , float gridScale = 1)
	{
		position = new Vector2 (position.x - rect.xMin, position.y - rect.yMin);
        float fixedPosX = (Mathf.FloorToInt(position.x / (patchWidth / gridScale)) * (patchWidth / gridScale));
        float fixedPosY = (Mathf.FloorToInt(position.y / (patchHeight / gridScale)) * (patchHeight / gridScale));
        return new Vector2 (fixedPosX + rect.xMin, fixedPosY + rect.yMin);
        
	}
	public void Draw()
	{
		GUI.color = new Color(1,1,1,0.1f);
		GUI.Box(rect,
		        content);
		for (float i = 0; i < contentWidth; i += patchWidth)
		{
			if(xMin + rect.xMin + i >= rect.xMin  && xMin + rect.xMin + i < rect.xMax)
				GUI.DrawTexture(new Rect(xMin + rect.xMin + i, rect.yMin, 1, rect.height), 
				                Resources.Load<Texture>("HLine"));
		}
		for (float i = 0; i < contentHeight; i += patchHeight)
		{
			if(yMin + rect.yMin  + i >= rect.yMin && yMin + rect.yMin + i < rect.yMax)
				GUI.DrawTexture(new Rect(rect.xMin, yMin + rect.yMin + i, rect.width, 1), 
				                Resources.Load<Texture>("VLine"));
		}
		GUI.color = Color.white;
	}
}
