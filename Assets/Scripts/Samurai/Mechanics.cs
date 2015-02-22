//using UnityEngine;
//using System.Collections;

//public class Mechanics : MonoBehaviour {

//    bool isDead = false;
//    bool isChargeReady = true;
//    float ChargeCoolDown = 12;
//    public BloodEffectController bloodEffect;
//    public GameObject blitzEffect;
//    public GameObject character;
//    public GameObject swordSlashCollider;
//    public GameObject swordChargeCollider;
//    public float SlashDuration = 1;
//    bool isBloodEffectApeared;
//    float endSlashTime;
//    bool holdToCharge;
//    bool ChargeOn;
//    bool moveCharacter;
//    float chargeDelay = 1;
//    Vector3 chargeDestination;
//    public float timeSinceHold;
//    bool decreaseTime = false;
//    bool refreshWithDelay;
//    float refreshDelay = 1f;
//    bool isDisappearing;
//    float disapearanceDelay;
//    float ignitionDelay = 0;
//    bool delayingIgnition;
//    public bool initiated;
//    public Texture jumpTexture;
//    public float initiationDelay = 0;

//    public void LoadMenu()
//    {
//        Application.LoadLevel("menu");
//    }
//    void Jump()
//    {
//        if(initiated && (!isDead))
//            character.GetComponent<Animation>().ChangeAnimation(5);
//    }
//    public void Die()
//    {
//        if (!isDead)
//        {
//            SendMessage("KillPower");
//            character.GetComponent<Animation>().ChangeAnimation(4);
//            Camera.main.SendMessage("KillPower");
//            Camera.main.cullingMask = 1 << 8;
//            character.GetComponent<Animation>().animationSpeed = 1.5f;
//            isDead = true;
//            SendMessage("SDie");
//        }
//    }
//    public void Initiate()
//    {
//        GetComponent<JumpMotor>().Ignite();
//        initiated = true;
//        swordSlashCollider.GetComponent<Renderer>().enabled = true;
//    }
//    public void Slash()
//    {
//        if (!delayingIgnition)
//        {
//            if (!ChargeOn)
//            {
//                if (!holdToCharge)
//                {

//                    if (Time.timeSinceLevelLoad > endSlashTime)
//                    {
//                        holdToCharge = true;
//                        ChargeOn = false;
//                        timeSinceHold = Time.timeSinceLevelLoad;
//                        isBloodEffectApeared = false;
//                    }
//                }
//            }
//        }
//    }
//    void CheckJumpStatus()
//    {
//        if (GetComponent<JumpMotor>().allowedToJump)
//            character.GetComponent<Animation>().ChangeAnimation(6);
//    }
//    void Update()
//    {
        
//        //Debug.Log(Time.timeSinceLevelLoad);
//        if(!initiated)
//            if (Time.timeSinceLevelLoad > initiationDelay)
      
//                Initiate();

//        if (isDead)
//            return;
//        if (!initiated)
//            return;
//        if (!isChargeReady)
//        {
//            if (ChargeCoolDown > 0)
//            {
//                ChargeCoolDown -= 10 * Time.deltaTime;

//            }
//            else
//            {
//                isChargeReady = true;
//                ChargeCoolDown = 8;
//            }
//        }
        
//        if (isDisappearing)
//        {
//            if (disapearanceDelay > 0)
//            {
//                disapearanceDelay -= Time.deltaTime * 20;
//            }
//            else 
//            {
//                DisapearBloodEffect();
//            }
//        }
//        if (refreshWithDelay)
//        {
//            if (refreshDelay > 0)
//            {
//                refreshDelay -= 150 * Time.deltaTime;
//            }
//            else
//            {
//                character.GetComponent<Animation>().ChangeAnimation(3);
         
