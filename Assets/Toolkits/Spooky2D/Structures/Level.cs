using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ObjRect
{
	public int left;
	public int top;
	public int width;
	public int height;
	public int depth;

	public ObjRect(int left, int top,int width,int height)
	{
		this.left = left;
		this.top = top;
		this.width = width;
		this.height = height;
		depth = 0;
	}
    public ObjRect(int left, int top, int width, int height , int depth) : this(left,top,width,height)
    {
        this.depth = depth;
    }
}
[System.Serializable]
public class Level{

    public ObjRect cameraPivot;
    public string name;
    public int width;
    public int height;
	public int idAccumulator;
	public  List<LevelObj> objects;

    public Level(string name, 
	             int width, 
	             int height)
    {

		objects = new List<LevelObj> (0);
        this.name = name;
        this.width = width;
        this.height = height;
		idAccumulator = 0;  
    }
}

[System.Serializable]
public class LevelObj
{
	public string texture;
	public ObjRect position;
	public int id;
    public string isCloneOf;
    public LevelObj(Rect position,int depth,int id,string parentName = "none")
    {
        texture = null;
        this.position = new ObjRect(0, 0, 0, 0);
        this.position.left = (int)position.xMin;
        this.position.top = (int)position.yMin;
        this.position.width = (int)position.width;
        this.position.height = (int)position.height;
        this.position.depth = depth;
        this.id = id;
        this.isCloneOf = parentName;
    }
	public LevelObj(Rect position, Texture texture,int id,int depth,string parentName = "none"): 
        this(position,depth,id,parentName)
	{
		this.texture = texture.name;
	}

}
[System.Serializable]
public class Chunk : LevelObj
{

    public bool isSaved;
    public int layerNumber;
    public int idAccumulator;
    public string Name;
    public List<LevelObj> objects;
    public ObjRect centerOfChunk;
    public Chunk(string name, Rect position, int id, int depth, List<LevelObj> objects, string parentName = "none" ) :
        base(position,depth,id,parentName)
    {
        isSaved = false;
        Name = name;
        idAccumulator = 0;
        layerNumber = depth;
        if (objects == null)
            this.objects = new List<LevelObj>();
        else
            this.objects = objects;
        if (this.objects.Count > 0)
        {
            Vector2 min = new Vector2(this.objects[0].position.left, this.objects[0].position.top);
            Vector2 max = new Vector2(this.objects[0].position.left + this.objects[0].position.width,
                                      this.objects[0].position.top + this.objects[0].position.height);
            foreach (var item in this.objects)
            {

                Vector2 tempMin = new Vector2(item.position.left, item.position.top);
                if (tempMin.magnitude < min.magnitude)
                    min = tempMin;
                Vector2 tempMax = new Vector2(item.position.left + item.position.width,
                                              item.position.top + item.position.height);
                if (tempMax.magnitude > max.magnitude)
                    max = tempMax;
                idAccumulator = 0;

            }
            centerOfChunk = new ObjRect((int)(max.x - min.x), (int)(max.y - min.y), 50, 50);
        }
        else
            centerOfChunk = new ObjRect(0,0,0,0);
    }

    public ObjRect CenterOfChunk
    {
        get { 
        if (this.objects.Count > 0)
        {
            Vector2 min = new Vector2(this.objects[0].position.left, this.objects[0].position.top);
            Vector2 max = new Vector2(this.objects[0].position.left + this.objects[0].position.width,
                                      this.objects[0].position.top + this.objects[0].position.height);
            foreach (var item in this.objects)
            {

                Vector2 tempMin = new Vector2(item.position.left, item.position.top);
                if (tempMin.x < min.x)
                    min = new Vector2(tempMin.x,min.y);
                Vector2 tempMax = new Vector2(item.position.left + item.position.width,
                                              item.position.top + item.position.height);
                if (tempMax.x > max.x)
                    max = new Vector2(tempMax.x,max.y);
               
            }
            foreach (var item in this.objects)
            {

                Vector2 tempMin = new Vector2(item.position.left, item.position.top);
                if (tempMin.y < min.y)
                    min = new Vector2(min.x, tempMin.y);
                Vector2 tempMax = new Vector2(item.position.left + item.position.width,
                                              item.position.top + item.position.height);
                if (tempMax.y > max.y)
                    max = new Vector2(max.x, tempMax.y);

            }
            centerOfChunk = new ObjRect((int)(min.x + (max.x - min.x)/2), (int)(min.y + (max.y - min.y)/2), 50, 50);
        }
        else
            centerOfChunk = new ObjRect(0,0,0,0);
        return centerOfChunk;
        }

    }
}

