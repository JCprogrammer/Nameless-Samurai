using UnityEngine;
using System.Collections;

public class Motor : MonoBehaviour
{

    public float igniteAfter = -1;
    protected bool givenPriodicAction = false;
    public float killPowerBy = -1;
    public bool igniteOnStart = false;
    protected double timeStep = 1;
    private double counter = 0;
    private bool triggered = false;
    public bool switchedOn = false;
    protected double timeSinceIgnition;
    public bool DebuggingMode = false;

    public bool record;
    public Stack positionMotionStack;
    public Stack rotationMotionStack;
    void Start()
    {
        positionMotionStack = new Stack();
       
        if (igniteOnStart && igniteAfter < 0)
            Ignite();
        else if (igniteAfter >= 0)
        {
            givenPriodicAction = true;
        }
        timeStep = 1.0f / GlobalVariables.framePerSecond;
    }
    public bool PowerStatus
    {
        get { return switchedOn; }
    }
    public virtual void Ignite()
    {
        if (DebuggingMode)
            Debug.Log(gameObject.name + "  Ignition Time: " + timeSinceIgnition);

        switchedOn = true;
    }
    protected virtual void KillPower()
    {
        if (DebuggingMode)
            Debug.Log(gameObject.name + "  KillPower Time: " + timeSinceIgnition);
        switchedOn = false;
    }
    protected virtual void Cycle()
    {
        timeSinceIgnition += GlobalVariables.deltaTimeConst;
    }
    public void MotoUpdate()
    {
        //timeStep = 1.0f / GlobalVariables.framePerSecond;
        RoundPerTimeStep();
        //Cycle();
    }
    void RoundPerTimeStep()
    {
        if (timeStep >= 1)
        {
            counter += GlobalVariables.deltaTime;
            if (((int)counter) % timeStep == 0)
            {
                if (!triggered)
                {
                    Cycle();
                    triggered = true;
                    counter = 0;
                }
            }
            if (counter + GlobalVariables.deltaTime >= timeStep)
            {
                triggered = false;
            }
        }
        else if (timeStep < 1)
        {
            counter += GlobalVariables.deltaTime / timeStep;
            if (((int)counter) % 1 == 0)
            {
                if (!triggered)
                {
                    Cycle();
                    triggered = true;
                    counter = 0;
                }
            }
            if (counter + (GlobalVariables.deltaTime / timeStep) >= 1)
            {
                triggered = false;
            }
        }
    }
    public void UpdateTimePeriod()
    {
        killPowerBy += (float)timeSinceIgnition;
        igniteAfter += (float)timeSinceIgnition;
    }

    public void EnableMe()
    {
        
        Debug.Log("Enabled");
        this.enabled = true;
    }
}
//[System.Serializable]
//public class MotionStack
//{
//    public SMotion root;
  

//    public MotionStack()
//    {
//        root = new SMotion();
//        root.nextMotion = new SMotion();
//        //TOS = root.nextMotion;
//    }
    

//    public void Push(SMotion item)
//    {
//       if(root.nextMotion == null)
//        else
//        {
//            SMotion newI = new SMotion();
//            newI.content = item.content;
//            TOS.nextMotion = newI;
            
//            SMotion temp = root.nextMotion;
//            while (true)
//            {
//                if (temp.nextMotion == TOS)
//                { }
//            }
//        }






//        Debug.Log(root.nextMotion.content.ToString());
//    }

//    public SMotion Pop()
//    {
//        if (TOS == root)
//        {
//            SMotion temp = root;
//            root = null;
//            return temp;
//        }
//        else
//        {
//            SMotion temp = root.nextMotion;
//            while (true)
//            {
//                if (temp.nextMotion == TOS)
//                {
//                    TOS = temp;
//                    SMotion tmp2 = TOS.nextMotion;
//                    TOS.nextMotion = null;
//                    return tmp2;
//                }
//                else
//                {
//                    temp = temp.nextMotion;
//                }
//            }
            
//        }
//    }
//}
[System.Serializable]
public class SMotion
{
    public object content;
    public SMotion nextMotion;
}
