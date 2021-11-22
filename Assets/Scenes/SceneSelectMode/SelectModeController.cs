using Piratera.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectModeController : MonoBehaviour
{
    [SerializeField]
    private Button buttonEasy;
    [SerializeField]
    private Button buttonHard;
    [SerializeField]
    private Button buttonMedium;
    void Start()
    {
        buttonEasy.onClick.AddListener(delegate { OnSelectMode(GameMode.EASY); });
        buttonMedium.onClick.AddListener(delegate { OnSelectMode(GameMode.MEDIUM); });
        buttonHard.onClick.AddListener(delegate { OnSelectMode(GameMode.HARD); });
    }

    public void OnSelectMode(GameMode mode)
    {
        Debug.Log("OnSelectMode: " + GameMode.HARD);
        if (UserData.Instance.IsEnoughBeri(UserData.Instance.StaminaConfig.pve_cost))
        {
            NetworkController.Send(SFSAction.COMBAT_BOT);
        }
        else
        {
            GuiManager.Instance.ShowPopupNotification("Not Enough Beri!");
        }
    
    }

    public void OnBack()
    {
        SceneManager.LoadScene("SceneLobby");
    }
}
