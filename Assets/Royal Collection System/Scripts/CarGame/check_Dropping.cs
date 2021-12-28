using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class check_Dropping : MonoBehaviour
{



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Land")
        {
            //GameOver And Dropping Car
            SceneManager.LoadScene("Car2dGame");//LoadScene
        }
    }

}
