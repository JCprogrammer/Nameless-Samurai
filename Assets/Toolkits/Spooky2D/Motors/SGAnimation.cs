using UnityEngine;
using System.Collections;

[System.Serializable]
public enum AnimationMode
{
    loop,
    oneShot
}

public class SGAnimation : Motor{

    public Texture[] textures;
    public SubAnimation defaultAnimation;
    public SubAnimation[] subAnimations;
    public float currenttextureId = 0;
    public AnimationCurve curve;
    SubAnimation currentAnimation;
    public float animationSpeed;
    public bool isAnimating;
    public float animationDelay = 0;
    public bool animateOnDelay = false;
    float delayCounter = 0;

    public override void Ignite()
    {
        base.Ignite();
        currentAnimation = defaultAnimation;
        if(defaultAnimation.indexBox.Length > 0)
        currenttextureId = defaultAnimation.indexBox[0];
        currenttextureId = 0;
        if (defaultAnimation.mode == AnimationMode.loop)
        {
            isAnimating = true;
        }
        if (animateOnDelay)
        {
            delayCounter = (float)timeSinceIgnition + animationDelay ;
            Debug.Log(delayCounter);
        }
    }
    protected override void Cycle()
    {

        base.Cycle();
        if (animateOnDelay)
        {
            if (timeSinceIgnition < delayCounter)
            {
                return;
            }
        }


        if (!switchedOn)
        {
            if (givenPriodicAction)
            {
                if (timeSinceIgnition >= igniteAfter){
                
                    givenPriodicAction = false;
                    Ignite();
                }
                else
                    return;
            }
        }
        if (switchedOn)
        {
            if (igniteAfter >= 0)
            {
                if (timeSinceIgnition >= killPowerBy)
                {
                    KillPower();
                    return;
                }
            }
        }
        if (!switchedOn)
            return;
        if (isAnimating)
        {
            if (textures.Length == 0)
                return;

            if (currenttextureId < currentAnimation.indexBox[1] + 0.5f)
                currenttextureId += 0.005f * animationSpeed * (GlobalVariables.deltaTime / 0.02f);
            
            else if (currentAnimation.mode == AnimationMode.oneShot)
            {
                foreach (var item in currentAnimation.targetObjects)
                {
                    foreach (var message in currentAnimation.messageAfterAnimation)
                    {
                        item.SendMessage(message, SendMessageOptions.DontRequireReceiver);
                        
                    }
                }

                if (defaultAnimation.mode == AnimationMode.loop)
                {
                    if (currentAnimation.switchToDefault)
                    {
                        currentAnimation = defaultAnimation;
                        currenttextureId = currentAnimation.indexBox[0];
                    }
                    else
                        isAnimating = false;
                }
                else
                {
                    isAnimating = false;
                }      
            }
            else
            {
                currenttextureId = currentAnimation.indexBox[0];
                foreach (var item in currentAnimation.targetObjects)
                {
                    foreach (var message in currentAnimation.messageAfterAnimation)
                    {
                        item.SendMessage(message, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
                if ((int)currenttextureId <= textures.Length - 1)
            transform.renderer.material.mainTexture = textures[(int)currenttextureId];
        }
       
    }
   public void ChangeAnimation(int subAnimation)
    {
        currentAnimation = subAnimations[subAnimation];
        currenttextureId = currentAnimation.indexBox[0];
        isAnimating = true;
        if (animateOnDelay)
        {
            delayCounter = Time.timeSinceLevelLoad + animationDelay;
        }
    }
    [System.Serializable]
    public class SubAnimation
    {
        public bool switchToDefault = true;
        public AnimationMode mode;
        public int[] indexBox;
        public GameObject[] targetObjects;
        public string[] messageAfterAnimation;
    }
}
