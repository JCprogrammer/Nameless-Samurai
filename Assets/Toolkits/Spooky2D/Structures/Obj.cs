using UnityEngine;
using System.Collections;

public class Obj {

	public static int objectCount;
	public Vector2 size;
	public int id;
	public string name;
	public Texture texture;
	public Obj(string name,
	               Vector2 size,
	               Texture associatedTexture)
	{
		this.name = name;
		this.size = size;
		this.texture = associatedTexture;
		id = objectCount;
		objectCount++;
	}
}
