using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
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
        if (generator.chunksSelected.Length < savedChunkNames.Length)
            generator.chunksSelected = new bool[savedChunkNames.Length];

        float maxX = 0;
        float minX = 0;
        float maxY = 0;
        float minY = 0;
        for (int j = 0; j < savedChunkNames.Length; j++)
        {
            if (!savedChunkNames[j].Contains(searchPhrase))
                continue;
            if (!(showSelecteds && showNotSelecteds))
                if ((showSelecteds && !generator.chunksSelected[j]) || (showNotSelecteds && generator.chunksSelected[j])
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
            if (generator.chunksSelected[j])
                GUI.color = Color.green;
            else
                GUI.color = Color.red;
            if (GUI.Button(new Rect(ChunksRect.xMax - 105, ChunksRect.yMax - 24, 100, 20), !generator.chunksSelected[j] ? "Select" : "Deselect"))
                generator.chunksSelected[j] = true ^ generator.chunksSelected[j];
            GUI.color = guiDefColor;
            GUI.Label(new Rect(ChunksRect.xMax - 210, ChunksRect.yMax - 24, 100, 20), c1.Name);
            GUI.Label(new Rect(ChunksRect.xMax - 400, ChunksRect.yMax - 24, 200, 20), "Chunk Size: " +(maxX - minX).ToString() + " X " + (maxY - minY).ToString());
        }
        
    }

    GameObject CreateSceneObject(LevelObj item, Transform parent = null)
    {
        bool isChunk = false;
        if (item is Chunk)
        {
            Debug.Log("Is Chunk");

            GameObject ChunkContainer = new GameObject(((Chunk)item).Name + "." + item.id.ToString());

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
            DestroyImmediate(ChunkContainer);
            return (AssetDatabase.LoadAssetAtPath("Assets/ChunkPrefabs/Prefabs/" + ((Chunk)item).Name + "." + item.id.ToString() + ".prefab", typeof(GameObject)) as GameObject);
        }
        else
        {
            GameObject sceneObj;
            MeshFilter tmpFilter;
            MeshRenderer tmpRenderer;
            Material objMaterial = AssetDatabase.LoadAssetAtPath("Assets/ChunkPrefabs/Materials/" + item.texture + item.id.ToString() + ".mat", typeof(Material)) as Material;
            GameObject sceneObjContainer = GameObject.Find(item.texture + "." + item.id.ToString());
            if (sceneObjContainer == null || parent != null)
            {
                sceneObjContainer = new GameObject(item.texture + "." + item.id.ToString());
                sceneObj = new GameObject(item.texture + "." + item.id.ToString() + "Sprite");
                tmpFilter = sceneObj.AddComponent<MeshFilter>();
                tmpRenderer = sceneObj.AddComponent<MeshRenderer>();
                if (objMaterial == null)
                {
                    objMaterial = new Material(Shader.Find("Transparent/Diffuse"));
                    AssetDatabase.CreateAsset(objMaterial, "Assets/ChunkPrefabs/Materials/" + item.texture + item.id.ToString() + ".mat");
                }
            }
            else
            {
                sceneObj = sceneObjContainer.transform.GetChild(0).gameObject;
                tmpFilter = sceneObj.GetComponent<MeshFilter>();
                tmpRenderer = sceneObj.GetComponent<MeshRenderer>();
                objMaterial = tmpRenderer.material;
            }

            string meshName = item.position.width.ToString() + "X" + item.position.height;
            Mesh mesh = AssetDatabase.LoadAssetAtPath("Assets/ChunkPrefabs/" + meshName + ".asset", typeof(Mesh)) as Mesh;

            if (mesh == null)
            {
                mesh = new Mesh();
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
                AssetDatabase.CreateAsset(mesh, "Assets/ChunkPrefabs/" + meshName + ".asset");
            }




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
            string chunkObjName = "";

            sceneObjContainer.transform.position += parent.position - new Vector3(25 / GlobalVariables.minifier, 50 / GlobalVariables.minifier, 0);
            sceneObjContainer.transform.parent = parent;
            chunkObjName = parent.name;
            PrefabUtility.CreatePrefab("Assets/ChunkPrefabs/Prefabs/" + parent.name + ".prefab", parent.gameObject);


            return null;
        }
        
       
    }

    void BuildChunkPrefab(string[] chunkNames)
    {
       
         ChunkGenerator generator = target as ChunkGenerator;
         generator.chunks = new List<GameObject>();
        for (int i = 0; i < chunkNames.Length; i++)
        {
            if (!generator.chunksSelected[i])
                continue;
            generator.chunks.Add(CreateSceneObject(CubeFile.DeSerializeObject<Chunk>(chunkNames[i], "Chunks")));
            
        }
    }
}
