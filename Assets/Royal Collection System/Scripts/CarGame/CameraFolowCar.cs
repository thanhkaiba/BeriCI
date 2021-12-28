using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFolowCar : MonoBehaviour {

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>

    Transform Car;


    private void Start()
    {
        Car = GameObject.Find("Car").transform;

    }
    void Update ()
    {
        if (transform.position.x < Car.position.x)
        {
            transform.Translate(.08f,0f,0f);
        }
        if (transform.position.x > Car.position.x)
        {
            transform.Translate(-.08f, 0f, 0f);
        }
    }
}
