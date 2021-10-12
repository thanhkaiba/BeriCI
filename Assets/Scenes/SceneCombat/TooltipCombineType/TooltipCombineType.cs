using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipCombineType : MonoBehaviour
{
    public TypeCombineInGameCanvas iconType; 
    public Text title;
    public Text content;
    public static TooltipCombineType Instance;
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
            if (posInImage.x >= -size.width/2 && posInImage.x < size.width/2 &&
                 posInImage.y >= -size.height/2 && posInImage.y < size.height/2)
            {
                // click inside
            }
            else gameObject.SetActive(false);
        }
    }
    public void ShowTooltipPassiveType(PassiveType data, Vector2 pos)
    {
        ContainerClassBonus config = GlobalConfigs.Instance.ClassBonus;

        gameObject.SetActive(true);

        // dong thanh ham limit width height
        float width = GetComponent<RectTransform>().rect.width;
        float height = GetComponent<RectTransform>().rect.height;
        if (pos.x < 180 + width / 2) pos.x = 180 + width / 2;
        if (pos.x > Screen.width - 180 - width / 2) pos.x = Screen.width - 180 - width / 2;
        if (pos.y < 20 + height / 2) pos.y = 20 + height / 2;
        if (pos.y > Screen.height - 20 - height / 2) pos.y = Screen.height - 20 - width / 2;
        transform.position = pos;

        int maxPop = config.GetMaxPopNeed(data.type);
        List<float> para = config.GetParams(data.type, data.level);
        iconType.SetData(data);
        title.text = "" + data.type + " (" + data.current + "/" + maxPop + ")";
        switch (data.type)
        {
            case SailorType.WILD:
                content.text = "WILD hồi " + (para[0] * 100) + "% máu khi tấn công kẻ địch";
                break;
            case SailorType.MIGHTY:
                content.text = "MIGHTY tăng " + (para[0] * 100) + "% máu tối đa";
                break;
            case SailorType.SWORD_MAN:
                content.text = "SWORD MAN tăng " + para[0] + " tốc độ";
                break;
            case SailorType.HORROR:
                content.text = "Tất cả kẻ địch bị giảm " + para[0] + " giáp";
                break;
            case SailorType.BERSERK:
                content.text = "BERSERK tăng " + para[0] + " tốc độ sau khi tấn công kẻ địch";
                break;
            case SailorType.DOCTOR:
                content.text = "DOCTOR tăng " + (para[0] * 100) + "% hiệu lực hồi máu";
                break;
            case SailorType.WIZARD:
                content.text = "WIZARD tăng " + (para[0] * 100) + "% sát thương kĩ năng";
                break;
            case SailorType.SUPPORT:
                content.text = "Tất cả đồng đội nhận " + para[0] + " fury khởi đầu";
                break;
            case SailorType.SNIPER:
                content.text = "Khi một sniper tấn công, các sniper còn lại bứt tốc " + (para[0] * 100) + "%";
                break;
            case SailorType.ASSASSIN:
                content.text = "ASSASSIN tăng " + (para[0] * 100) + "% sát thương. \n Chỉ áp dụng khi đội hình có duy nhất 1 ASSASSIN";
                break;
            case SailorType.KUNG_FU:
                content.text = "KUNG FU sẽ tấn công thêm " + para[0] + "lần";
                break;
        }

    }
}
