using UnityEngine;
using System.Collections;

public class Vector2MovementMotor : Motor {
	public Direction direction = Direction.center;
	protected Vector3 directionVector;
	public float speed = 10;
	protected static int countingStarts = 0;
	public bool showTimeSinceIgnition = false;


    protected override void Cycle()
    {
        base.Cycle();
        //if (showTimeSinceIgnition)
        // Debug.Log("Time Since Ignition: " + timeSinceIgnition);
        if (!switchedOn)
        {
            if (givenPriodicAction)
            {
                if (timeSinceIgnition >= igniteAfter)
                {
                    Ignite();
                    givenPriodicAction = false;
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
                    givenPriodicAction = false;
                    return;
                }
            }
        }
        if (!switchedOn)
            return;
        countingStarts++;
        if (GlobalVariables.deltaTimeConst >= 0)
        {
            SMotion m1 = new SMotion();
            m1.content = transform.position;
            //positionMotionStack.Push(m1);
            transform.position += directionVector *
                ((speed * (GlobalVariables.deltaTimeConst)) /
                 (float)GlobalVariables.minifier);
        }
        else
        {
            if(positionMotionStack.Count > 0)
            transform.position = (Vector3)(((SMotion)positionMotionStack.Pop()).content);
        }

    }
	public override void Ignite ()
	{
		base.Ignite ();
		//Debug.Log (gameObject.name + " Ignited " + this.GetType().ToString());
        record = true;
        switch (direction) {
		case Direction.left:
			directionVector = new Vector3(-1,0,0);
			break;
		case Direction.down:
			directionVector = new Vector3(0,-1,0);
			break;
		case Direction.right:
			directionVector = new Vector3(1,0,0);
			break;
		case Direction.up:
			directionVector = new Vector3(0,1,0);
			break;
		default:
			directionVector = new Vector3(0,0,0);
			break;
		}
	}

    public void EnableVector2DMotor()
    {
        EnableMe();
    }

}
