using UnityEngine;
using System.Collections;

public class CameraRoller : MonoBehaviour {


	Camera gameCamera;
	public float boundaryRatio = 0.7f;
	Vector2 playerPositionOffset;
    public bool unbalanced = false;
    public bool playerLost = false;
    public float upwardAdjustingSpeed = 0.05f;
    public float downwardAdjustingSpeed = 0.5f;
	// Use this for initialization
	void Start () {
		playerPositionOffset = new Vector2 (transform.position.x - GameObject.FindGameObjectWithTag ("Player").transform.position.x,
		                                   transform.position.y - GameObject.FindGameObjectWithTag ("Player").transform.position.y);

		gameCamera = Camera.main;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (playerLost)
        {
            if ((GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0,0,transform.position.z) -
                    (transform.position - (Vector3)playerPositionOffset)).magnitude > 3f)
            {
                transform.Translate((GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0,0,transform.position.z) -
                    (transform.position - (Vector3)playerPositionOffset)) * GlobalVariables.deltaTime * 1f);
            }
            else
            {
                transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + (Vector3)playerPositionOffset + new Vector3(0, 0, transform.position.z);
                GetComponent<Vector2MovementMotor>().enabled = true;
                playerLost = false;
            }
            return;
        }



		Vector2 diff = new Vector2 (transform.position.x - GameObject.FindGameObjectWithTag ("Player").transform.position.x,
		                            transform.position.y - GameObject.FindGameObjectWithTag ("Player").transform.position.y);
        if ( (diff - playerPositionOffset).magnitude > 0.01f)
        {

            if (!IsInBoundary(this.GetComponent<Camera>(), GameObject.FindGameObjectWithTag("Player").transform, boundaryRatio))
            {
                unbalanced = true;

            }
            else
                unbalanced = false;
            if (unbalanced)
            {
                 //transform.Translate((playerPositionOffset - diff) * GlobalVariables.deltaTime * 1f);
                float speed = GameObject.FindWithTag("Player").transform.position.y > transform.position.y ? upwardAdjustingSpeed : downwardAdjustingSpeed;
                transform.position = Vector3.Lerp(transform.position, transform.position + (Vector3)(playerPositionOffset - diff), speed * GlobalVariables.deltaTime ); 
            }
        }
        else
        {
            unbalanced = false;
            //transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 0, transform.position.z) + (Vector3)playerPositionOffset;
        }
	}

	bool IsInBoundary(Camera gameCamera, Transform squareTransform, float boundaryRatio)
	{
        

		if(squareTransform.position.y- 1 >= gameCamera.transform.position.y  - (gameCamera.orthographicSize * boundaryRatio) &&
		   squareTransform.position.y - 1 <= gameCamera.transform.position.y  + (gameCamera.orthographicSize * boundaryRatio))
			return true;
		else
			return false;
	}

    public void FindPlayer()
    {
        playerLost = true;
        GetComponent<Vector2MovementMotor>().enabled = false;
    }
	void OnGUI()
	{
		float height = gameCamera.WorldToScreenPoint(new Vector3(0.0F, gameCamera.orthographicSize * boundaryRatio)).y -
			gameCamera.WorldToScreenPoint(new Vector3(0.0F, -gameCamera.orthographicSize * boundaryRatio)).y;

		//GUI.Box (new Rect (0, (Screen.height - height) / 2.0F , Screen.width, height), GUIContent.none);
	}
}
