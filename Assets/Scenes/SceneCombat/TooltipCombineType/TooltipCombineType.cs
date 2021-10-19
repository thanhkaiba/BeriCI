﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipCombineType : MonoBehaviour
{
    public IconClassBonus iconType; 
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
    public void ShowTooltipPassiveType(ClassBonusItem data, Vector2 pos)
    {
        ContainerClassBonus config = GlobalConfigs.Instance.ClassBonus;

        gameObject.SetActive(true);

        // dong thanh ham limit width height
        float width = GetComponent<RectTransform>().rect.width;
        float height = GetComponent<RectTransform>().rect.height;
        if (pos.x < 180 + width / 2) pos.x = 180 + width / 2;
        if (pos.x > Screen.width - 180 - width / 2) pos.x = Screen.width - 180 - width / 2;
        if (pos.y < 20 + height / 2) pos.y = 20 + height / 2;
        if (pos.y > Screen.height - 20 - height / 2) pos.y = Screen.height - 20 - height / 2;
        transform.position = pos;

        int maxPop = config.GetMaxPopNeed(data.type);
        List<float> para = config.GetParams(data.type, data.level);
        iconType.SetData(data);
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
            case SailorClass.HORROR:
                content.text = "Tất cả kẻ địch bị giảm " + para[0] + " giáp";
                break;
            case SailorClass.BERSERK:
                content.text = "BERSERK tăng " + para[0] + " tốc độ sau khi tấn công kẻ địch";
                break;
            case SailorClass.DOCTOR:
                content.text = "DOCTOR tăng " + (para[0] * 100) + "% hiệu lực hồi máu";
                break;
            case SailorClass.WIZARD:
                content.text = "WIZARD tăng " + (para[0] * 100) + "% sát thương kĩ năng";
                break;
            case SailorClass.SUPPORT:
                content.text = "Tất cả đồng đội nhận " + para[0] + " fury khởi đầu";
                break;
            case SailorClass.SNIPER:
                content.text = "Khi một sniper tấn công, các sniper còn lại bứt tốc " + (para[0] * 100) + "%";
                break;
            case SailorClass.ASSASSIN:
                content.text = "ASSASSIN tăng " + (para[0] * 100) + "% sát thương. \n Chỉ áp dụng khi đội hình có duy nhất 1 ASSASSIN";
                break;
            case SailorClass.KUNG_FU:
                content.text = "KUNG FU sẽ tấn công thêm " + para[0] + "lần";
                break;
        }

    }
}
