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
    public void ChangeLevel(int level)
    {
        // TODO sau phai check theo tung loai
        if (level >= 1) GetComponent<Image>().sprite = bg_2;
        else GetComponent<Image>().sprite = bg_0;
    }
    public void SetData(ClassBonusItem data)
    {
        this.data = data;
        ChangeIcon(data.type);
        ChangeLevel(data.level);

        ContainerClassBonus config = GlobalConfigs.ClassBonus;
        int maxPop = config.GetMaxPopNeed(data.type);

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