//                delayingIgnition = true;
//                ignitionDelay = 1;
//                Camera.main.GetComponent<TimeC>().deltaTime = 1;
//                ChargeOn = false;
//                refreshWithDelay = false;
//            }
//        }
//        if (delayingIgnition)
//        {
//            if (ignitionDelay > 0)
//            {
//                ignitionDelay -= 5 * Time.deltaTime;
//                //Debug.Log(ignitionDelay);
//            }
//            else
//            {
//                Debug.Log("messageSend");
//                SendMessage("Ignite");
//                delayingIgnition = false;
//            }
//        }
//        if (decreaseTime)
//        {
//            if (Camera.main.GetComponent<TimeC>().deltaTime > 0)
//            {
//                Camera.main.GetComponent<TimeC>().Decrease();
//                character.GetComponent<Animation>().animationSpeed = character.GetComponent<Animation>().animationSpeed > 0 ?
//                    character.GetComponent<Animation>().animationSpeed - 0.1f : 0;
//            }
//            else
//            {
//                decreaseTime = false;
//                character.GetComponent<Animation>().ChangeAnimation(0);
//                SendMessage("SCharge");
//                moveCharacter = true;
//                swordChargeCollider.SetActive(true);
//                GetComponent<BoxCollider2D>().enabled = false;
//                chargeDestination = transform.position + new Vector3(30, 0, 0);
//                chargeDelay = 0.35f;
//                bloodEffect.bloodEffectLength = 30;
//                bloodEffect.Adjust();
//                blitzEffect.GetComponent<UnityEngine.Animation>()["BlitzEffect Animation"].speed = 10.0F;
//                blitzEffect.GetComponent<UnityEngine.Animation>().Play();
                
//            }
//        }
//        else if(moveCharacter)
//        {
//            if (chargeDelay > 0)
//            {

//                chargeDelay -= 0.01f ;
//                return;
//            }
//            if (chargeDestination.x - 1 > transform.position.x)
//                transform.position = Vector3.Lerp(transform.position, chargeDestination, Time.deltaTime * 150);
//            else
//            {
//                SendMessage("SSheathe");
//                transform.position = chargeDestination;
//                character.GetComponent<Animation>().ChangeAnimation(2);
//                swordChargeCollider.SetActive(false);
//                GetComponent<BoxCollider2D>().enabled = true;
//                moveCharacter = false;
//                holdToCharge = false;
//                decreaseTime = false;
//                isBloodEffectApeared = false;
//                character.GetComponent<Animation>().animationSpeed = 5;
                
//            if (!isBloodEffectApeared)
//            {
//                bloodEffect.Appear();
//                isBloodEffectApeared = true;
//                isDisappearing = true;
//                disapearanceDelay = 1;
//            }
//            }
//        }

//    }
//    void DisapearBloodEffect()
//    {
//        bloodEffect.Disappear();
//    }
//    void RefreshWithDelay()
//    {
               
//        refreshWithDelay = true;
//        refreshDelay = 1;
//    }
//    void FixedUpdate()
//    {
//        if (isDead)
//            return;
//        if (!initiated)
//            return;
//        if (holdToCharge)
//        {
//            if (Input.GetKeyUp(KeyCode.Space) && (!ChargeOn))
//            {
//                swordSlashCollider.SetActive(true);
//                endSlashTime = Time.timeSinceLevelLoad + SlashDuration;
//                swordSlashCollider.GetComponent<Animation>().ChangeAnimation(0);
//                holdToCharge = false;
//                SendMessage("SSlash");
//                //Debug.Log(ChargeOn);
//            }
//            if (Time.timeSinceLevelLoad > timeSinceHold + 0.03f)
//            {
//                if (isChargeReady)
//                {
//                    holdToCharge = false;
//                    ChargeOn = true;
//                    isChargeReady = false;
//                }
//            }
//        }
//        if (Time.timeSinceLevelLoad > endSlashTime)
//            swordSlashCollider.SetActive(false);
//        //else
//        //    Debug.Log(Time.timeSinceLevelLoad);
//        if (ChargeOn)
//        {
//            if (Camera.main.GetComponent<TimeC>().deltaTime > 0)
//                decreaseTime = true;
            
//        }
//    }

//}
