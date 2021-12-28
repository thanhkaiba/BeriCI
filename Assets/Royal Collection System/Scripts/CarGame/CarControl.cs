using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour {

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>

  

    WheelJoint2D wj;
    JointMotor2D mo;

    public float speedCar;//Car Speed

	void Start ()
    {
        wj = gameObject.GetComponents<WheelJoint2D>()[0];
        mo = new JointMotor2D();

    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mo.motorSpeed = speedCar;
            mo.maxMotorTorque = 1000;
            wj.useMotor = true;
            wj.motor = mo;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            mo.motorSpeed = -speedCar;
            mo.maxMotorTorque = 1000;
            wj.useMotor = true;
            wj.motor = mo;
        }
        if (Input.GetKeyUp(KeyCode.A) ||  Input.GetKeyUp(KeyCode.D))
        {
            wj.useMotor = false;
        }
    }


    //Finish Controll Obe 
    private bool finishController = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish" && !finishController)
        {      
            //useMotor Car is !Active
            wj.useMotor = false;

            //FinishGame Manager Game Find
            FindObjectOfType<ManagerGame>().ComplateGame();

            //CarController enabled is !Active
            gameObject.GetComponent<CarControl>().enabled = false;


            //finish
            finishController = true;

        }
    }
}
