using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MainController : MonoBehaviour
{

    public static MainController instance;
    public List<Motor> motors;
    // Use this for initialization

    void Start()
    {
        motors = new List<Motor>(FindObjectsOfType<Motor>());
        instance = this;
    }

    public void addNewMotor(Motor motor)
    {
        motors.Add(motor);
    }
    void FixedUpdate()
    {
        for (int i = 0; i < motors.Count; i++)
        {


            if (motors[i] != null)
            {
                if (motors[i].enabled == true)
                    motors[i].MotoUpdate();
            }
        }
    }
}
