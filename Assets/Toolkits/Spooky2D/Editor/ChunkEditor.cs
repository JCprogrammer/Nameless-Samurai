﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Runtime.Serialization;
using System.Reflection;



public class ChuckEditor : EditorWindow
{
    ObjRect Canvas;
    public static Listbox TextureList;
    public static Listbox ChunkItemList;
    public static Listbox SavedChunksList;
    static Chunk chunk;
    static LevelObj objSelectorGrid;
    static LevelObj selectedLevelObj;
    static float hScroll;
    static float vScroll;
    static Grid grid;
    static ChuckEditor window;
    static bool noChangesApplied;
    static DynamicSelection dynamicSelection = new DynamicSelection();
    Rect grabbedObj;
    List<string> deletedItems;
    Vector2 lastGrabbedObjScale;
    static Texture selectedBlock;
    const int durationCap = 10;
    int counter;
    string TimeSpan = "0";
    bool MovingSeeker = false;
    bool ctrlSelected = false;
    bool[] layers;
    bool movingCenterOfChunk;
    bool autoAdjustCOC;
    string[] layersName;
    Rect seekerBoundry = new Rect(180, 85, 20, 22);
    public static List<ComponentGroup> serializedObjects;
    [MenuItem("Spooky Guys/Chunk Editor")]
    static void Init()
    {
        objSelectorGrid = new LevelObj(new Rect(0, 0, 0, 0), Resources.Load<Texture>("HexaGridRed"), 0, 0);
           
        grid = new Grid(800, 400, new Rect(215, 90, 500, 400), 40, 40);
        ChunkItemList = new Listbox(new string[] { "None" }, "LevelChunksHirarchy", new Rect(10, 480, 200, 140), 20);
        
        window = (ChuckEditor)EditorWindow.GetWindow(typeof(ChuckEditor));
        window.minSize = new Vector2(735, 650);
        chunk = new Chunk("NewChunk", new Rect(0, 0, 1, 1), 0, 0, new List<LevelObj>());
        string[] savedChunks = CubeFile.CheckDirectory("SavedFiles/Chunks");
        window.Canvas = new ObjRect(180, 90, 800, 400);
        window.movingCenterOfChunk = false;
        SavedChunksList = new Listbox(savedChunks, "SavedLevels", new Rect(10, 295, 200, 140), 20);
        window.autoAdjustCOC = true;
        foreach (var item in SavedChunksList.items)
        {
            item.action = window.LoadChunk;
        }
        foreach (var item in ChunkItemList.items)
        {
            item.action = window.SelectItemOnScene;
        }
        window.lastGrabbedObjScale = new Vector2(40, 40);
        window.deletedItems = new List<string>(0);
        window.grabbedObj = new Rect(0, 0, window.lastGrabbedObjScale.x, window.lastGrabbedObjScale.y);

        string[] availObjs = window.LoadAvailableObjects();
         TextureList = new Listbox(availObjs, "AvailableObjects", new Rect(10, 110, 200, 140), 20);
        foreach (var item in ChunkItemList.items)
        {
            item.action = window.changeSelectedBlockTexture;
        }
        window.SelectItemOnScene("");
        window.wantsMouseMove = true;
        window.changeSelectedBlockTexture("");
    }

