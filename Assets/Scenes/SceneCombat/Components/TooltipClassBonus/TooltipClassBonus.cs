using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TooltipClassBonus : MonoBehaviour
{
    // public IconClassBonus iconType; 
    public Text title;
    public Text content;
    public static TooltipClassBonus Instance;
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Rect size = GetComponent<RectTransform>().rect;
            Vector2 posInImage = transform.position - Input.mousePosition;
            if (posInImage.x >= -size.width / 2 && posInImage.x < size.width / 2 &&
                 posInImage.y >= -size.height / 2 && posInImage.y < size.height / 2)
            {
                // click inside
            }
            else gameObject.SetActive(false);
        }
    }
    public void ShowTooltipPassiveType(ClassBonusItem data, Transform _pos)
    {
        ContainerClassBonus config = GlobalConfigs.ClassBonus;
        gameObject.SetActive(true);
        gameObject.transform.position = _pos.position;
        gameObject.transform.localScale = SceneManager.GetActiveScene().name == "SceneCombat2D" ? new Vector3(.6f, .6f, .6f) : Vector3.one;
        int maxPop = config.GetMaxPopNeed(data.type);
        List<float> para = config.GetParams(data.type, data.level);
        // iconType.SetData(data);
        title.text = "" + data.type + " (" + data.current + "/" + maxPop + ")";
        switch (data.type)
        {
            case SailorClass.WILD:
                content.text = "WILD hồi " + (para[0] * 100) + "% máu khi tấn công kẻ địch";
                break;
            case SailorClass.MIGHTY:
                content.text = "MIGHTY tăng " + (para[0] * 100) + "% máu tối đa";
                break;
            case SailorClass.SWORD_MAN:
                content.text = "SWORD MAN tăng " + para[0] + " tốc độ";
                break;
            case SailorClass.DEMON:
                content.text = "Tất cả kẻ địch bị giảm " + para[0] + " giáp";
                break;
            case SailorClass.CYBORG:
                content.text = "CYBORG tăng " + para[0] + " giáp";
                break;
            case SailorClass.SEA_CREATURE:
                content.text = "Đồng đội nhận " + (para[0] * 100) + "kháng phép";
                break;
            case SailorClass.MAGE:
                content.text = "MAGE tăng " + (para[0] * 100) + "% Power";
                break;
            case SailorClass.SUPPORT:
                content.text = "Tất cả đồng đội nhận " + para[0] + " fury khởi đầu";
                break;
            case SailorClass.MARKSMAN:
                content.text = "Marksman gây thêm sát thương kẻ địch dưới " + (para[0] * 100) + "% máu.";
                break;
            case SailorClass.ASSASSIN:
                content.text = "ASSASSIN tăng " + (para[0] * 100) + "% sát thương. \n Chỉ áp dụng khi đội hình có duy nhất 1 ASSASSIN";
                break;
        }

    }
}
