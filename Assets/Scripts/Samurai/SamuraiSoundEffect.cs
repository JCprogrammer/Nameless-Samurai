//using UnityEngine;
//using System.Collections;

//public class SamuraiSoundEffect : MonoBehaviour {

//    public AudioSource slashSoundEffect;
//    public AudioSource chargeSoundEffect;
//    public AudioSource sheatheSoundEffect;
//    public AudioSource deathSoundEffect;
//    bool isSlashing;
//    bool isCharging;
//    bool isSheathing;
//    bool isDying;
//    void Start()
//    {
//        isSlashing = false;
//        isCharging = false;
//        isSheathing = false;
//        isDying = false;
//    }
//    public void SDie()
//    {

//        if ((!isSlashing) && (!isCharging) && (!isSheathing) && (!isDying))
//        {
//            deathSoundEffect.Play();
//            isSlashing = true;
//        }
//    }
//    public void SSlash()
//    {
//        if ((!isSlashing) && (!isCharging) && (!isSheathing) && (!isDying))
//        {
//            slashSoundEffect.Play();
//            isSlashing = true;
//        }
//    }
//    public void SCharge()
//    {
//        if ((!isSlashing) && (!isCharging) && (!isSheathing) && (!isDying))
//        {
//            chargeSoundEffect.Play();
//            isCharging = true;
//        }
//    }
//    public void SSheathe()
//    {
//        if ((!isSlashing) && (!isSheathing) && (!isDying))
//        {
//            sheatheSoundEffect.PlayDelayed(1.5f);
//            isSheathing = true;
//        }
 
//    }

//    void FixedUpdate()
//    {
//        if (!slashSoundEffect.isPlaying)
//            isSlashing = false;
//        if (!chargeSoundEffect.isPlaying)
//            isCharging = false;
//        if (!sheatheSoundEffect.isPlaying)
//            isSheathing = false;
//        if (!deathSoundEffect.isPlaying)
//            isDying = false;
    
//    }
//}
