using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ManagerGame : MonoBehaviour {

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>



    public GameObject ground_ComplateGame;/// Ground Complate Canvas



    public void ComplateGame()
    {

        //ground_ComplateGame is Active
        ground_ComplateGame.SetActive(true);

        //ground_ComplateGame is Aniamtions
        ground_ComplateGame.transform.DOShakeScale(1, 1).SetEase(Ease.OutQuint).Play().OnComplete(() =>
        {
            FindObjectOfType<RoyalStarsManager>().On3Stars();///Stars On 3

        });


    }


    //Change Scene Demo
    public void ChangeScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);//LoadScene
    }

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }
}
