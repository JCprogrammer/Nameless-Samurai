using UnityEngine;
using System.Collections;

public class BloodEffectController : MonoBehaviour {
	//public Transform cameraTransform;
	public float bloodEffectLength = 30.0F;
	private bool disappearing = false;
	public float fadeOutSpeed = 1.0F;
	//public GameObject collidingGameObject;
    Vector3 playerAppearNormalPosition;
    Vector3 bloodAppearPosition;
    public float scale = 0.93F;
    public Vector3 offset;
    public bool isAppear = false;
    //public bool isCollidingAppear = false;

	public void Adjust()
	{
		Vector3 bloodEffectStartNormalizedPosition = Camera.main.WorldToScreenPoint(GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>().bounds.center);
		Vector3 bloodEffectEndNormalizedPosition = Camera.main.WorldToScreenPoint(GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>().bounds.center + new Vector3(bloodEffectLength, 0.0F, 0.0F));
		bloodEffectStartNormalizedPosition = new Vector3(bloodEffectStartNormalizedPosition.x / Screen.width, (bloodEffectStartNormalizedPosition.y / Screen.height), 0.0F);
		bloodEffectEndNormalizedPosition = new Vector3(bloodEffectEndNormalizedPosition.x / Screen.width, (bloodEffectEndNormalizedPosition.y / Screen.height), 0.0F);

        transform.position = new Vector3(((bloodEffectEndNormalizedPosition.x + bloodEffectStartNormalizedPosition.x) / 2.0F) + offset.x,
                                         ((bloodEffectEndNormalizedPosition.y + bloodEffectStartNormalizedPosition.y) / 2.0F) + offset.y,
                                         offset.z);

		transform.localScale = new Vector3((bloodEffectEndNormalizedPosition.x - bloodEffectStartNormalizedPosition.x) * scale,
		                                   (bloodEffectEndNormalizedPosition.x - bloodEffectStartNormalizedPosition.x) * scale * (107.0F / 600.0F) * (Screen.width / Screen.height),
		                                   1.0F);
		//float startPlayerRelativePositionX = player.position.x - cameraTransform.position.x;
	}

	public void Appear()
	{
        //bloodAdjustNormalPosition = transform.position;
		//Adjust();
        playerAppearNormalPosition = Camera.main.WorldToScreenPoint(GameObject.FindGameObjectWithTag("Player").transform.position);
        playerAppearNormalPosition = new Vector3(playerAppearNormalPosition.x / Screen.width, playerAppearNormalPosition.y / Screen.height, playerAppearNormalPosition.z);
        bloodAppearPosition = transform.position;

        isAppear = true;
        //isCollidingAppear = true;
        

		/*collidingGameObject = new GameObject("Colliding");
        collidingGameObject.tag = "Sword";
		collidingGameObject.SetActive(false);
		collidingGameObject.transform.localPosition = new Vector3(GameObject.FindGameObjectWithTag("Player").renderer.bounds.center.x  + (bloodEffectLength / 2.0F), GameObject.FindGameObjectWithTag("Player").renderer.bounds.center.y, GameObject.FindGameObjectWithTag("Player").renderer.bounds.center.z);
		collidingGameObject.transform.localScale = new Vector3(bloodEffectLength, GameObject.FindGameObjectWithTag("Player").transform.lossyScale.y, 10.0F);
		collidingGameObject.AddComponent<BoxCollider2D>();
		collidingGameObject.GetComponent<BoxCollider2D>().isTrigger = false;
		collidingGameObject.AddComponent<Rigidbody2D>();
		collidingGameObject.GetComponent<Rigidbody2D>().isKinematic = true;*/

		GetComponent<GUITexture>().color = new Color(0.5F, 0.5F, 0.5F, 0.5F);
	}

	/*public void ActiveCollider()
	{
        collidingGameObject.transform.Translate(0.0F, 0.1F, 0.0F);
		collidingGameObject.SetActive(true);
	}*/

	public void Disappear()
	{
        /*isCollidingAppear = false;
		Destroy(collidingGameObject);*/

		disappearing = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (isAppear)
        {
            Vector3 playerNormalPosition = Camera.main.WorldToScreenPoint(GameObject.FindGameObjectWithTag("Player").transform.position);
            playerNormalPosition = new Vector3(playerNormalPosition.x / Screen.width, playerNormalPosition.y / Screen.height, playerNormalPosition.z);

            float positionDifference = playerNormalPosition.x - playerAppearNormalPosition.x;

            
            transform.localPosition = new Vector3(bloodAppearPosition.x + positionDifference, bloodAppearPosition.y, bloodAppearPosition.z);
            
        }

        /*if(isCollidingAppear)
        {
            collidingGameObject.transform.localPosition = transform.localPosition;
        }*/


		if(disappearing)
		{
			GetComponent<GUITexture>().color = new Color(0.5F, 0.5F, 0.5F, GetComponent<GUITexture>().color.a - (fadeOutSpeed * Time.deltaTime));

            if (GetComponent<GUITexture>().color.a <= 0.0F)
            {
                disappearing = false;
                isAppear = false;
            }
		}
	}
}