    void MovingCOC()
    {
        if (movingCenterOfChunk)
        {
            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x,
                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y);
            if (grid.rect.Contains(filteredMousePosition))
            {
                Vector2 grabbedPosition = new Vector2(grid.FilterPosition(filteredMousePosition + new Vector2((int)hScroll, 0)).x,
                                              grid.FilterPosition(filteredMousePosition + new Vector2(0, (int)vScroll)).y);

                chunk.centerOfChunk = new ObjRect((int)grabbedPosition.x, (int)grabbedPosition.y, 40, 40);
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    movingCenterOfChunk = false;
                }
            }
        }
    }
    void clearSelection()
    {
        if (dynamicSelection.objects.Count > 0)
        {
            foreach (var item in dynamicSelection.objects)
            {

                item.position = new ObjRect(item.position.left + (int)(dynamicSelection.boundry.xMin - dynamicSelection.firstPos.x),
                    item.position.top + (int)(dynamicSelection.boundry.yMin - dynamicSelection.firstPos.y),
                    item.position.width,
                    item.position.height);
            }
        }
        dynamicSelection.drawRect = new Rect(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x + (int)hScroll,
                          HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y + (int)vScroll, 0, 0);
        if (dynamicSelection.isClonning)
            chunk.objects.AddRange(dynamicSelection.objects);
        dynamicSelection.objects = new List<LevelObj>();
    }
    void AssociativeSeletion()
    {
        if (!Event.current.control)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
            {
                clearSelection();
                dynamicSelection.isClonning = false;
            }
            else if (Event.current.button == 0 && Event.current.type == EventType.mouseDrag)
            {
                dynamicSelection.drawRect = new Rect(dynamicSelection.drawRect.xMin,
                                                     dynamicSelection.drawRect.yMin,
                                                     HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x + (int)hScroll - dynamicSelection.drawRect.xMin,
                                                     HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y + (int)vScroll - dynamicSelection.drawRect.yMin);


            }
            else if (Event.current.type == EventType.mouseUp)
            {

                float max = Mathf.Max(dynamicSelection.drawRect.xMin, dynamicSelection.drawRect.xMax);
                dynamicSelection.drawRect.xMin = Mathf.Min(dynamicSelection.drawRect.xMax, dynamicSelection.drawRect.xMin);
                dynamicSelection.drawRect.xMax = max;
                max = Mathf.Max(dynamicSelection.drawRect.yMin, dynamicSelection.drawRect.yMax);
                dynamicSelection.drawRect.yMin = Mathf.Min(dynamicSelection.drawRect.yMax, dynamicSelection.drawRect.yMin);
                dynamicSelection.drawRect.yMax = max;
                foreach (LevelObj item in chunk.objects)
                {
                    if (dynamicSelection.drawRect.Contains(new Vector2(item.position.left, item.position.top)))
                    {
                        dynamicSelection.objects.Add(item);

                        if (dynamicSelection.objects.Count == 1)
                        {
                            dynamicSelection.boundry = ChangeType(item.position);
                        }
                        else if (dynamicSelection.objects.Count > 1)
                        {
                            dynamicSelection.boundry.xMin = Mathf.Min(dynamicSelection.boundry.xMin, ChangeType(item.position).xMin);
                            dynamicSelection.boundry.xMax = Mathf.Max(dynamicSelection.boundry.xMax, ChangeType(item.position).xMax);
                            dynamicSelection.boundry.yMin = Mathf.Min(dynamicSelection.boundry.yMin, ChangeType(item.position).yMin);
                            dynamicSelection.boundry.yMax = Mathf.Max(dynamicSelection.boundry.yMax, ChangeType(item.position).yMax);
                        }
                        dynamicSelection.firstPos = new Vector2(dynamicSelection.boundry.xMin, dynamicSelection.boundry.yMin);
                    }
                }

                if (dynamicSelection.objects.Count > 0)
                {
                    dynamicSelection.drawRect = dynamicSelection.boundry;
                }
                else
                    dynamicSelection.drawRect = new Rect();

            }



        }
        else if (Event.current.type == EventType.mouseDrag)
        {
            Vector2 tmpV = grid.FilterPosition(new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x + (int)hScroll,
                                   HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y + (int)vScroll));
            dynamicSelection.boundry = new Rect(tmpV.x,
                                   tmpV.y, dynamicSelection.drawRect.width, dynamicSelection.drawRect.height);

            dynamicSelection.drawRect = dynamicSelection.boundry;

        }
        else if (Event.current.shift)
        {
            if (!dynamicSelection.isClonning)
            {

                List<LevelObj> tmpObjList = new List<LevelObj>();
                foreach (var item in dynamicSelection.objects)
                {
                    tmpObjList.Add(new LevelObj(ChangeType(item.position),
                        Resources.Load<Texture>("Objects/" + item.texture),
                        chunk.idAccumulator, chunk.layerNumber,
                        item.texture + "." + item.id.ToString()));
                    if (GameObject.Find(item.texture + "." + item.id.ToString()).GetComponents<Component>().Length > 0)
                    {
                        serializedObjects.Add(new ComponentGroup(item.texture + "." + item.id.ToString(),
                                                 GameObject.Find(item.texture + "." + item.id.ToString()).GetComponents<Component>()));
                    }
                    chunk.idAccumulator++;
                }
                chunk.objects.AddRange(tmpObjList);
                dynamicSelection.objects = tmpObjList;
                dynamicSelection.isClonning = true;

            }
        }
        else if (Event.current.type == EventType.keyUp && Event.current.keyCode == KeyCode.A)
        {
            float max = Mathf.Max(dynamicSelection.drawRect.xMin, dynamicSelection.drawRect.xMax);
            dynamicSelection.drawRect.xMin = Mathf.Min(dynamicSelection.drawRect.xMax, dynamicSelection.drawRect.xMin);
            dynamicSelection.drawRect.xMax = max;
            max = Mathf.Max(dynamicSelection.drawRect.yMin, dynamicSelection.drawRect.yMax);
            dynamicSelection.drawRect.yMin = Mathf.Min(dynamicSelection.drawRect.yMax, dynamicSelection.drawRect.yMin);
            dynamicSelection.drawRect.yMax = max;
            foreach (LevelObj item in chunk.objects)
            {

                dynamicSelection.objects.Add(item);

                if (dynamicSelection.objects.Count == 1)
                {
                    dynamicSelection.boundry = ChangeType(item.position);
                }
                else if (dynamicSelection.objects.Count > 1)
                {
                    dynamicSelection.boundry.xMin = Mathf.Min(dynamicSelection.boundry.xMin, ChangeType(item.position).xMin);
                    dynamicSelection.boundry.xMax = Mathf.Max(dynamicSelection.boundry.xMax, ChangeType(item.position).xMax);
                    dynamicSelection.boundry.yMin = Mathf.Min(dynamicSelection.boundry.yMin, ChangeType(item.position).yMin);
                    dynamicSelection.boundry.yMax = Mathf.Max(dynamicSelection.boundry.yMax, ChangeType(item.position).yMax);
                }
                dynamicSelection.firstPos = new Vector2(dynamicSelection.boundry.xMin, dynamicSelection.boundry.yMin);

            }

            if (dynamicSelection.objects.Count > 0)
            {
                dynamicSelection.drawRect = dynamicSelection.boundry;
            }
            else
                dynamicSelection.drawRect = new Rect();
        }
        Rect tmpR = new Rect(dynamicSelection.drawRect.xMin - (int)hScroll,
                                    dynamicSelection.drawRect.yMin - (int)vScroll,
                                    dynamicSelection.drawRect.width,
                                    dynamicSelection.drawRect.height);
        if (!(tmpR.xMin < grid.rect.xMin ||
             tmpR.yMin < grid.rect.yMin ||
             tmpR.xMax > grid.rect.xMax ||
             tmpR.yMax > grid.rect.yMax))
        {
            Color dColor = GUI.color;
            GUI.color = Color.blue;
            GUI.Box(tmpR, GUIContent.none);
            GUI.Box(tmpR, GUIContent.none);
            GUI.color = dColor;
        }
    }
    public void changeSelectedBlockTexture(string parameter)
    {
        selectedBlock = Resources.Load<Texture>("Objects/" + TextureList.getSelectedItemContent());
    }
    public void SelectItemOnScene(string item)
    {
        if (ChunkItemList.items.Count > 0)
        {
            string lst4SelectedItem = ChunkItemList.getSelectedItemContent();
            Selection.activeGameObject = GameObject.Find(lst4SelectedItem);
            LevelObj obj = chunk.objects.Find(x =>
            {
                return x.texture + "." + x.id == lst4SelectedItem;
            });
            selectedLevelObj = obj;
            if (obj != null)
                objSelectorGrid.position = obj.position;

        }
    }
    public static Rect ChangeType(ObjRect objRect)
    {
        return new Rect(objRect.left, objRect.top, objRect.width, objRect.height);
    }
    void Object_MouseSelector()
    {
        if (Event.current.control)
        {

            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x,
                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y);
            if (grid.rect.Contains(filteredMousePosition))
            {

                grabbedObj = new Rect(filteredMousePosition.x,
                   filteredMousePosition.y,
                   grabbedObj.width, grabbedObj.height);
                lastGrabbedObjScale = new Vector2(grabbedObj.width, grabbedObj.height);
                //GUI.DrawTexture (grabbedObj, Resources.Load<Texture> ("Objects/" + lst5.getSelectedItemContent ()));
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    if (chunk.objects.Count > 0)
                    {
                        Vector2 grabbedPosition = new Vector2(grid.FilterPosition(filteredMousePosition + new Vector2((int)hScroll, 0)).x,
                                              grid.FilterPosition(filteredMousePosition + new Vector2(0, (int)vScroll)).y);
                        foreach (LevelObj item in chunk.objects)
                        {
                            //if (item.position.left == grabbedPosition.x && item.position.top == grabbedPosition.y) {
                            if (ChangeType(item.position).Contains(grabbedPosition))
                            {
                                ChunkItemList.SelectItem(item.texture + "." + item.id);
                                SelectItemOnScene("");
                            }
                        }
                    }
                }
                else if (Event.current.button == 0 && Event.current.type == EventType.mouseDrag)
                {
                    if (chunk.objects.Count > 0)
                    {
                        Vector2 grabbedPosition = new Vector2(grid.FilterPosition(filteredMousePosition + new Vector2((int)hScroll, 0)).x,
                                              grid.FilterPosition(filteredMousePosition + new Vector2(0, (int)vScroll)).y);
                        selectedLevelObj.position = new ObjRect((int)grabbedPosition.x, (int)grabbedPosition.y, selectedLevelObj.position.width, selectedLevelObj.position.height, chunk.layerNumber);

                        SelectItemOnScene("");
                    }
                }
            }

        }
    }
    void AddItem()
    {

        if (Event.current.control)
        {
            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x,
                                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y);
            if (grid.rect.Contains(filteredMousePosition))
            {
                grabbedObj = new Rect(filteredMousePosition.x,
                                       filteredMousePosition.y,
                                       grabbedObj.width, grabbedObj.height);

                lastGrabbedObjScale = new Vector2(grabbedObj.width, grabbedObj.height);
                //GUI.DrawTexture (grabbedObj, Resources.Load<Texture>("Objects/" + lst5.getSelectedItemContent()));


                if (Event.current.button == 1 && Event.current.isMouse)
                {
                    if (chunk.objects.Count > 0)
                    {

                        if (!ChangeType(chunk.objects[chunk.objects.Count - 1].position).Contains(new Vector2(grid.FilterPosition(filteredMousePosition + new Vector2((int)hScroll, 0)).x,
                                                                                         grid.FilterPosition(filteredMousePosition + new Vector2(0, (int)vScroll)).y)))
                        {
                            Debug.Log(TextureList.getSelectedItemContent());
                            chunk.objects.Add(new LevelObj(new Rect(grid.FilterPosition(filteredMousePosition + new Vector2((int)(hScroll ), 0)).x,
                                                                  grid.FilterPosition(filteredMousePosition + new Vector2(0, (int)(vScroll ))).y,
                                                                  grabbedObj.width,
                                                                  grabbedObj.height)
                                                         , Resources.Load<Texture>("Objects/" + TextureList.getSelectedItemContent()), chunk.idAccumulator, chunk.layerNumber));
                            ChunkItemList.Additem(new string[] { chunk.objects[chunk.objects.Count - 1].texture + "." + chunk.idAccumulator.ToString() });
                            chunk.idAccumulator++;
                        }
                    }
                    else
                    {
                        Debug.Log(TextureList.getSelectedItemContent());
                        chunk.objects.Add(new LevelObj(new Rect(grid.FilterPosition(filteredMousePosition + new Vector2((int)(hScroll ), 0)).x ,
                                                              grid.FilterPosition(filteredMousePosition + new Vector2(0, (int)(vScroll ))).y,
                                                              grabbedObj.width,
                                                              grabbedObj.height)
                                                       , Resources.Load<Texture>("Objects/" + TextureList.getSelectedItemContent()), chunk.idAccumulator, chunk.layerNumber));
                        TextureList.Additem(new string[] { chunk.objects[0].texture + "." + chunk.idAccumulator.ToString() });
                        chunk.idAccumulator++;
                    }
                }


            }
        }
    }
    void DeleteItem()
    {
        if (Event.current.alt)
        {
            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x + (int)hScroll,
                                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y + (int)vScroll);
            if (Event.current.button == 1 && Event.current.isMouse)
            {
                foreach (var item in chunk.objects)
                {
                    if (ChangeType(item.position).Contains(filteredMousePosition))
                    {
                        TextureList.DeleteItem(new string[] { item.texture + item.id.ToString() });
                        chunk.objects.Remove(item);
                        deletedItems.Add(item.texture + "." + item.id.ToString());

                        window.Repaint();
                        return;
                    }
                }
            }
        }


        if (Event.current.keyCode == KeyCode.Backspace && Event.current.type == EventType.keyUp)
        {
            if (dynamicSelection.objects.Count == 0)
                return;
            foreach (var item in dynamicSelection.objects)
            {
                TextureList.DeleteItem(new string[] { item.texture + item.id.ToString() });
                chunk.objects.Remove(item);
                deletedItems.Add(item.texture + "." + item.id.ToString());

            }
            clearSelection();
            window.Repaint();
        }
    }
    bool IsObjectChanged(GameObject inGameObject)
    {
        if (inGameObject.GetComponents<UnityEngine.Component>().Length > 2)
            return true;
        return false;
    }
    void SaveChunk()
    {
        clearSelection();
        Debug.Log(chunk.Name);
        foreach (var item in chunk.objects)
        {
            item.position = new ObjRect(item.position.left - chunk.centerOfChunk.left,
                                        item.position.top - chunk.centerOfChunk.top,
                                        item.position.width,
                                        item.position.height);
        }
        chunk.isSaved = true;
        CubeFile.SerializeObject<Chunk>(chunk.Name, chunk,"Chunks/");
        RefreshEditor();
    }
    void WipeDeletedGameObjects()
    {
        if (deletedItems.Count > 0)
        {
            int counter = deletedItems.Count;
            while (counter > 0)
            {
                GameObject.DestroyImmediate(GameObject.Find(deletedItems[counter - 1]));
                deletedItems.Remove(deletedItems[counter - 1]);
                counter--;
            }
            EditorApplication.SaveScene();
        }
    }
    void LoadChunk(string chunkName)
    {
        chunk = CubeFile.DeSerializeObject<Chunk>(chunkName,"Chunks/");
       
            foreach (var item in chunk.objects)
            {
                item.position = new ObjRect(item.position.left + chunk.centerOfChunk.left,
                                            item.position.top + chunk.centerOfChunk.top,
                                            item.position.width,
                                            item.position.height);
            }

            RefreshEditor();
            CommitChanges();   
       
    }
    void CommitChanges()
    {
        noChangesApplied = true;
        grid = new Grid(800, 400, new Rect(215, 90, 500, 400), 40, 40);
     
    }
    void RefreshEditor()
    {
        if (chunk.objects == null)
            return;
        string[] chunkItems = new string[chunk.objects.Count];
        for (int i = 0; i < chunkItems.Length; i++)
        {
            chunkItems[i] = chunk.objects[i].texture + "." + chunk.objects[i].id.ToString();
        }
        ChunkItemList = new Listbox(chunkItems, "Items", new Rect(10, 480, 200, 140), 20);

        string[] savedChunks = CubeFile.CheckDirectory("SavedFiles/Chunks");
        SavedChunksList = new Listbox(savedChunks, "SavedChunks", new Rect(10, 295, 200, 140), 20);
        foreach (var item in SavedChunksList.items)
        {
            item.action = window.LoadChunk;
            Debug.Log(item.content[0].text + " " + chunk.Name);
            if (item.content[0].text .Contains( chunk.Name))
                SavedChunksList.SelectItem(item.id);
        }
        //chunk = new Chunk("NewChunk", new Rect(0, 0, 1, 1), 0, 0, new List<LevelObj>(), 0);
        
    }
    string[] LoadAvailableObjects()
    {
        Texture[] tList = Resources.LoadAll<Texture>("Objects/");

        string[] objectNames = new string[tList.Length];
        for (int i = 0; i < tList.Length; i++)
        {
            objectNames[i] = tList[i].name;
        }
        return objectNames;
    }
    void DeleteChunk()
    {
        CubeFile.DeleteFile(chunk.Name,"/Chunks/");
        RefreshEditor();
    }
    void ResizeSelectedTexture()
    {

        GUI.Label(new Rect(360, 63, 35, 20), "Obj H:");
        grabbedObj.height = int.Parse(GUI.TextField(new Rect(400, 63, 70, 20), grabbedObj.height.ToString()));
        GUI.Label(new Rect(470, 63, 35, 20), "Obj W:");
        grabbedObj.width = int.Parse(GUI.TextField(new Rect(510, 63, 70, 20), grabbedObj.width.ToString()));

    }
    void PickScale()
    {
        grid.patchWidth = (int)lastGrabbedObjScale.x;
        grid.patchHeight = (int)lastGrabbedObjScale.y;
    }
    void PickTexture()
    {
        ChunkItemList.SelectItem(selectedLevelObj.texture);
        changeSelectedBlockTexture("");
    }
    void OnGUI()
    {
        DeleteItem();
        ResizeSelectedTexture();
        if (GUI.Button(new Rect(160, 10, 50, 20), "Save"))
            SaveChunk();
        if (GUI.Button(new Rect(160, 35, 90, 20), "Moving COC"))
        {
            movingCenterOfChunk = true;
            autoAdjustCOC = false;
        }
        if (GUI.Button(new Rect(120, 65, 50, 20), "COC"))
        {
            movingCenterOfChunk = false;
            autoAdjustCOC = true;
        }
        if (GUI.Button(new Rect(215, 10, 50, 20), "Delete"))
        {
            DeleteChunk();
        }
        AddItem();
        if (dynamicSelection.objects.Count == 0)
            Object_MouseSelector();

        string temp = GUI.TextField(new Rect(10, 10, 140, 20), chunk.Name);
        if (chunk.Name != temp ||
            Canvas.width != grid.contentWidth ||
            Canvas.height != grid.contentHeight)
        {
            chunk.Name = temp;
            noChangesApplied = false;
        }
        
        GUI.Label(new Rect(250, 40, 100, 20), "Patch Height:");
        grid.patchHeight = int.Parse(GUI.TextField(new Rect(325, 40, 80, 20),
        grid.patchHeight.ToString()));
        GUI.Label(new Rect(430, 40, 100, 20), "Patch Width:");
        grid.patchWidth = int.Parse(GUI.TextField(new Rect(500, 40, 80, 20),
                                                  grid.patchWidth.ToString()));
        GUI.Label(new Rect(280, 10, 100, 20), "Chunk Height:");
        Canvas.height = int.Parse(GUI.TextField(new Rect(340, 10, 80, 20),
                                                 Canvas.height.ToString()));
        if (Canvas.height < 1)
            Canvas.height = 400;

        GUI.Label(new Rect(440, 10, 100, 20), "Chunk Width:");
        Canvas.width = int.Parse(GUI.TextField(new Rect(500, 10, 80, 20),
                                                  Canvas.width.ToString()));
        if (Canvas.width < 1)
            Canvas.width = 800;


        if (noChangesApplied)
            GUI.DrawTexture(new Rect(115, 40, 20, 20), Resources.Load<Texture>("Checked"));
        else
            GUI.DrawTexture(new Rect(115, 40, 20, 20), Resources.Load<Texture>("NotChecked"));

        if (GUI.Button(new Rect(10, 40, 100, 20), "Commit"))
        {
            CommitChanges();

        }

        if (GUI.Button(new Rect(10, 63, 100, 20), "Clear"))
        {
            foreach (var item in chunk.objects)
            {
                deletedItems.Add(item.texture + "." + item.id.ToString());
            }
            chunk.objects.RemoveAll(x => { return true; });

            RefreshEditor();
        }
        if (GUI.Button(new Rect(270, 63, 80, 20), "Pick Scale"))
        {
            PickScale();
        }

        if (GUI.Button(new Rect(180, 63, 85, 20), "Pick Texture"))
        {
            PickTexture();
        }

        ChunkItemList.EventHandler();
        SavedChunksList.EventHandler();
        TextureList.EventHandler();

        hScroll = GUI.HorizontalScrollbar(new Rect(215, 490, 500, 10), hScroll, grid.patchWidth, 0, Canvas.width - grid.rect.width);
        vScroll = GUI.VerticalScrollbar(new Rect(715, 90, 10, 400), vScroll, grid.patchHeight, 0, Canvas.height - grid.rect.height);

        vScroll = vScroll < 0 ? 0 : vScroll;
        hScroll = hScroll < 0 ? 0 : hScroll;

        grid.xMin = -(int)hScroll;
        grid.yMin = -(int)vScroll;
        
        foreach (var item in chunk.objects)
        {
            if (dynamicSelection.objects.Contains(item))
                continue;
           
            Rect tmpRect = new Rect(item.position.left - (int)(hScroll )  ,
                                    item.position.top - (int)(vScroll ),
                     item.position.width,
                     item.position.height);
            if (!(tmpRect.xMin < grid.rect.xMin ||
                 tmpRect.yMin < grid.rect.yMin ||
                 tmpRect.xMax > grid.rect.xMax ||
                 tmpRect.yMax > grid.rect.yMax))
                GUI.DrawTexture(tmpRect, Resources.Load<Texture>("Objects/" + item.texture));
        }
        Rect tmpR = new Rect(); 
        //if(movingCenterOfChunk)
            tmpR = new Rect(chunk.centerOfChunk.left - 25 - (int)hScroll ,
                     chunk.centerOfChunk.top - 25 - (int)vScroll ,
                     50,
                     50);
        if(autoAdjustCOC)
            tmpR = new Rect(chunk.CenterOfChunk.left - 25 - (int)hScroll,
                     chunk.CenterOfChunk.top - 25 - (int)vScroll,
                     50,
                     50);
        if (!(tmpR.xMin < grid.rect.xMin ||
             tmpR.yMin < grid.rect.yMin ||
             tmpR.xMax > grid.rect.xMax ||
             tmpR.yMax > grid.rect.yMax))

        GUI.DrawTexture(tmpR, Resources.Load<Texture>("plus"));

        foreach (var item in dynamicSelection.objects)
        {
            //if (item.position.depth == 40 - (GetActiveLayer() * 5))
            //{
            Rect tmpRect = new Rect(item.position.left - (int)hScroll + (dynamicSelection.boundry.xMin - dynamicSelection.firstPos.x),
                     item.position.top - (int)vScroll + (dynamicSelection.boundry.yMin - dynamicSelection.firstPos.y),
                     item.position.width,
                     item.position.height);
            if (!(tmpRect.xMin < grid.rect.xMin ||
                 tmpRect.yMin < grid.rect.yMin ||
                 tmpRect.xMax > grid.rect.xMax ||
                 tmpRect.yMax > grid.rect.yMax))
                GUI.DrawTexture(tmpRect, Resources.Load<Texture>("Objects/" + item.texture));
        }

        Rect tmpR2 = new Rect(objSelectorGrid.position.left - (int)hScroll,
                             objSelectorGrid.position.top - (int)vScroll,
                             objSelectorGrid.position.width,
                             objSelectorGrid.position.height);
        
        if (!(tmpR2.xMin < grid.rect.xMin ||
             tmpR2.yMin < grid.rect.yMin ||
             tmpR2.xMax > grid.rect.xMax ||
             tmpR2.yMax > grid.rect.yMax))

            GUI.DrawTexture(tmpR2, Resources.Load<Texture>(objSelectorGrid.texture));

        GUI.DrawTexture(new Rect(225, 430, 50, 50), Resources.Load<Texture>("Coordinate"));
        if (selectedBlock != null)
            GUI.DrawTexture(new Rect(585, 10, 60, 60), selectedBlock);
        MovingCOC();
        Debug.Log(movingCenterOfChunk);
        grid.Draw();
        AssociativeSeletion();
        window.Repaint();
    }

}