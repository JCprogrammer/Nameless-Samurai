using UnityEngine;
using System.Collections;
using System.Reflection;
public class AdvancedV2DMotor : Vector2MovementMotor {

    public int groupNo = 0;
    public CallBackAction[] actions;
    public void Refresh()
    {
        timeSinceIgnition = 0;
        this.enabled = false;
    }

    //protected override void Cycle()
    //{
    //    bas
    //    if (showTimeSinceIgnition)
    //        Debug.Log("Time Since Ignition: " + timeSinceIgnition);
    //    if (!switchedOn)
    //    {
    //        if (givenPriodicAction)
    //        {
    //            if (timeSinceIgnition >= igniteAfter &&
    //timeSinceIgnition < killPowerBy)
    //                Ignite();
    //            else
    //                return;
    //        }
    //    }
    //    if (switchedOn)
    //    {
    //        if (killPowerBy > 0)
    //        {
    //            if (timeSinceIgnition >= killPowerBy)
    //            {
    //                KillPower();
    //                return;
    //            }
    //        }
    //    }
    //    if (!switchedOn)
    //        return;
    //    countingStarts++;
    //    transform.position += directionVector *
    //        ((speed * (GlobalVariables.deltaTimeConst)) /
    //         (float)GlobalVariables.minifier);
    //}
    
    protected override void KillPower()
    {
        base.KillPower();
        Debug.Log("Kill Power Triggered");
        foreach (var action in actions)
        {
            foreach (var target in action.targets)
            {
                AdvancedV2DMotor[] components = target.GetComponents<AdvancedV2DMotor>();
                foreach (var item in components)
                {
                    if (item.groupNo == action.targetGroupNo)
                    {
                        MethodInfo mi = item.GetType().GetMethod(action.targetMessage);
                        mi.Invoke(item, null);
                    }
                }
            }
        }
    }
[System.Serializable]
    public class CallBackAction
    {
        public string targetMessage = "";
        public GameObject[] targets;
        public int targetGroupNo;
    }
}

