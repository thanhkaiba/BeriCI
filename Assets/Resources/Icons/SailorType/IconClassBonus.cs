using Piratera.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IconClassBonus : MonoBehaviour
{
    public Image icon;
    public Text text;
    public ClassBonusItem data;
    public bool clickable = true;

    [SerializeField]
    private Sprite bg_0;
    [SerializeField]
    private Sprite bg_1;
    [SerializeField]
    private Sprite bg_2;
    [SerializeField]
    private Sprite bg_none;
    public void ChangeIcon(SailorClass type)
    {
        icon.sprite = Resources.Load<Sprite>("Icons/SailorType/" + type);
    }
    public void ChangeText(string _text)
    {
        text.text = _text;
    }
    public void ChangeLevel(int level, int levelNumber)
    {
        // TODO sau phai check theo tung loai
        if (level == -1)
        {
            GetComponent<Image>().sprite = bg_none;
            icon.color = new Color(160 / 255f, 160 / 255f, 160 / 255f);
            foreach (Outline o in GetComponentsInChildren<Outline>()) o.enabled = false;
            text.color = new Color(120 / 255f, 120 / 255f, 120 / 255f);
        }
        else if (levelNumber - level == 1) GetComponent<Image>().sprite = bg_2;
        else if (levelNumber - level == 2) GetComponent<Image>().sprite = bg_1;
        else GetComponent<Image>().sprite = bg_0;
    }
    public void SetData(ClassBonusItem data)
    {
        SynergiesConfig config = GlobalConfigs.Synergies;
        int levelNumber = config.GetLevelCount(data.type);
        int maxPop = config.GetNextLevelPopNeed(data.type, data.level);

        this.data = data;
        ChangeIcon(data.type);
        ChangeLevel(data.level, levelNumber);
        ChangeText("" + data.current + "/" + maxPop);

        if (clickable && data.level != -1)
        {
            var btn = gameObject.GetComponent<Button>();
            if (btn == null) btn = gameObject.AddComponent<Button>() as Button;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickIcon(SceneManager.GetActiveScene().name == "SceneCombat2D" ? btn.transform.GetChild(1).transform : btn.transform.GetChild(0).transform));
        }
    }
    private void OnClickIcon(Transform pos)
    {
        if (data == null) return;
        if (TooltipClassBonus.Instance != null && data.level != -1)
        {
            TooltipClassBonus.Instance.ShowTooltipPassiveType(data, pos);
        }
    }
}
