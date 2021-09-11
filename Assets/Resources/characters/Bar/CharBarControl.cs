using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharBarControl : MonoBehaviour
{
    public Slider healthBar;
    public Text healthText;
    public Slider speedBar;
    public Text speedText;
    public Slider furyBar;
    public Text furyText;
    public Image iconType;
    public Image iconSkill;
    public Text textName;

    public void SetName (string name)
    {
        textName.text = name;
    }
    public void SetIconType(AttackType type)
    {
        Sprite spr;
        switch (type)
        {
            case AttackType.MELEE:
                spr = Resources.Load<Sprite>("IconCharacterType/SwordMan");
                break;
            case AttackType.RANGE:
                spr = Resources.Load<Sprite>("IconCharacterType/Archer");
                break;
            case AttackType.ASSASSINATE:
                spr = Resources.Load<Sprite>("IconCharacterType/Assassin");
                break;
            default:
                spr = Resources.Load<Sprite>("IconCharacterType/Archer");
                break;
        }
        iconType.sprite = spr;
    }
    public void SetIconSkill(Skill skill)
    {
        if (skill != null)
        {
            Debug.Log("skill.name" + skill.name);
            iconSkill.sprite = Resources.Load<Sprite>("IconSkill/" + skill.name);
        }
        else iconSkill.sprite = Resources.Load<Sprite>("IconSkill/None");
    }
    public void SetHealthBar(float max, float min)
    {
        healthBar.value = min / max;
        healthText.text = (System.Math.Ceiling(min)).ToString();
    }
    public void SetSpeedBar(int max, int min)
    {
        speedBar.value = (float)min / max;
        speedText.text = min + "/" + max;
    }
    public void SetFuryBar(int max, int min)
    {
        furyBar.gameObject.SetActive(max != 0);
        if (max != 0)
        {
            furyBar.value = (float)min / max;
            furyText.text = min + "/" + max;
        }
    }
}
