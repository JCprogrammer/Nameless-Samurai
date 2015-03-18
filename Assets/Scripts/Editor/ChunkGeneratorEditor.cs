using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(ChunkGenerator))]
public class ChunkGeneratorEditor : Editor
{

    // bool[] generator.chunks;
    string searchPhrase = "";
    bool showSelecteds = true;
    bool showNotSelecteds = true;
    bool showPreview = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Box("Remember, in order to better see previews, put the CoC on top-left corner of the chunk.");
        ChunkGenerator generator = target as ChunkGenerator;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search: ",GUILayout.Width(60));
        searchPhrase = GUILayout.TextField(searchPhrase);
        GUILayout.EndHorizontal();
       
        GUILayout.BeginHorizontal();
        showNotSelecteds = GUILayout.Toggle(showNotSelecteds, "ShowNotSelecteds");
        showSelecteds = GUILayout.Toggle(showSelecteds, "ShowSelecteds");
        showPreview = GUILayout.Toggle(showPreview, "Show Preview");
        GUILayout.EndHorizontal();
        string[] savedChunkNames = CubeFile.CheckDirectory("SavedFiles/Chunks");
        if (GUILayout.Button("Build Prefabs"))
            BuildChunkPrefab(savedChunkNames);
        if (generator.chunks.Length < savedChunkNames.Length)
            generator.chunks = new bool[savedChunkNames.Length];

