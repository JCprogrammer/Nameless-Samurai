using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(ChunkGenerator))]
public class ChunkGeneratorEditor : Editor
{

    bool[] chunks;
    string searchPhrase = "";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Search: ",GUILayout.Width(60));
        searchPhrase = GUILayout.TextField(searchPhrase);
        GUILayout.EndHorizontal();
        string[] savedChunkNames = CubeFile.CheckDirectory("SavedFiles/Chunks");
        if (chunks == null)
            chunks = new bool[savedChunkNames.Length];


        for (int j = 0; j < savedChunkNames.Length; j++)
        {
            if (!savedChunkNames[j].Contains(searchPhrase))
                continue;
            Chunk c1 = CubeFile.DeSerializeObject<Chunk>(savedChunkNames[j], "Chunks");
            Rect ChunksRect = GUILayoutUtility.GetRect(200, 200);
            GUI.Box(ChunksRect, GUIContent.none);
            Vector2 Offset = new Vector2(ChunksRect.xMin + 50, ChunksRect.yMin + 50);
            for (int i = 0; i < 10; i++)
            {
                foreach (var chunkObject in c1.objects)
                {
                    if (chunkObject.position.depth != ((i + 1) * 0.5f))
                        continue;
                    Rect tmpRect = new Rect(Offset.x + (chunkObject.position.left) * 0.5f,
                                            Offset.y + (chunkObject.position.top) * 0.5f,
                                            chunkObject.position.width * 0.5f,
                                            chunkObject.position.height * 0.5f);
                    GUI.Label(tmpRect, Resources.Load<Texture>("Objects/" + chunkObject.texture));

                }
            }

            Color guiDefColor = GUI.color;
            if (chunks[j])
                GUI.color = Color.green;
            else
                GUI.color = Color.red;
            if (GUI.Button(new Rect(ChunksRect.xMax - 105, ChunksRect.yMax - 24, 100, 20), !chunks[j] ? "Select" : "Deselect"))
                chunks[j] = true ^ chunks[j];
            GUI.color = guiDefColor;
            GUI.Label(new Rect(ChunksRect.xMax - 210, ChunksRect.yMax - 24, 100, 20), c1.Name);
        }
        
    }
}
