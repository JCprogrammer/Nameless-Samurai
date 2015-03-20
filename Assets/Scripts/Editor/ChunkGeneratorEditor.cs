using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(ChunkGenerator))]
public class ChunkGeneratorEditor : Editor
{

    // bool[] generator.chunks;
    List<ChunkGenerator.LayerSectionInformation> layersInfo;
    void Initiate()
    {
        ChunkGenerator generator = target as ChunkGenerator;

        layersInfo = generator.layersInfo;
        if (layersInfo.Count > 0)
            return;
        else
            Debug.Log("Count less or equal to 0");
        generator.layersInfo = new List<ChunkGenerator.LayerSectionInformation>();
        for (int i = 0; i < 10; i++)
        {
            generator.layersInfo.Add(new ChunkGenerator.LayerSectionInformation());
        }
        
        if (generator.levelLayers.Count > 0)
            return;
        generator.levelLayers = new List<LevelLayer>(0);
        for (int i = 0; i < 10; i++)
        {
            generator.levelLayers.Add(new LevelLayer());
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Initiate();
        if (GUILayout.Button("Scan"))
            ScanForLayers();
        if (GUILayout.Button("Instantiate"))
            Generate();
        ChunkGenerator generator = target as ChunkGenerator;
        for (int i = 0; i < 10; i++)
        {
            if (generator.visibleLayers[i])
                CreateLayerSection(i);
            else
                CreateMiniLevelSection(i);
        }
    }
    void CreateMiniLevelSection(int index)
    {
        ChunkGenerator generator = target as ChunkGenerator;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Resources.Load<Texture>("WhiteTriangle"), GUILayout.Width(20), GUILayout.Height(20)))
            generator.visibleLayers[index] = true;
        GUILayout.Box("Layer " + index.ToString());
        GUILayout.EndHorizontal();
    }
    void CreateLayerSection(int index)
    {
        ChunkGenerator generator = target as ChunkGenerator;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(Resources.Load<Texture>("RedTriangle"), GUILayout.Width(20), GUILayout.Height(20)))
            generator.visibleLayers[index] = false;
        GUILayout.Box("Layer " + index.ToString());
        GUILayout.EndHorizontal();
        GUILayout.Box("Remember, in order to better see previews, put the CoC on top-left corner of the chunk.");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Search: ", GUILayout.Width(60));
        layersInfo[index].searchPhrase = GUILayout.TextField(layersInfo[index].searchPhrase);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        layersInfo[index].showNotSelecteds = GUILayout.Toggle(layersInfo[index].showNotSelecteds, "ShowNotSelecteds");
        layersInfo[index].showSelecteds = GUILayout.Toggle(layersInfo[index].showSelecteds, "ShowSelecteds");
        layersInfo[index].showPreview = GUILayout.Toggle(layersInfo[index].showPreview, "Show Preview");
        GUILayout.EndHorizontal();
        string[] savedChunkNames = CubeFile.CheckDirectory("SavedFiles/Chunks");
        if (GUILayout.Button("Build Prefabs"))
            BuildChunkPrefab(savedChunkNames, index);
        if (generator.levelLayers[index].chunksSelected.Length < savedChunkNames.Length)
            generator.levelLayers[index].chunksSelected = new bool[savedChunkNames.Length];

        float maxX = 0;
        float minX = 0;
        float maxY = 0;
        float minY = 0;
        if (generator.levelLayers[index].hSpeed.Length == 0)
            generator.levelLayers[index].hSpeed = new float[savedChunkNames.Length];
        for (int j = 0; j < savedChunkNames.Length; j++)
        {
            if (!savedChunkNames[j].Contains(layersInfo[index].searchPhrase))
                continue;
            if (!(layersInfo[index].showSelecteds && layersInfo[index].showNotSelecteds))
                if ((layersInfo[index].showSelecteds && !generator.levelLayers[index].chunksSelected[j]) || (layersInfo[index].showNotSelecteds && generator.levelLayers[index].chunksSelected[j])
                    || (!layersInfo[index].showSelecteds && !layersInfo[index].showNotSelecteds))
                    continue;
            Chunk c1 = CubeFile.DeSerializeObject<Chunk>(savedChunkNames[j], "Chunks");
            Rect ChunksRect;
            if (layersInfo[index].showPreview)
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

                float scaler = (maxY - minY > maxX - minX ? 450.0f : 300.0f) / Mathf.Max((maxX - minX), (maxY - minY));
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
            if (generator.levelLayers[index].chunksSelected[j])
                GUI.color = Color.green;
            else
                GUI.color = Color.red;
            if (GUI.Button(new Rect(ChunksRect.xMax - 105, ChunksRect.yMax - 24, 100, 20), !generator.levelLayers[index].chunksSelected[j] ? "Select" : "Deselect"))
                generator.levelLayers[index].chunksSelected[j] = true ^ generator.levelLayers[index].chunksSelected[j];
            GUI.color = guiDefColor;
            GUI.Label(new Rect(ChunksRect.xMax - 210, ChunksRect.yMax - 24, 100, 20), c1.Name); 
            GUI.Label(new Rect(ChunksRect.xMax - 400, ChunksRect.yMax - 44, 100, 20), "Speed: ");
            generator.levelLayers[index].hSpeed[j] = float.Parse(GUI.TextField(new Rect(ChunksRect.xMax - 300, ChunksRect.yMax - 44,100 , 20), generator.levelLayers[index].hSpeed[j].ToString()));
            GUI.Label(new Rect(ChunksRect.xMax - 400, ChunksRect.yMax - 24, 200, 20), "Chunk Size: " + (maxX - minX).ToString() + " X " + (maxY - minY).ToString());
        }

    }
    GameObject CreateSceneObject(LevelObj item, Transform parent = null)
    {
      
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
            ChunkContainer.GetComponent<ChunkData>().chunkLength = (xMax - xMin) / GlobalVariables.minifier;

           
            GameObject sample = AssetDatabase.LoadAssetAtPath("Assets/ChunkPrefabs/Prefabs/" + ((Chunk)item).Name + "." + item.id.ToString() + ".prefab", typeof(GameObject)) as GameObject;
            if(sample == null)
                PrefabUtility.CreatePrefab("Assets/ChunkPrefabs/Prefabs/" + ChunkContainer.name + ".prefab", ChunkContainer.gameObject);

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
         
            sceneObjContainer.transform.position += parent.position - new Vector3(25 / GlobalVariables.minifier, 50 / GlobalVariables.minifier, 0);
            sceneObjContainer.transform.parent = parent;
         
         
            return null;
        }
        
       
    }
    void BuildChunkPrefab(string[] chunkNames ,int index)
    {
       
         ChunkGenerator generator = target as ChunkGenerator;
         generator.levelLayers[index].chunks = new List<GameObject>();
        for (int i = 0; i < chunkNames.Length; i++)
        {
            if (!generator.levelLayers[index].chunksSelected[i])
                continue;
            generator.levelLayers[index].chunks.Add(CreateSceneObject(CubeFile.DeSerializeObject<Chunk>(chunkNames[i], "Chunks")));
            
        }
    }

    void Generate()
    {
        ChunkGenerator generator = target as ChunkGenerator;
        for (int i = 0; i < generator.levelLayers.Count; i++)
        {
            if (generator.levelLayers[i].chunks.Count == 0)
                continue;
            int randomIndex = Random.Range(0, generator.levelLayers[i].chunks.Count);
            GameObject sample = Instantiate(generator.levelLayers[i].chunks[randomIndex], generator.levelLayers[i].initiationPointOfGeneration,Quaternion.identity) as GameObject;
            generator.levelLayers[i].initiationPointOfGeneration += new Vector3(sample.GetComponent<ChunkData>().chunkLength, 0, 0);
        }

    }
    void ScanForLayers()
    {
        ChunkData[] levelObjects;
        levelObjects = GameObject.FindObjectsOfType<ChunkData>();
        List<ChunkData>[] gObjects = new List<ChunkData>[10];
        for (int i = 0; i < 10; i++)
        {
            gObjects[i] = new List<ChunkData>();
            foreach (var item in levelObjects)
            {
                if (item.transform.position.z == 40 - ((i + 1) * 5))
                {
                    gObjects[i].Add(item);
                }

            }
            PinInitiationPoint(i, gObjects[i]);
            Debug.Log("There are " + gObjects[i].Count + " Chunks in Layer " + (i+1).ToString());
        }



    }

    void PinInitiationPoint(int layerIndex, List<ChunkData> chunks)
    {
        ChunkGenerator generator = target as ChunkGenerator;
        if (chunks.Count == 0)
            return;
        float maxX = chunks[0].transform.position.x + chunks[0].chunkLength;
        generator.levelLayers[layerIndex].currentChunk = chunks[0].gameObject;
        foreach (var item in chunks)
        {
            if (item.transform.position.x + item.chunkLength > maxX)
            {
                maxX = item.transform.position.x + item.chunkLength;
                generator.levelLayers[layerIndex].currentChunk = item.gameObject;
            }
        }
        generator.levelLayers[layerIndex].initiationPointOfGeneration = new Vector3(maxX, chunks[0].transform.position.y, chunks[0].transform.position.z);
    }

}

