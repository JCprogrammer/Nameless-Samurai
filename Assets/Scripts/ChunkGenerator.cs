using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChunkGenerator : MonoBehaviour
{
    [HideInInspector]
    public bool[] visibleLayers = new bool[10] {false,
                                                false,
                                                false,
                                                false,
                                                false,
                                                false,
                                                false,
                                                false,
                                                false,
                                                false};
    public List<LevelLayer> levelLayers = new List<LevelLayer>(0);
    public List<LayerSectionInformation> layersInfo = new List<LayerSectionInformation>(0);
    //public ChunkGenerator me;
    
    //public GameObject currentChunk;
    //[HideInInspector]
    //public float timeForNextGeneration = 0;
    //public float HorizontalSpeed;
    //bool isFadingIn;

    //public override void Ignite()
    //{
    //    base.Ignite();
    //    Generate();
    //}
    //void Generate()
    //{
    //    // Random.seed = System.DateTime.Now.Millisecond;
    //    int randomIndex = Random.Range(0, chunks.Count);
    //    Motor sample = (Instantiate(chunks[randomIndex], transform.position, Quaternion.identity) as GameObject).GetComponent<Motor>();
    //    if (sample != null)
    //    {
    //        //MainController.instance.addNewMotor(sample);
    //        Motor[] children = new Motor[0];
    //        if (sample.transform.childCount > 0)
    //            children = sample.GetComponentsInChildren<Motor>();
    //        foreach (var item in children)
    //        {
    //            MainController.instance.addNewMotor(item);
    //        }
    //    }
    //    currentChunk = chunks[randomIndex];
    //    timeForNextGeneration = (float)timeSinceIgnition + (currentChunk.GetComponent<ChunkData>().chunkLength / (HorizontalSpeed / GlobalVariables.minifier));
    //    Debug.Log((currentChunk.GetComponent<ChunkData>().chunkLength / (HorizontalSpeed / GlobalVariables.minifier)));
    //}
    //protected override void Cycle()
    //{
    //    base.Cycle();
    //    if (timeSinceIgnition >= timeForNextGeneration)
    //    {
    //        Generate();
    //    }
    //}
    //public override void KillPower()
    //{
    //    base.KillPower();
    //}


    public class LayerSectionInformation
    {
        public string searchPhrase;
        public bool showSelecteds;
        public bool showNotSelecteds;
        public bool showPreview;
        
        public LayerSectionInformation()
        {
            searchPhrase = "";
            showSelecteds = true;
            showNotSelecteds = true;
            showPreview = true;
        }
        
    }
}
[System.Serializable]
public class LevelLayer
{
    public bool[] chunksSelected = new bool[0];
    public List<GameObject> chunks;
    public Vector3 initiationPointOfGeneration;
    public float[] hSpeed;
    public GameObject currentChunk;
    public LevelLayer()
    {
        
        initiationPointOfGeneration = Vector3.zero;
    }
}

