using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GravityMotor))]
[RequireComponent(typeof(CenterRotateMotor))]
public class JumpMotor : Vector2MovementMotor
{
    float timeSinceJump = 0;
    public int JumpSpeed = 0;
    public bool allowedToJump = false;
    bool callGravityMotorIgnition = false;
    protected override void Cycle()
    {
        base.Cycle();
        if (!switchedOn)
            return;
        if (speed > 0)
        {
            speed -= GlobalVariables.baseAccelerator * (GlobalVariables.deltaTimeConst / GlobalVariables.deltaTime) ;
            //GetComponent<CenterRotateMotor>().FreeToRotate
        }
        if (speed <= 0)
        {
            if (callGravityMotorIgnition){
                SendMessage("GravityMotorIgnite", SendMessageOptions.DontRequireReceiver);
                callGravityMotorIgnition = false;
            }
            speed = 0;
        }
    }

    public override void Ignite()
    {
        base.Ignite();
        speed = 0;
    }

    public void Jump()
    {
        if (allowedToJump)
        {
            timeSinceJump = 0;
            callGravityMotorIgnition = true;
            speed = Mathf.Sqrt(JumpSpeed * GlobalVariables.baseAccelerator  * 100);
            allowedToJump = false;
            SendMessage("GravityMotorKillPower", SendMessageOptions.DontRequireReceiver);
            SendMessage("FreeToRotate", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {

        ////if (coll.transform.position.y + (coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y/2) <=
        ////   this.transform.position.y - (this.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y/2))
        //if (coll.transform.position.y + (coll.transform.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y/2) <=
        //   this.transform.position.y - (this.transform.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y/2))
        
        //{
        //    if (speed == 0 && allowedToJump == false)
        //    {
        //        allowedToJump = true;
        //    }
        //}
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (transform.position.y - (GetComponent<BoxCollider2D>().size.y/2 + (0.1f * transform.localScale.y)) >
            coll.transform.position.y + (coll.transform.GetComponent<BoxCollider2D>().size.y/2))
        {
            allowedToJump = true;
            SendMessage("FixRotationDistortion",SendMessageOptions.DontRequireReceiver);
            gameObject.transform.position = new Vector3(transform.position.x,
                coll.transform.position.y +
                        (coll.transform.GetComponent<BoxCollider2D>().size.y / 2) +
                        (GetComponent<BoxCollider2D>().size.y / 2),
                transform.position.z);
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        ////if (coll.transform.position.y + (coll.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2)  <=
        ////    this.transform.position.y - (this.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.y / 2) + 0.01f)
        ////{
        ////    if (speed == 0)
        ////        allowedToJump = false;
        ////}
        //allowedToJump = false;
    }
}
