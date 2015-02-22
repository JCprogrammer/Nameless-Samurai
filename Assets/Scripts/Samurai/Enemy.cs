//using UnityEngine;
//using System.Collections;

//public class Enemy : MonoBehaviour {

//    public GameObject character;
//    public GameObject bloodSplash;
//    public AudioSource bloodSplashSFX;
//    bool triggered;

//    // Use this for initialization
//    void Start () {
//        triggered = false;
//    }
	
//    // Update is called once per frame
//    void Update () {
	
//    }

//    void OnTriggerStay2D(Collider2D coll)
//    {
//        if (!triggered)
//        {
//            if (coll.tag == "Sword")
//            {
//                triggered = true;
//                bloodSplash.renderer.enabled = true;
//                bloodSplash.GetComponent<Animation>().ChangeAnimation(0);
//                collider2D.enabled = false;
//                character.GetComponent<Animation>().animationSpeed = 0;
//                Debug.Log(coll.name);
//            }
//        }
//    }

//    void OnTriggerEnter2D(Collider2D coll)
//    {
       
//        if (!triggered)
//        {
//            if (coll.tag == "Sword")
//            {
//                triggered = true;
//                bloodSplash.renderer.enabled = true;
//                bloodSplash.GetComponent<Animation>().ChangeAnimation(0);
//                collider2D.enabled = false;
//                character.GetComponent<Animation>().animationSpeed = 0;
//                Debug.Log(coll.name);
//                if (!bloodSplashSFX.isPlaying)
//                {
//                    bloodSplashSFX.Play();
//                }
//            }
//        }
//    }
//    void KillMe()
//    {
//        Destroy(gameObject);
//    }

//}
