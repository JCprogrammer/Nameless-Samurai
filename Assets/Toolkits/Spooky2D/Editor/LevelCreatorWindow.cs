using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Runtime.Serialization;
using System.Reflection;

public delegate void ListClickAction(string item);
public class DynamicSelection
{
    public Rect drawRect;
    public List<LevelObj> objects;
    public Rect boundry;
    public Vector2 firstPos;
    public DynamicSelection()
    {
        firstPos = Vector2.zero;
        drawRect = new Rect();
        objects = new List<LevelObj>();
        boundry = new Rect();
       
    }
    public bool isClonning;
}

public class ComponentGroup
{
    public string name;
    public List<Component> components;
    public ComponentGroup(string name, Component[] components)
    {
        this.name = name;
        this.components = new List<Component>();
        this.components.AddRange(components);
    }
}

public class LevelCreatorWindow : EditorWindow
{
    bool movingCameraPivot = false;
    public static Listbox lst1;
    public static Listbox lst2;
    public static Listbox lst3;
    public static Listbox lst4;
    public static Listbox lst5;
    public static Listbox lst6;
    static Level level;
    static LevelObj objSelectorGrid;
    static LevelObj selectedLevelObj;
    static float hScroll;
    static float vScroll;
    static LevelCreatorWindow window;
    static Grid grid;
    static bool noChangesApplied;
    static DynamicSelection dynamicSelection = new DynamicSelection();
    Rect grabbedObj;
    List<string> deletedItems;
    Vector2 lastGrabbedObjScale;
    static Texture selectedBlock;
    bool[] layers;
    string[] layersName;
    int activeLayer;
    const int durationCap = 10;
    int counter;
    Chunk chunk;
    string TimeSpan = "0";
    bool MovingSeeker = false;
    bool ctrlSelected = false;
    Rect seekerBoundry = new Rect(180, 85, 20, 22);
    float gridScale = 1;
    Rect gridScalerRect;
    bool gridScaleChanged = false;
    public static List<ComponentGroup> serializedObjects;


    
    [MenuItem("Spooky Guys/Level Editor")]
    static void Init()
    {
        
        objSelectorGrid = new LevelObj(new Rect(0, 0, 0, 0), Resources.Load<Texture>("HexaGridRed"), 0, 0);
        serializedObjects = new List<ComponentGroup>();
        level = new Level("NewLevel", 800, 400);
        grid = new Grid(level.width, level.height, new Rect(180, 90, 800, 400), 40, 40);
        window = (LevelCreatorWindow)EditorWindow.GetWindow(typeof(LevelCreatorWindow));
        window.minSize = new Vector2(1000, 700);
        window.gridScalerRect = new Rect(160, 90, 20, 200);
        noChangesApplied = true;
        hScroll = 0;
        vScroll = 0;
        level.cameraPivot = new ObjRect((int)grid.rect.xMin, (int)grid.rect.yMax - 40, 40, 40);
        window.activeLayer = 7;
        lst2 = new Listbox(new string[] { "None" }, "AvailableLevelBehaviorList", new Rect(10, 390, 150, 280), 20);
        lst3 = new Listbox(new string[] { "None" }, "LevelBehaviorHirarchy", new Rect(180, 530, 180, 140), 20);
        lst4 = new Listbox(new string[] { "None" }, "LevelChunksHirarchy", new Rect(370, 530, 180, 140), 20);

        string[] savedChunks = CubeFile.CheckDirectory("SavedFiles/Chunks/");
        lst1 = new Listbox(savedChunks, "AvailableChunkList", new Rect(10, 110, 150, 220), 20);
        string[] savedLevels = CubeFile.CheckDirectory("SavedFiles/");
        lst6 = new Listbox(savedLevels, "SavedLevels", new Rect(560, 530, 180, 140), 20);
        foreach (var item in lst1.items)
        {
            item.action = window.LoadChunk;
        }
        foreach (var item in lst6.items)
        {
            item.action = window.LoadLevel;
        }
        foreach (var item in lst4.items)
        {
            item.action = window.SelectItemOnScene;
        }
        window.lastGrabbedObjScale = new Vector2(40, 40);
        window.deletedItems = new List<string>(0);
        window.grabbedObj = new Rect(0, 0, window.lastGrabbedObjScale.x, window.lastGrabbedObjScale.y);
        string[] availObjs = window.LoadAvailableObjects();
        lst5 = new Listbox(availObjs, "AvailableObjects", new Rect(755, 530, 200, 140), 20);
        foreach (var item in lst5.items)
        {
            item.action = window.changeSelectedBlockTexture;
        }
        window.layers = new bool[10];
        window.layers[7] = true;
        window.layersName = new string[]{"L1",
										 "L2",
										 "L3",
										 "L4",
										 "L5",
										 "L6",
										 "L7",
										 "L8",
										 "L9",
										 "L10"};
        window.SelectItemOnScene("");
        //double tempIntBuffer = Camera.main.GetComponent<SoundController>().startFrom * (5.655f * 40) - hScroll;
        //window.seekerBoundry = new Rect(180 + (float)tempIntBuffer, 85, 20, 22);
        window.wantsMouseMove = true;
        window.changeSelectedBlockTexture("");

    }
    void MovingCamera()
    {
        if (movingCameraPivot)
        {
            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x,
                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y);
            if (grid.rect.Contains(filteredMousePosition))
            {
                Vector2 grabbedPosition = new Vector2(grid.FilterPosition(filteredMousePosition + new Vector2(hScroll, 0)).x,
                                              grid.FilterPosition(filteredMousePosition + new Vector2(0, vScroll)).y);

                level.cameraPivot = new ObjRect((int)grabbedPosition.x,(int)grabbedPosition.y,40,40);
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    movingCameraPivot = false;
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

                    item.position = new ObjRect(item.position.left + (dynamicSelection.boundry.xMin - dynamicSelection.firstPos.x)/gridScale,
                                                item.position.top +  (dynamicSelection.boundry.yMin - dynamicSelection.firstPos.y)/gridScale,
                                                item.position.width,
                                                item.position.height,
                                                item.position.depth);
                
            }
        }
        dynamicSelection.drawRect = new Rect(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x + hScroll*gridScale,
                                             HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y + vScroll * gridScale, 0, 0);
        if (dynamicSelection.isClonning)
            level.objects.AddRange(dynamicSelection.objects);
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
                                                     HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x + hScroll*gridScale - dynamicSelection.drawRect.xMin,
                                                     HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y + vScroll*gridScale - dynamicSelection.drawRect.yMin);


            }
            else if (Event.current.type == EventType.mouseUp)
            {

                float max = Mathf.Max(dynamicSelection.drawRect.xMin, dynamicSelection.drawRect.xMax);
                dynamicSelection.drawRect.xMin = Mathf.Min(dynamicSelection.drawRect.xMax, dynamicSelection.drawRect.xMin);
                dynamicSelection.drawRect.xMax = max;
                max = Mathf.Max(dynamicSelection.drawRect.yMin, dynamicSelection.drawRect.yMax);
                dynamicSelection.drawRect.yMin = Mathf.Min(dynamicSelection.drawRect.yMax, dynamicSelection.drawRect.yMin);
                dynamicSelection.drawRect.yMax = max;
                foreach (LevelObj item in level.objects)
                {
                    if (dynamicSelection.drawRect.Contains(new Vector2((item.position.left - grid.rect.left) * gridScale + grid.rect.left,
                                                                       (item.position.top   - grid.rect.top) * gridScale + grid.rect.top )) 
                                                            && ( GetActiveLayer().Contains((40 -(int)item.position.depth)/5)))
                    {
                        dynamicSelection.objects.Add(item);

                        if (dynamicSelection.objects.Count == 1)
                        {
                            dynamicSelection.boundry = ChangeType(item.position);
                            dynamicSelection.boundry = new Rect((dynamicSelection.boundry.left - grid.rect.left) * gridScale + grid.rect.left,
                                                                 (dynamicSelection.boundry.top - grid.rect.top) * gridScale + grid.rect.top ,
                                                                 dynamicSelection.boundry.width * gridScale,
                                                                 dynamicSelection.boundry.height * gridScale);
                        }
                        else if (dynamicSelection.objects.Count > 1)
                        {
                            dynamicSelection.boundry.xMin = Mathf.Min(dynamicSelection.boundry.xMin, ((ChangeType(item.position).xMin - grid.rect.left)*gridScale)+ grid.rect.left);
                            dynamicSelection.boundry.xMax = Mathf.Max(dynamicSelection.boundry.xMax, ((ChangeType(item.position).xMax - grid.rect.left)*gridScale)+ grid.rect.left);
                            dynamicSelection.boundry.yMin = Mathf.Min(dynamicSelection.boundry.yMin, ((ChangeType(item.position).yMin - grid.rect.top) * gridScale) + grid.rect.top);
                            dynamicSelection.boundry.yMax = Mathf.Max(dynamicSelection.boundry.yMax, ((ChangeType(item.position).yMax - grid.rect.top) * gridScale) + grid.rect.top);
                        }
                        dynamicSelection.firstPos = new Vector2(dynamicSelection.boundry.xMin , dynamicSelection.boundry.yMin);
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


            Vector2 tmpV = grid.FilterPosition(new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x + hScroll*gridScale,
                                                            HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y + vScroll*gridScale ));
            dynamicSelection.boundry = new Rect(tmpV.x,
                                                tmpV.y,
                                                dynamicSelection.drawRect.width,
                                                dynamicSelection.drawRect.height);

            dynamicSelection.drawRect = dynamicSelection.boundry;
            //foreach (var item in dynamicSelection.objects)
            //{
            //    item.position.depth = 40 - ((activeLayer + 1) * 5);
            //}

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
                        level.idAccumulator, 40 - ((activeLayer + 1) * 5),
                        item.texture + "." + item.id.ToString()));
                    if (GameObject.Find(item.texture + "." + item.id.ToString()).GetComponents<Component>().Length > 0)
                    {
                        serializedObjects.Add(new ComponentGroup(item.texture + "." + item.id.ToString(),
                                                 GameObject.Find(item.texture + "." + item.id.ToString()).GetComponents<Component>()));
                    }
                    level.idAccumulator++;
                }
                level.objects.AddRange(tmpObjList);
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
            foreach (LevelObj item in level.objects)
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
        Rect tmpR = new Rect(dynamicSelection.drawRect.xMin - hScroll * gridScale,
                                    dynamicSelection.drawRect.yMin - vScroll * gridScale,
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
    public List<int> GetActiveLayer()
    {
        List<int> buffer = new List<int>();
        for (int i = 0; i < layers.Length; i++)
            if (layers[i])
            {
                buffer.Add(i + 1);
            }
        return buffer;
    }
    public void changeSelectedBlockTexture(string parameter)
    {
        selectedBlock = Resources.Load<Texture>("Objects/" + lst5.getSelectedItemContent());
    }
    public void Update()
    {
        counter = System.DateTime.Now.Second;
        if (counter == durationCap)
        {
            //Debug.Log("Level Saved. Time: " + System.DateTime.Now.ToString());
            counter = 0;
        }

    }
    public void SelectItemOnScene(string item)
    {
        if (lst4.items.Count > 0)
        {
            string lst4SelectedItem = lst4.getSelectedItemContent();
            Selection.activeGameObject = GameObject.Find(lst4SelectedItem);
            LevelObj obj = level.objects.Find(x =>
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

            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x - grid.rect.left,
                                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y - grid.rect.top);
            filteredMousePosition /= gridScale;
            if (grid.rect.Contains(filteredMousePosition * gridScale + new Vector2(grid.rect.left, grid.rect.top)))
            {

                grabbedObj = new Rect(filteredMousePosition.x,
                   filteredMousePosition.y,
                   grabbedObj.width, grabbedObj.height);
                lastGrabbedObjScale = new Vector2(grabbedObj.width, grabbedObj.height);
                //GUI.DrawTexture (grabbedObj, Resources.Load<Texture> ("Objects/" + lst5.getSelectedItemContent ()));
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    if (level.objects.Count > 0)
                    {
                        Vector2 grabbedPosition = new Vector2((grid.FilterPosition(filteredMousePosition + new Vector2(hScroll + grid.rect.left, 0), gridScale).x),
                                                              (grid.FilterPosition(filteredMousePosition + new Vector2(0, vScroll + grid.rect.top), gridScale).y));
                        foreach (LevelObj item in level.objects)
                        {
                            if (item.position.depth == 40 - ((activeLayer + 1) * 5))
                            {
                                if (ChangeType(item.position).Contains(grabbedPosition))
                                {
                                    lst4.SelectItem(item.texture + "." + item.id);
                                    SelectItemOnScene("");
                                }
                            }
                        }
                    }
                }
                else if (Event.current.button == 0 && Event.current.type == EventType.mouseDrag)
                {

                    if (level.objects.Count > 0)
                    {
                        Vector2 grabbedPosition = new Vector2((grid.FilterPosition(filteredMousePosition + new Vector2(hScroll + grid.rect.left, 0), gridScale).x),
                                                           (grid.FilterPosition(filteredMousePosition + new Vector2(0, vScroll + grid.rect.top), gridScale).y));

                        selectedLevelObj.position = new ObjRect(grabbedPosition.x, grabbedPosition.y, selectedLevelObj.position.width, selectedLevelObj.position.height, 40 - ((activeLayer + 1) * 5));
                        SelectItemOnScene("");
                    }
                    
                }
         
            }

        }
        else
        {
            if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
            {
                objSelectorGrid.position = new ObjRect(0, 0, 0, 0);
            }
        }
    }
    void AddItem()
    {

        if (Event.current.control)
        {
            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x - grid.rect.left,
                                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y - grid.rect.top);
            filteredMousePosition /= gridScale;
            if (grid.rect.Contains(filteredMousePosition*gridScale + new Vector2(grid.rect.left,grid.rect.top)))
            {
                grabbedObj = new Rect(filteredMousePosition.x,
                                       filteredMousePosition.y,
                                       grabbedObj.width, grabbedObj.height);

                lastGrabbedObjScale = new Vector2(grabbedObj.width, grabbedObj.height);
                //GUI.DrawTexture (grabbedObj, Resources.Load<Texture>("Objects/" + lst5.getSelectedItemContent()));


                if (Event.current.button == 1 && Event.current.isMouse)
                {
                    if (level.objects.Count > 0)
                    {
                        bool allowedToAdd = true;
                        foreach (var item in level.objects)
                        {
                            if (item.position.depth == 40 - ((activeLayer + 1) * 5))
                            {
                                if (ChangeType(item.position).Contains(new Vector2(grid.FilterPosition(filteredMousePosition + new Vector2(hScroll + grid.rect.left, 0), gridScale).x,
                                                                                   grid.FilterPosition(filteredMousePosition + new Vector2(0, vScroll + grid.rect.top), gridScale).y)))
                                {
                                    allowedToAdd = false;
                                    break;
                                }
                            }
                        }
                        if(allowedToAdd)
                            {
                                Debug.Log(lst5.getSelectedItemContent());

                                level.objects.Add(new LevelObj(new Rect((grid.FilterPosition(filteredMousePosition + new Vector2(hScroll + grid.rect.left, 0),gridScale).x),
                                                                      (grid.FilterPosition(filteredMousePosition + new Vector2(0, vScroll + grid.rect.top),gridScale).y),
                                                                      grabbedObj.width,
                                                                      grabbedObj.height)
                                                             , Resources.Load<Texture>("Objects/" + lst5.getSelectedItemContent()), level.idAccumulator, 40 - ((activeLayer + 1) * 5)));
                                //lst4.Additem(new string[] {level.objects[level.objects.Count-1].texture +"." + level.objects[level.objects.Count-1].id.ToString()});
                                lst4.Additem(new string[] { level.objects[level.objects.Count - 1].texture + "." + level.idAccumulator.ToString() });
                                level.idAccumulator++;
                               
                            }    
                        
                        
                    }
                    else
                    {
                        Debug.Log(lst5.getSelectedItemContent());
                        level.objects.Add(new LevelObj(new Rect((grid.FilterPosition(filteredMousePosition+ new Vector2(hScroll + grid.rect.left, 0),gridScale).x),
                                                              (grid.FilterPosition(filteredMousePosition + new Vector2(0, vScroll + grid.rect.top),gridScale).y),
                                                              grabbedObj.width,
                                                              grabbedObj.height)
                                                       , Resources.Load<Texture>("Objects/" + lst5.getSelectedItemContent()), level.idAccumulator, 40 - ((activeLayer + 1) * 5)));
                        //lst4.Additem(new string[] {level.objects[0].texture +"." + level.objects[0].id.ToString()});
                        lst4.Additem(new string[] { level.objects[0].texture + "." + level.idAccumulator.ToString() });
                        level.idAccumulator++;
                    }
                }


            }
        }
    }
    void LoadChunk(string chunkName)
    {
        chunk = CubeFile.DeSerializeObject<Chunk>(chunkName, "Chunks/");
    }
    void MoveChunk()
    {
        if (chunk != null)
        {

            if (Event.current.control && Event.current.shift)
            {

                if (dynamicSelection.objects.Count == 0)
                {
                    Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x,
                                                 HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y);
                    if (grid.rect.Contains(filteredMousePosition))
                    {
                        chunk.centerOfChunk = new ObjRect((grid.FilterPosition(filteredMousePosition - new Vector2(vScroll, hScroll)).x / gridScale),
                                                            (grid.FilterPosition(filteredMousePosition).y/gridScale),
                                                            10, 10);
                        foreach (var chunkObject in chunk.objects)
                        {
                            Rect tmpRect = new Rect((chunkObject.position.left  + chunk.centerOfChunk.left) * gridScale,
                                                    (chunkObject.position.top + chunk.centerOfChunk.top) * gridScale,
                                chunkObject.position.width * gridScale,
                                chunkObject.position.height * gridScale);

                            if (!(tmpRect.xMin < grid.rect.xMin ||
                                 tmpRect.yMin < grid.rect.yMin ||
                                 tmpRect.xMax > grid.rect.xMax ||
                                 tmpRect.yMax > grid.rect.yMax))
                                GUI.DrawTexture(tmpRect, Resources.Load<Texture>("Objects/" + chunkObject.texture));

                        }
                        GUI.DrawTexture(new Rect(chunk.centerOfChunk.left - 5, chunk.centerOfChunk.top - 5, 10, 10), Resources.Load<Texture>("plus"));

                    }

                }

            }
        }

    }
    void GenerateChunk()
    {
        if (Event.current.control && Event.current.shift)
        {
            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x - grid.rect.left,
                                       HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y - grid.rect.top);
            filteredMousePosition /= gridScale;
            if (grid.rect.Contains(filteredMousePosition * gridScale + new Vector2(grid.rect.left, grid.rect.top)))
            {
               
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {  
                    List<LevelObj> objBuffer = new List<LevelObj>();
                    foreach (var item in chunk.objects)
	                {
                        objBuffer.Add(new LevelObj(new Rect(((item.position.left)),
                                                            ((item.position.top) ),
                                                            item.position.width, item.position.height),
                            Resources.Load<Texture>("Objects/" + item.texture),
                            item.id, item.position.depth + 40 - ((activeLayer + 1) * 5)));        
	                }
                    level.objects.Add(new Chunk("[chunk]" + chunk.Name, new Rect(), level.idAccumulator, 40 - ((activeLayer + 1) * 5), objBuffer));
                    
                    
                    lst4.Additem(new string[] { ((Chunk)level.objects[level.objects.Count - 1]).Name + "." + level.idAccumulator.ToString() });
                    level.idAccumulator++;
                    Debug.Log(((Chunk)level.objects[level.objects.Count - 1]).Name);
                    Debug.Log(((Chunk)level.objects[level.objects.Count - 1]).objects.Count);
                    ((Chunk)level.objects[level.objects.Count - 1]).centerOfChunk = new ObjRect((grid.FilterPosition(filteredMousePosition + new Vector2(hScroll + grid.rect.left, 0),gridScale).x),
                                                                                                 (grid.FilterPosition(filteredMousePosition + new Vector2(0, vScroll + grid.rect.top),gridScale).y), 10, 10);
                }

            }
        }
    }
    void DeleteItem()
    {
        if (Event.current.alt)
        {
            Vector2 filteredMousePosition = new Vector2(HandleUtility.WorldToGUIPoint(Event.current.mousePosition).x - grid.rect.left,
                                                         HandleUtility.WorldToGUIPoint(Event.current.mousePosition).y - grid.rect.top);
            filteredMousePosition /= gridScale;
            
            if (Event.current.button == 1 && Event.current.isMouse)
            {

                foreach (var item in level.objects)
                {
                    if (item.position.depth == 40 - ((activeLayer + 1) * 5))
                    {
                        if (item is Chunk)
                        {
                            if ((new Rect(((Chunk)item).centerOfChunk.left - 5, ((Chunk)item).centerOfChunk.top - 5, 10, 10)).Contains(filteredMousePosition + new Vector2(hScroll + grid.rect.left, vScroll + grid.rect.top)) && (item.position.depth == 40 - (activeLayer + 1) * 5))
                            {
                                lst4.DeleteItem(new string[] { "." + item.id.ToString() });
                                level.objects.Remove(item);
                                deletedItems.Add(((Chunk)item).Name + "." + item.id.ToString());
                                Debug.Log(((Chunk)item).Name);
                                window.Repaint();
                                return;
                            }
                        }

                        if (ChangeType(item.position).Contains(filteredMousePosition + new Vector2(hScroll + grid.rect.left, vScroll + grid.rect.top)) && (item.position.depth == 40 - (activeLayer + 1) * 5))
                        {
                            lst4.DeleteItem(new string[] { item.texture + "." + item.id.ToString() });
                            level.objects.Remove(item);
                            deletedItems.Add(item.texture + "." + item.id.ToString());

                            window.Repaint();
                            return;
                        }
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
                lst4.DeleteItem(new string[] { item.texture + item.id.ToString() });
                level.objects.Remove(item);
                deletedItems.Add(item.texture + "." + item.id.ToString());

            }
            clearSelection();
            window.Repaint();
        }
    }
    void BuildLevelScene()
    {
        if (!EditorApplication.OpenScene("Assets/Levels/" + level.name + ".unity"))
            EditorApplication.NewScene();
        foreach (var item in level.objects)
        {
            CreateSceneObject(item);
        }
        CreateCamera();
        EditorApplication.SaveScene("Assets/Levels/" + level.name + ".unity", false);
        WipeDeletedGameObjects();
    }
    bool IsObjectChanged(GameObject inGameObject)
    {
        if (inGameObject.GetComponents<UnityEngine.Component>().Length > 2)
            return true;
        return false;
    }
    void CreateCamera()
    {

        GameObject cam = GameObject.Find("Main Camera");
        Camera camCom;
        if (cam == null)
        {
            cam = new GameObject("Main Camera");
            camCom = cam.AddComponent<Camera>();
        }
        camCom = cam.GetComponent<Camera>();
        camCom.isOrthoGraphic = true;
        camCom.orthographicSize = 15;
        if (lst4.items.Count > 0)
        {
            //Transform focousedObj = GameObject.Find(lst4.getSelectedItemContent()).transform;
            //if (focousedObj == null)
              //  cam.transform.position = Vector3.zero;
            //else
            //{
                //Debug.Log("focousedItemName : " + focousedObj.name);
            Vector3 focousedObj =  new Vector3(1 * (float)level.cameraPivot.left /
                                                         GlobalVariables.minifier, -1 * (float)level.cameraPivot.top /
                                                         GlobalVariables.minifier, 0) + new Vector3((1 * (float)level.cameraPivot.width /
                                                                                                      (GlobalVariables.minifier * 2)) + 1, (-1 * (float)level.cameraPivot.height /
                                                                                                   (GlobalVariables.minifier * 2)) + 2, level.cameraPivot.depth);
      
            cam.transform.position = new Vector3(focousedObj.x + (400.0f / GlobalVariables.minifier),
                    focousedObj.y + (200.0f / GlobalVariables.minifier), -10);
            
                   
      //}
        }
        else
        {
            cam.transform.position = Vector3.zero;
        }
        GameObject light = GameObject.Find("Light");
        if (light == null)
        {
            light = new GameObject("Light");
            Light lightCom = light.AddComponent<Light>();
            lightCom.type = LightType.Directional;
            lightCom.intensity = 0.6f;
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
                                                 GlobalVariables.minifier, 0) + new Vector3(20.0f/GlobalVariables.minifier,40.0f/GlobalVariables.minifier,item.position.depth);
            Debug.Log("Number of objects: " + ((Chunk)item).objects.Count + "Center of Chunk" + ((Chunk)item).centerOfChunk.top);

            ChunkData cData = ChunkContainer.AddComponent<ChunkData>();
            float xMin = ((Chunk)item).centerOfChunk.left;
            float xMax = ((Chunk)item).centerOfChunk.left;
            
            foreach (var item2 in ((Chunk)item).objects)
            {
                if (item2.position.left + ((Chunk)item).centerOfChunk.left < xMin)
                    xMin = item2.position.left + ((Chunk)item).centerOfChunk.left;
                if (item.position.left + item.position.width + ((Chunk)item).centerOfChunk.left > xMax)
                    xMax = item.position.left + item.position.width + ((Chunk)item).centerOfChunk.left;
                CreateSceneObject(item2,ChunkContainer.transform);

            }
            Debug.Log(xMax + " " + xMin);
            cData.chunkLength = (xMax - xMin) * 2 / GlobalVariables.minifier;
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
            if (item.isCloneOf != "none")
            {
                Debug.Log(item.isCloneOf);
                List<Component> comps = serializedObjects.Find(x => x.name == item.isCloneOf).components;

                foreach (Component item2 in comps)
                {

                    FieldInfo[] sourceObjectFields = item2.GetType().GetFields();
                    Component destObjComponent = sceneObjContainer.AddComponent(item2.GetType());
                    foreach (FieldInfo fieldInfo in sourceObjectFields)
                        destObjComponent.GetType().GetField(fieldInfo.Name).SetValue(destObjComponent, fieldInfo.GetValue(item2));
                }
                item.isCloneOf = "none";
            }
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
                                                                                                     (GlobalVariables.minifier * 2)) + 2, item.position.depth);
        if (parent != null)
        {
            sceneObjContainer.transform.position += parent.position - new Vector3(25 / GlobalVariables.minifier, 50 / GlobalVariables.minifier, 0);
            sceneObjContainer.transform.parent = parent;
        }
    }
    void SaveLevel()
    {
        clearSelection();
        CubeFile.SerializeObject<Level>(level.name, level);
        BuildLevelScene();
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
    void LoadLevel(string levelName)
    {
        EditorApplication.NewScene();
        level = CubeFile.DeSerializeObject<Level>(levelName);
        RefreshEditor();
        CommitChanges();
    }
    void CommitChanges()
    {
        noChangesApplied = true;
        grid = new Grid(level.width, level.height, new Rect(180, 90, 800, 400),
                        grid.patchWidth , grid.patchHeight );
        if (Camera.main.GetComponent<SoundController>() != null)
            Camera.main.GetComponent<SoundController>().startFrom = float.Parse(TimeSpan);

    }
    void RefreshEditor()
    {
        lst3 = new Listbox(new string[] { "None" }, "LevelBehaviorHirarchy", new Rect(180, 530, 180, 140), 20);
        if (level.objects == null)
            return;
        string[] chunkItems = new string[level.objects.Count];
        for (int i = 0; i < chunkItems.Length; i++)
        {
            if (level.objects[i] is Chunk)
            {
                chunkItems[i] = "[chunk]"+((Chunk)level.objects[i]).Name + "." + level.objects[i].id.ToString();
                Debug.Log("asdas");
            }
            chunkItems[i] = level.objects[i].texture + "." + level.objects[i].id.ToString();
        }
        lst4 = new Listbox(chunkItems, "LevelChunksHirarchy", new Rect(370, 530, 180, 140), 20);

        string[] savedLevels = CubeFile.CheckDirectory("SavedFiles/");
        lst6 = new Listbox(savedLevels, "SavedLevels", new Rect(560, 530, 180, 140), 20);
        foreach (var item in lst6.items)
        {
            item.action = window.LoadLevel;
            if (item.content[0].text == level.name)
                lst6.SelectItem(item.id);
        }

        foreach (var item in lst4.items)
        {
            item.action = window.SelectItemOnScene;
        }
        EditorApplication.OpenScene("Assets/Levels/" + level.name + ".unity");
        SelectItemOnScene("");
        //serializedObjects = new List<ComponentGroup>();
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
    void DeleteLevel()
    {
        CubeFile.DeleteFile(level.name);
        CubeFile.DeleteScene(level.name);
        RefreshEditor();
    }
    void ResizeSelectedTexture()
    {

        GUI.Label(new Rect(360, 63, 35, 20), "Obj H:");
        grabbedObj.height = int.Parse(GUI.TextField(new Rect(400, 63, 70, 20), grabbedObj.height.ToString()));
        GUI.Label(new Rect(470, 63, 35, 20), "Obj W:");
        grabbedObj.width = int.Parse(GUI.TextField(new Rect(510, 63, 70, 20), grabbedObj.width.ToString()));

    }
    void TimeBar()
    {

        Color tmp = GUI.color;
        GUI.color = Color.cyan;
        GUI.Box(new Rect(180, 90, 800, 30), GUIContent.none);
        GUI.color = Color.grey;
        for (int j = 180 + 8; j < 220; j += 8)
        {
            GUI.DrawTexture(new Rect(j, 90, 1, 10), Resources.Load<Texture>("VLine"));
        }
        for (int i = 40; i < 800; i += 40)
        {
            GUI.DrawTexture(new Rect(180 + i, 90, 2, 30), Resources.Load<Texture>("VLine"));
            for (int j = i + 8; j < i + 40; j += 8)
            {
                GUI.DrawTexture(new Rect(180 + j, 90, 0.5f, 10), Resources.Load<Texture>("VLine"));
            }
        }

        if (Event.current.button == 0 && Event.current.type == EventType.mouseDrag)
        {
            if (seekerBoundry.Contains(Event.current.mousePosition))
            {
                MovingSeeker = true;
            }
        }
        if (MovingSeeker)
            seekerBoundry = new Rect((Event.current.mousePosition.x >= 180 ?
                                     (Event.current.mousePosition.x <= 980 ?
                                      Event.current.mousePosition.x - 10 : 970) :
                                                                           170),
                85,
                seekerBoundry.width,
                seekerBoundry.height);

        if (Event.current.type == EventType.mouseUp && MovingSeeker)
            MovingSeeker = false;
        GUI.color = Color.red;
        GUI.DrawTexture(seekerBoundry, Resources.Load<Texture>("RedTriangle"));
        GUI.DrawTexture(new Rect(seekerBoundry.left + 9.1f, seekerBoundry.top + 2, 2, 400), Resources.Load<Texture>("VLine"));
        GUI.color = tmp;
    }
    void PickScale()
    {
        grid.patchWidth = (int)lastGrabbedObjScale.x;
        grid.patchHeight = (int)lastGrabbedObjScale.y;
    }
    void PickTexture()
    {
        lst5.SelectItem(selectedLevelObj.texture);
        changeSelectedBlockTexture("");
    }

    void RTNew(Rect tmpRect, Texture texture)
    {
        Rect txCords = new Rect(0, 0, 1, 1);
        float tXMin = Mathf.Max(grid.rect.xMin, tmpRect.xMin);
        float tXMax = Mathf.Min(grid.rect.xMax, tmpRect.xMax);
        float txXmin = tmpRect.xMin;
        
        
        float xScale = tmpRect.width / (tXMax - tXMin);
        float txCordsXMin = (tXMin - txXmin) / tmpRect.width;
        tmpRect.xMin = tXMin;
        tmpRect.xMax = tXMax;

        float tYMin = Mathf.Max(grid.rect.yMin, tmpRect.yMin);
        float tYMax = Mathf.Min(grid.rect.yMax, tmpRect.yMax);
        float txYmin = tYMin;


        float yScale = tmpRect.height / (tYMax - tYMin);
        float txCordsYMin =  (tYMin - txYmin) / tmpRect.height;
        tmpRect.yMin = tYMin;
        tmpRect.yMax = tYMax;

        txCords = new Rect(txCordsXMin, txCordsYMin , 1 / xScale, 1 / yScale);
        if (!(tmpRect.xMin > grid.rect.xMax || tmpRect.xMax < grid.rect.xMin))
            if (!(tmpRect.yMin > grid.rect.yMax || tmpRect.yMax < grid.rect.yMin))
                GUI.DrawTextureWithTexCoords(tmpRect, texture, txCords);
    }
    //void RasterizeTexture( Rect tmpRect, Texture texture)
    //{

    //    Rect txCords = new Rect(0, 0, 1, 1);
    //    if (tmpRect.xMax > grid.rect.xMax)
    //    {
    //        float tmpRectWidth = tmpRect.width;
    //        tmpRect.width *= ((Mathf.Min(tmpRect.xMax, grid.rect.xMax) - Mathf.Max(tmpRect.xMin, grid.rect.xMin)) / tmpRect.width);
    //        txCords = new Rect(0,
    //            0.0f,
    //            (tmpRect.width / tmpRectWidth),
    //            1);
    //    }

    //    if (tmpRect.xMin <= grid.rect.xMin)
    //    {
    //        float tmpRectWidth = tmpRect.width;
    //        // float tmpRectXmin = tmpRect.xMin;
    //        tmpRect.xMin = grid.rect.xMin;
    //        tmpRect.width *= ((Mathf.Min(tmpRect.xMax, grid.rect.xMax) - Mathf.Max(tmpRect.xMin, grid.rect.xMin)) / tmpRect.width);
    //        txCords = new Rect(1 - ((Mathf.Min(tmpRect.xMax, grid.rect.xMax) - Mathf.Max(tmpRect.xMin, grid.rect.xMin)) / tmpRectWidth),
    //                           0.0f,
    //                           tmpRect.width / tmpRectWidth,
    //                           1);
    //    }
    //    if (tmpRect.yMax >= grid.rect.yMax)
    //    {
    //        float tmpRectHeight = tmpRect.height;
    //        tmpRect.height *= ((Mathf.Min(tmpRect.yMax, grid.rect.yMax) - Mathf.Max(tmpRect.yMin, grid.rect.yMin)) / tmpRect.height);
            
    //        txCords = new Rect(txCords.xMin,
    //            1 - ((Mathf.Min(tmpRect.yMax, grid.rect.yMax) - Mathf.Max(tmpRect.yMin, grid.rect.yMin)) / tmpRectHeight),
    //            txCords.width,
    //            (tmpRect.height / tmpRectHeight));
    //    }

    //     if (tmpRect.yMin < grid.rect.yMin)
    //    {

    //        float tmpRectHeight = tmpRect.height;
    //        // float tmpRectYmin = tmpRect.yMin;
    //        tmpRect.yMin = grid.rect.yMin;


    //        tmpRect.height *= ((Mathf.Min(tmpRect.yMax, grid.rect.yMax) - Mathf.Max(tmpRect.yMin, grid.rect.yMin)) / tmpRect.height);
    //        txCords = new Rect(txCords.xMin,
    //                           0,
    //                           txCords.width,
    //                           tmpRect.height / tmpRectHeight);

    //    }
    //    if (!(tmpRect.xMin > grid.rect.xMax || tmpRect.xMax < grid.rect.xMin))
    //        if (!(tmpRect.yMin > grid.rect.yMax || tmpRect.yMax < grid.rect.yMin))
    //            GUI.DrawTextureWithTexCoords(tmpRect, texture, txCords);
    //}
    void OnGUI()
    {
        DeleteItem();

        ResizeSelectedTexture();
        if (GUI.Button(new Rect(160, 10, 50, 20), "Save"))
            SaveLevel();

        if (GUI.Button(new Rect(215, 10, 50, 20), "Delete"))
        {
            DeleteLevel();
        }
        
        if (dynamicSelection.objects.Count == 0)
            Object_MouseSelector();

        string temp = GUI.TextField(new Rect(10, 10, 140, 20), level.name);
        if (level.name != temp ||
            level.width != grid.contentWidth ||
            level.height != grid.contentHeight)
        {
            level.name = temp;
            noChangesApplied = false;
        }
        TimeSpan = (((hScroll - 180 + seekerBoundry.xMin) + 10) / (5.665f * 40)).ToString();
        TimeSpan = ((float.Parse(TimeSpan) * (0.017f / GlobalVariables.deltaTime))).ToString();
        //TimeSpan = GUI.TextField(new Rect(150, 40, 60, 20), TimeSpan);
        if (GUI.Button(new Rect(150, 40, 100, 20), "MovingCamera"))
        {
            movingCameraPivot = true;
        }
        //GUI.TextField(new Rect(120, 65, 50, 20), (level.width / (5.665f * 40)).ToString());


        float tmpGridScale = gridScale;

        gridScale = float.Parse(GUI.TextField(new Rect(115, 63, 50, 20), gridScale.ToString()));
        gridScale = GUI.VerticalScrollbar(gridScalerRect, gridScale, 0.05f, 4, 0.1f);
        if (tmpGridScale != gridScale)
        {
            grid.patchWidth = grid.basePatchWidth * gridScale;
            grid.patchHeight = grid.basePatchHeight * gridScale;
            tmpGridScale = gridScale;
        }
        GUI.Label(new Rect(250, 40, 100, 20), "Patch Height:");

        grid.basePatchHeight = float.Parse(GUI.TextField(new Rect(325, 40, 80, 20),
                                                  (grid.basePatchHeight).ToString()));
        //grid.patchHeight = 40 * gridScale;
        GUI.Label(new Rect(430, 40, 100, 20), "Patch Width:");

        grid.basePatchWidth = float.Parse(GUI.TextField(new Rect(500, 40, 80, 20),
                                                    (grid.basePatchWidth).ToString()));
        //grid.patchWidth = 40 * gridScale;
        GUI.Label(new Rect(280, 10, 100, 20), "lvl Height:");
        level.height = int.Parse(GUI.TextField(new Rect(340, 10, 80, 20),
                                                 level.height.ToString()));
        if (level.height < 1)
            level.height = 400;

        GUI.Label(new Rect(440, 10, 100, 20), "lvl Width:");
        level.width = int.Parse(GUI.TextField(new Rect(500, 10, 80, 20),
                                                  level.width.ToString()));
        if (level.width < 1)
            level.width = 800;
        if (noChangesApplied)
            GUI.DrawTexture(new Rect(115, 40, 20, 20), Resources.Load<Texture>("Checked"));
        else
            GUI.DrawTexture(new Rect(115, 40, 20, 20), Resources.Load<Texture>("NotChecked"));

        if (GUI.Button(new Rect(10, 40, 100, 20), "Commit"))
        {
            CommitChanges();
        }
        AddItem();
        if (GUI.Button(new Rect(10, 63, 100, 20), "Clear"))
        {
            foreach (var item in level.objects)
            {
                if (item is Chunk)
                {
                    deletedItems.Add(((Chunk)item).Name + "." + item.id.ToString());
                    continue;
                }

                deletedItems.Add(item.texture + "." + item.id.ToString());
            }
            level.objects.RemoveAll(x => { return true; });
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

        lst1.EventHandler();
        lst2.EventHandler();
        lst3.EventHandler();
        lst4.EventHandler();
        lst5.EventHandler();
        lst6.EventHandler();

        

        hScroll = GUI.HorizontalScrollbar(new Rect(180, 490, 800, 10), hScroll, grid.basePatchWidth / gridScale , 0, level.width - grid.rect.width);
        vScroll = GUI.VerticalScrollbar(new Rect(980, 90, 10, 400), vScroll, grid.basePatchHeight /gridScale, 0, level.height - grid.rect.height);

        vScroll = vScroll < 0 ? 0 : vScroll;
        hScroll = hScroll < 0 ? 0 : hScroll;

        grid.xMin = -hScroll*gridScale;
        grid.yMin = -vScroll*gridScale;
        
        grid.Draw();
        MoveChunk();
        GenerateChunk();
        foreach (var item in level.objects)
        {
            if (dynamicSelection.objects.Contains(item))
                continue;
            if (item is Chunk)
            {
                if (item.position.depth == 40 - ((activeLayer + 1) * 5))
                {
                    foreach (var chunkObject in ((Chunk)item).objects)
                    {

                        //if (chunkObject.position.depth == 40 - ((activeLayer + 1) * 5))
                        
                            Rect tmpRect = new Rect((chunkObject.position.left + ((Chunk)item).centerOfChunk.left - grid.rect.left - hScroll) * gridScale + grid.rect.left,
                                                    (chunkObject.position.top + ((Chunk)item).centerOfChunk.top - grid.rect.top - vScroll) * gridScale + grid.rect.top,
                                                    chunkObject.position.width * gridScale,
                                                    chunkObject.position.height * gridScale);
                            //if (!(tmpRect.xMin < grid.rect.xMin ||
                            //     tmpRect.yMin < grid.rect.yMin ||
                            //     tmpRect.xMax > grid.rect.xMax ||
                            //     tmpRect.yMax > grid.rect.yMax))
                            //    GUI.DrawTexture(tmpRect, Resources.Load<Texture>("Objects/" + chunkObject.texture));
                            RTNew(tmpRect, Resources.Load<Texture>("Objects/" + chunkObject.texture));

                    }
                    Rect pivotPos = new Rect(((Chunk)item).centerOfChunk.left - 5 - hScroll, ((Chunk)item).centerOfChunk.top - 5 - vScroll, 10, 10);
                    RTNew(pivotPos, Resources.Load<Texture>("plus"));

                }
                continue;
            }
            
            
            if (GetActiveLayer().Contains((40-(int)item.position.depth)/5))
            {
                Rect tmpRect = new Rect((((item.position.left - grid.rect.left - hScroll) * gridScale) + grid.rect.left),
                               (((item.position.top - grid.rect.top - vScroll) * gridScale) + grid.rect.top),
                               item.position.width * gridScale,
                               item.position.height * gridScale);
                RTNew(tmpRect, Resources.Load<Texture>("Objects/" + item.texture));
            }
        }

        foreach (var item in dynamicSelection.objects)
        {
            if (GetActiveLayer().Contains((40 - (int)item.position.depth) / 5))
            {
                Rect tmpRect = new Rect(((item.position.left - hScroll - grid.rect.left) * gridScale) + grid.rect.left + (dynamicSelection.boundry.xMin - dynamicSelection.firstPos.x),
                                        ((item.position.top - vScroll - grid.rect.top) * gridScale) + grid.rect.top    + (dynamicSelection.boundry.yMin - dynamicSelection.firstPos.y),
                                        item.position.width * gridScale,
                                        item.position.height * gridScale);
                //if (!(tmpRect.xMin < grid.rect.xMin ||
                //     tmpRect.yMin < grid.rect.yMin ||
                //     tmpRect.xMax > grid.rect.xMax ||
                //     tmpRect.yMax > grid.rect.yMax))
                //    GUI.DrawTexture(tmpRect, Resources.Load<Texture>("Objects/" + item.texture));
                RTNew(tmpRect, Resources.Load<Texture>("Objects/" + item.texture));
            }
        }

        Rect tmpR = new Rect(((objSelectorGrid.position.left - hScroll - grid.rect.left) * gridScale) + grid.rect.left,
                             ((objSelectorGrid.position.top -  grid.rect.top - vScroll)* gridScale) + grid.rect.top,
                             objSelectorGrid.position.width * gridScale,
                             objSelectorGrid.position.height * gridScale);
        //if (!(tmpR.xMin < grid.rect.xMin ||
        //     tmpR.yMin < grid.rect.yMin ||
        //     tmpR.xMax > grid.rect.xMax ||
        //     tmpR.yMax > grid.rect.yMax))
        //    GUI.DrawTexture(tmpR, Resources.Load<Texture>(objSelectorGrid.texture));
        RTNew( tmpR, Resources.Load<Texture>(objSelectorGrid.texture));
        GUI.DrawTexture(new Rect(190, 380, 100, 100), Resources.Load<Texture>("Coordinate"));
        if (selectedBlock != null)
        {
            float max = Mathf.Max(selectedBlock.width, selectedBlock.height);
            GUI.DrawTexture(new Rect(585, 10, 60 * (selectedBlock.width / max), 60 * (selectedBlock.height / max)), selectedBlock);
            GUI.Label(new Rect(585, 73, 100, 20), selectedBlock.width + " X " + selectedBlock.height );
        }

        List<int> selectedLayers = GetActiveLayer();
        int tempLayerNum =  GUI.SelectionGrid(new Rect(655, 10, 340, 60), activeLayer, layersName, 5);
        activeLayer = tempLayerNum;
       
        for (int i = 0; i < layers.Length; i++)
        {
            if (i == activeLayer)
            {
                layers[i] = true;
                layersName[i] = "#L" + (i + 1).ToString();
                continue;
            }
            if (!Event.current.control)
            {
                layersName[i] = "L" + (i + 1).ToString();
                layers[i] = false;
            }
        }

        if (level.cameraPivot != null)
            GUI.DrawTexture(ChangeType(level.cameraPivot), Resources.Load<Texture>("CameraPivot"));
        MovingCamera();
        TimeBar();
        AssociativeSeletion();
        window.Repaint();
    }
}


