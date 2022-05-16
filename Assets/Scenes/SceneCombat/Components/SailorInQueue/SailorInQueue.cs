using UnityEngine;
using UnityEngine.UI;

public class SailorInQueue : MonoBehaviour
{
    private CombatSailor sailorData;
    public Image background;
    public SmoothSlider speedSlider;
    public Image icon;
    public Text level;
    public Text quality;
    public Image colorTeam;
    [SerializeField]
    private Sprite starSpr, starSprRed, starSprPurple;
    public Transform nodeStar;
    public Text textSpeed;
    public void SetData(CombatSailor s)
    {
        sailorData = s;
        icon.sprite = GameUtils.GetSailorAvt(s.Model.config_stats.root_name);
        speedSlider.speed = 2.4f;

        var model = sailorData.Model;
        ShowStar(model.star);
        if (sailorData.cs.team == Team.A)
        {
            icon.transform.localScale = new Vector3(-1, 1, 1);
            colorTeam.color = new Color(103f / 255f, 169f / 255f, 255f / 255f);
        }
        else
        {
            icon.transform.localScale = new Vector3(1, 1, 1);
            colorTeam.color = new Color(217f / 255f, 88f / 255f, 97f / 255f);
        }
        level.text = sailorData.Model.level.ToString();
        quality.text = "Q:" + sailorData.Model.quality.ToString();
    }
    public CombatSailor GetData()
    {
        return sailorData;
    }
    public void PresentData()
    {
        float newValue = (float)sailorData.cs.CurrentSpeed / sailorData.cs.SpeedNeed;
        if (speedSlider.value < newValue) speedSlider.ChangeValue(newValue);
        else speedSlider.SetValue(newValue);
        //textSpeed.text = sailorData.cs.CurrentSpeed + "/" + sailorData.cs.SpeedNeed;
    }
    public void OnClick()
    {
        TooltipSailorInfo.Instance.ShowTooltip(sailorData.gameObject);
    }
    private void ShowStar(int star)
    {
        if (star <= 5)
        {
            for (int i = 0; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSpr;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.2f, 0.2f, 1);
                starObj.transform.localPosition = new Vector3(-6.5f * i, -18 * i);
            }
        }
        else if (star <= 8)
        {
            for (int i = 5; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSprRed;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                starObj.transform.localPosition = new Vector3(-6.5f * 1.25f * (i-5) - 2, -18 * 1.25f * (i - 5) - 2);
            }
        }
        else
        {
            for (int i = 8; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSprPurple;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                starObj.transform.localPosition = new Vector3(-6.5f * 2f * (i - 8) - 8, -18 * 2f * (i - 8) - 8);
            }   
        }
    }
}