        float maxX = 0;
        float minX = 0;
        float maxY = 0;
        float minY = 0;
        for (int j = 0; j < savedChunkNames.Length; j++)
        {
            if (!savedChunkNames[j].Contains(searchPhrase))
                continue;
            if (!(showSelecteds && showNotSelecteds))
                if ((showSelecteds && !generator.chunks[j]) || (showNotSelecteds && generator.chunks[j])
                    || (!showSelecteds && !showNotSelecteds))
                    continue;
            Chunk c1 = CubeFile.DeSerializeObject<Chunk>(savedChunkNames[j], "Chunks");
            Rect ChunksRect;
            if (showPreview)
            {
                maxX = c1.objects[0].position.left + c1.objects[0].position.width;
                minX = c1.objects[0].position.left;
                maxY = c1.objects[0].position.top + c1.objects[0].position.height;
                minY = c1.objects[0].position.top;
                foreach (var item in c1.objects)
                {
                    maxX = Mathf.Max(maxX, item.position.left + item.position.width);
                    minX = Mathf.Min(minX, item.position.left);
                    maxY = Mathf.Max(maxY, item.position.top + item.position.height);
                    minY = Mathf.Min(minY, item.position.top);
                }

                float scaler = (maxY-minY > maxX-minX ? 450.0f : 300.0f) / Mathf.Max((maxX - minX), (maxY - minY));
                ChunksRect = GUILayoutUtility.GetRect(400, 200);
                GUI.Box(ChunksRect, GUIContent.none);
                Vector2 Offset = new Vector2(ChunksRect.xMin + 20, ChunksRect.yMin + 20);
                for (int i = 0; i < 10; i++)
                {
                    foreach (var chunkObject in c1.objects)
                    {
                        if (chunkObject.position.depth != ((i + 1) * 0.5f))
                            continue;
                        Rect tmpRect = new Rect(Offset.x + (chunkObject.position.left) * scaler,
                                                Offset.y + (chunkObject.position.top) * scaler,
                                                chunkObject.position.width * scaler,
                                                chunkObject.position.height * scaler);
                        GUI.Label(tmpRect, Resources.Load<Texture>("Objects/" + chunkObject.texture));

                    }
                }
            }
            else
                ChunksRect = GUILayoutUtility.GetRect(200, 30);
            Color guiDefColor = GUI.color;
            if (generator.chunks[j])
                GUI.color = Color.green;
            else
                GUI.color = Color.red;
            if (GUI.Button(new Rect(ChunksRect.xMax - 105, ChunksRect.yMax - 24, 100, 20), !generator.chunks[j] ? "Select" : "Deselect"))
                generator.chunks[j] = true ^ generator.chunks[j];
            GUI.color = guiDefColor;
            GUI.Label(new Rect(ChunksRect.xMax - 210, ChunksRect.yMax - 24, 100, 20), c1.Name);
            GUI.Label(new Rect(ChunksRect.xMax - 400, ChunksRect.yMax - 24, 200, 20), "Chunk Size: " +(maxX - minX).ToString() + " X " + (maxY - minY).ToString());
        }
        
    }

    void CreateSceneObject(LevelObj item, Transform parent = null)
    {

        if (item is Chunk)
        {
            Debug.Log("Is Chunk");
            GameObject ChunkContainer = GameObject.Find(((Chunk)item).Name + "." + item.id.ToString());
            if (ChunkContainer == null)
                ChunkContainer = new GameObject(((Chunk)item).Name + "." + item.id.ToString());
            else
                return;
            ChunkContainer.transform.position = new Vector3(1 * (float)(((Chunk)item).centerOfChunk.left) /
                                                 GlobalVariables.minifier, -1 * (float)(((Chunk)item).centerOfChunk.top) /
                                                 GlobalVariables.minifier, 0) + new Vector3(20.0f / GlobalVariables.minifier, 40.0f / GlobalVariables.minifier, item.position.depth);
            Debug.Log("Number of objects: " + ((Chunk)item).objects.Count + "Center of Chunk" + ((Chunk)item).centerOfChunk.top);

            ChunkData cData = ChunkContainer.AddComponent<ChunkData>();
            float xMin = ((Chunk)item).centerOfChunk.left + ((Chunk)item).objects[0].position.left;
            float xMax = ((Chunk)item).centerOfChunk.left + ((Chunk)item).objects[0].position.left + ((Chunk)item).objects[0].position.width;
            Debug.Log(xMax);
            foreach (var item2 in ((Chunk)item).objects)
            {
                if (item2.position.left + ((Chunk)item).centerOfChunk.left < xMin)
                    xMin = item2.position.left + ((Chunk)item).centerOfChunk.left;
                if (item2.position.left + item2.position.width + ((Chunk)item).centerOfChunk.left > xMax)
                    xMax = item2.position.left + item2.position.width + ((Chunk)item).centerOfChunk.left;
                CreateSceneObject(item2, ChunkContainer.transform);

            }
            Debug.Log(xMax + " " + xMin);
            cData.chunkLength = (xMax - xMin) / GlobalVariables.minifier;
            return;
        }
        GameObject sceneObj;
        MeshFilter tmpFilter;
        MeshRenderer tmpRenderer;
        Material objMaterial;
        GameObject sceneObjContainer = GameObject.Find(item.texture + "." + item.id.ToString());
        if (sceneObjContainer == null || parent != null)
        {
            sceneObjContainer = new GameObject(item.texture + "." + item.id.ToString());
            sceneObj = new GameObject(item.texture + "." + item.id.ToString() + "Sprite");
            tmpFilter = sceneObj.AddComponent<MeshFilter>();
            tmpRenderer = sceneObj.AddComponent<MeshRenderer>();
            objMaterial = new Material(Resources.Load<Material>("SourceMaterial"));
            
        }
        else
        {
            sceneObj = sceneObjContainer.transform.GetChild(0).gameObject;
            tmpFilter = sceneObj.GetComponent<MeshFilter>();
            tmpRenderer = sceneObj.GetComponent<MeshRenderer>();
            objMaterial = tmpRenderer.material;
        }

        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), 
			new Vector3(0, (float)item.position.height/GlobalVariables.minifier, 0), 
			new Vector3((float)item.position.width/GlobalVariables.minifier, 0 , 0), 
			new Vector3((float)item.position.width/GlobalVariables.minifier, (float)item.position.height/GlobalVariables.minifier,0)
			};

        mesh.uv = new Vector2[] {new Vector2(0, 0), 
			new Vector2(0, 1), 
			new Vector2(1,0) , 
			new Vector2(1, 1)};
        mesh.triangles = new int[] { 0, 1, 2, 3, 2, 1 };
        mesh.normals = new Vector3[] { new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1) };
        //mesh.bounds = new Bounds (new Vector2 (mesh.bounds.center.x + (mesh.bounds.size.x / 2), mesh.bounds.center.y + (mesh.bounds.size.y / 2)), mesh.bounds.size);
       
        tmpFilter.mesh = mesh;
        objMaterial.SetTexture(0, Resources.Load<Texture>("Objects/" + item.texture));
        Shader defShader = Shader.Find("Transparent/Diffuse");

        objMaterial.shader = defShader;
        objMaterial.color = new Color(1, 1, 1);
        tmpRenderer.material = objMaterial;

        Camera levelCamera = SceneView.FindObjectOfType<Camera>();

        Debug.Log("______________________ Screen to world point: " + levelCamera.ScreenToWorldPoint(new Vector3(item.position.left, item.position.top, 0)));

        sceneObj.transform.parent = sceneObjContainer.transform;
        if ((sceneObjContainer.GetComponent<BoxCollider2D>() == null) && item.position.depth == 0)
            sceneObjContainer.AddComponent<BoxCollider2D>();

        if (sceneObjContainer.GetComponent<BoxCollider2D>() != null)
            sceneObjContainer.GetComponent<BoxCollider2D>().size = mesh.bounds.size;

        sceneObj.transform.localPosition = new Vector3(-1 * (float)item.position.width /
                                                        (GlobalVariables.minifier * 2), -1 * (float)item.position.height /
                                                        (GlobalVariables.minifier * 2), 0);

        sceneObjContainer.transform.position = new Vector3(1 * (float)item.position.left /
                                                           GlobalVariables.minifier, -1 * (float)item.position.top /
                                                           GlobalVariables.minifier, 0) + new Vector3((1 * (float)item.position.width /
                                                                                                        (GlobalVariables.minifier * 2)) + 1, (-1 * (float)item.position.height /
                                                                                                     (GlobalVariables.minifier * 2)) + 2, parent == null ? item.position.depth : parent.position.z - item.position.depth);
        if (parent != null)
        {
            sceneObjContainer.transform.position += parent.position - new Vector3(25 / GlobalVariables.minifier, 50 / GlobalVariables.minifier, 0);
            sceneObjContainer.transform.parent = parent;
        }
    }

    void BuildChunkPrefab(string[] chunkNames)
    {
         ChunkGenerator generator = target as ChunkGenerator;
        for (int i = 0; i < chunkNames.Length; i++)
        {
            if (!generator.chunks[i])
                continue;
            CreateSceneObject(CubeFile.DeSerializeObject<Chunk>(chunkNames[i], "Chunks"));
        }
    }
}
