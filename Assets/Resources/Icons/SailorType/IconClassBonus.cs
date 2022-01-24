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
    private void Start()
    {
        if (clickable)
        {
            Button b = gameObject.AddComponent<Button>() as Button;
            b.onClick.AddListener(() => OnClickIcon(SceneManager.GetActiveScene().name == "SceneCombat2D" ? b.transform.GetChild(1).transform : b.transform.GetChild(0).transform));
        }
    }
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
        if (levelNumber - level == 1) GetComponent<Image>().sprite = bg_2;
        else if (levelNumber - level == 2) GetComponent<Image>().sprite = bg_1;
        else GetComponent<Image>().sprite = bg_0;
    }
    public void SetData(ClassBonusItem data)
    {
        SynergiesConfig config = GlobalConfigs.Synergies;
        int levelNumber = config.GetLevelCount(data.type);
        int maxPop = config.GetMaxPopNeed(data.type);

        this.data = data;
        ChangeIcon(data.type);
        ChangeLevel(data.level, levelNumber);

        ChangeText("" + data.current + "/" + maxPop);
    }
    private void OnClickIcon(Transform pos)
    {
        if (data == null) return;
        if (TooltipClassBonus.Instance != null)
        {
            TooltipClassBonus.Instance.ShowTooltipPassiveType(data, pos);
        }
    }
}
