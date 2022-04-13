using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Piratera.Config;
using Piratera.Network;

public class UpgradeShip : MonoBehaviour
{
    [SerializeField] private GameObject sail, helm;
    [SerializeField] private GameObject sailCanvas, helmCanvas;
    private Text sailLevelTxt, helmLevelTxt;
    private Text sailUpgradeBtnLabel, helmUpgradeBtnLabel;
    private GameObject sailUpgradeBtn, helmUpgradeBtn;

    private int sailLevel, helmLevel;
    private const int MAX_SAIL_LEVEL = 7;
    private const int MAX_HELM_LEVEL = 7;

    // Start is called before the first frame update
    void Start()
    {
        //NetworkController.AddServerActionListener(OnReceiveServerAction);
        //InitData();

        sailLevel = 1;
        helmLevel = 1;

        sailLevelTxt = sailCanvas.transform.Find("Level").gameObject.GetComponent<Text>();
        helmLevelTxt = helmCanvas.transform.Find("Level").gameObject.GetComponent<Text>();

        sailUpgradeBtn = sailCanvas.transform.Find("UpgradeSail").gameObject;
        helmUpgradeBtn = helmCanvas.transform.Find("UpgradeHelm").gameObject;
        sailUpgradeBtnLabel = sailUpgradeBtn.transform.Find("SailBtnLabel").gameObject.GetComponent<Text>();
        helmUpgradeBtnLabel = helmUpgradeBtn.transform.Find("HelmBtnLabel").gameObject.GetComponent<Text>();

        Debug.Log(GlobalConfigs.UpgradeShipConfig);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Upgrade Sail
    public void upgradeSail()
    {
        if (sailLevel < MAX_SAIL_LEVEL)
        {
            sailLevel++;
            Sprite newSail = Resources.Load("UpgradeShip/sail_" + sailLevel, typeof(Sprite)) as Sprite;
            SpriteRenderer sailRenderer = sail.GetComponent<SpriteRenderer>();

            sailRenderer.sprite = newSail;
            sailLevelTxt.text = "Sail Level " + sailLevel;

            if (sailLevel == MAX_SAIL_LEVEL)
            {
                sailUpgradeBtnLabel.text = "MAX";
                sailUpgradeBtnLabel.rectTransform.localPosition = new Vector3(0, 20, 0);
                sailUpgradeBtnLabel.fontSize = 40;
                sailUpgradeBtn.transform.Find("UpgradePrice").gameObject.SetActive(false);
            }
        }
    }

    // Upgrade Helm
    public void upgradeHelm()
    {
        if (helmLevel < MAX_HELM_LEVEL)
        {
            helmLevel++;

            Sprite newHelm = Resources.Load("UpgradeShip/ship_1", typeof(Sprite)) as Sprite;
            SpriteRenderer helmRenderer = helm.GetComponent<SpriteRenderer>();

            if (helmLevel > 4) {
                newHelm = Resources.Load("UpgradeShip/ship_2", typeof(Sprite)) as Sprite;
            }
            if (helmLevel > 6)
            {
                //helm.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            helmRenderer.sprite = newHelm;
            helmLevelTxt.text = "Helm Level " + helmLevel;

            if (helmLevel == MAX_HELM_LEVEL)
            {
                helmUpgradeBtnLabel.text = "MAX";
                helmUpgradeBtnLabel.fontSize = 40;
                helmUpgradeBtnLabel.rectTransform.localPosition = new Vector3(0, 20, 0);
                helmUpgradeBtn.transform.Find("UpgradePrice").gameObject.SetActive(false);
            }
        }
    }
}
