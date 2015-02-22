using UnityEngine;
using System.Collections;

public class HazardScensor : MonoBehaviour
{

    public string[] actions;
    // Use this for initialization
    public float sensationThreshold = 0.1f;
    void Start()
    {

    }
    public void Destruct()
    {
        Camera.main.SendMessage("KillPower");
        Destroy(this.gameObject);
    }
    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void LoadLastCheckPoint()
    {
        Camera.main.SendMessage("LoadFromLastCheckpoint");
    }
    public void KillPowerAll()
    {
        SendMessage("KillPower");
        Camera.main.SendMessage("KillPower");
    }
    void OnCollisionStay2D(Collision2D coll)
    {
      //if(gameObject.transform.position.x < coll.transform.position.x)
      if(true)
        {
            //if ((coll.gameObject.transform.position.y - coll.gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y/2) <
            //   (this.gameObject.transform.position.y + this.gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y/2) - scensationThreshold)
            //{
          if (!(gameObject.transform.position.y - gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2 + sensationThreshold >
            coll.transform.position.y + coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2))
          {
                //if ((coll.gameObject.transform.position.y + coll.gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y/2) - scensationThreshold >
                //   (this.gameObject.transform.position.y - this.gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y/2) + scensationThreshold)
                //{
                    foreach (var item in actions)
                    {
                        SendMessage(item, SendMessageOptions.DontRequireReceiver);
                    }
               // }
            }
        }

        

    }
}
