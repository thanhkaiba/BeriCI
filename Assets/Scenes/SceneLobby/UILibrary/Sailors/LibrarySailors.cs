using Piratera.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibrarySailors : MonoBehaviour
{
    private int COL_NUM = 4;
    [SerializeField]
    private Transform tableContent;
    [SerializeField]
    private GameObject sailorRow;

    [SerializeField]
    private Image sailorImage, rankImage;
    [SerializeField]
    private Text sailorName, title, basePower, baseHealth, baseSpeed, baseAR, baseMR, mana;
    [SerializeField]
    private IconClassInAvt icon_0, icon_1;
    [SerializeField]
    private SailorDesc sailorDesc;

    private string[] listSailors = {
        "Helti",
        "Meechik",
        "Sojeph",
        "Beel",
        "Tons",
        "Herminia",
        "Scrub",
        "RowT",
        "OBonbee",
        "Jenkins",
        "Salvatafo",
        "Scott",
        "Alex",
        "QChi",
        "Galdalf",
        "Tad",
        "LiuHi",
        "Mealodo",
        "Geechoso",
        "Anglersei",
        "Yuchik",
        "Kamijita",
        "FatBrakes",
        "Daedra",
        "Lade",
        "Camelia",
        "Zeke",
        "Wuone",
        "BeiBei",
        "Mun",
        "Macay",
        "Nael",
    };
    private void Start()
    {
        System.Array.Sort(listSailors);
        ShowListSailor();
        SelectSailor(listSailors[0]);
    }
    private void ShowListSailor()
    {
        int avtCount = listSailors.Length;
        int rowNum = (int)Mathf.Ceil(avtCount / (float)COL_NUM);
        for (int i = 0; i < rowNum; i++)
        {
            var go = Instantiate(sailorRow, tableContent);
            for (int j = 0; j < COL_NUM; j++)
            {
                int idx = i * COL_NUM + j;
                var img = go.transform.Find("sailor_" + j);
                if (idx >= avtCount) img.gameObject.SetActive(false);
                else
                {
                    var config = GlobalConfigs.GetSailorConfig(listSailors[idx]);
                    switch(config.rank)
                    {
                        case SailorRank.B:
                            img.GetComponent<Image>().color = new Color32(200, 200, 200, 255);
                            break;
                        case SailorRank.A:
                            img.GetComponent<Image>().color = new Color32(138, 200, 138, 255);
                            break;
                        case SailorRank.S:
                            img.GetComponent<Image>().color = new Color32(33, 59, 255, 255);
                            break;
                        case SailorRank.SR:
                            img.GetComponent<Image>().color = new Color32(253, 39, 224, 255);
                            break;
                        case SailorRank.SSR:
                            img.GetComponent<Image>().color = new Color32(255, 151, 23, 255);
                            break;
                    }
                    img.Find("avt").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/IconSailor/" + listSailors[idx]);
                    var btn = img.gameObject.AddComponent<Button>();
                    btn.onClick.AddListener(() => SelectSailor(listSailors[idx]));
                }
            }
        }
    }
    private void SelectSailor(string sailor)
    {
        var config = GlobalConfigs.GetSailorConfig(sailor);
        sailorImage.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailor);
        rankImage.sprite = Resources.Load<Sprite>("Icons/IconRank/" + config.rank.ToString());
        sailorName.text = sailor;
        foreach (var item in sailorDesc.list)
            if (sailor == item.root_name)
                title.text = item.title;
        basePower.text = "" + config.power_base;
        baseHealth.text = "" + config.health_base;
        baseSpeed.text = "" + config.speed_base;
        baseAR.text = "" + config.armor;
        baseMR.text = "" + config.magic_resist;
        if (config.max_fury > 0)
            mana.text = "Mana (init/cost): " + config.start_fury + "/" + config.max_fury;
        else
            mana.text = "No mana";

        if (config.classes.Count > 0)
        {
            icon_0.gameObject.SetActive(true);
            icon_0.SetClass(config.classes[0]);
        }
        else icon_0.gameObject.SetActive(false);
        if (config.classes.Count > 1)
        {
            icon_1.gameObject.SetActive(true);
            icon_1.SetClass(config.classes[1]);
        }
        else icon_1.gameObject.SetActive(false);
    }
    public void Close()
    {
        Destroy(gameObject);
    }
}
