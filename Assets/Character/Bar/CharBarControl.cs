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

    public void SetIconType(CharacterType type)
    {
        Sprite spr;
        switch (type)
        {
            case CharacterType.SHIPWRIGHT:
                spr = Resources.Load<Sprite>("IconCharacterType/Tanker");
                break;
            case CharacterType.SNIPER:
                spr = Resources.Load<Sprite>("IconCharacterType/Sniper");
                break;
            case CharacterType.ARCHER:
                spr = Resources.Load<Sprite>("IconCharacterType/Archer");
                break;
            case CharacterType.SWORD_MAN:
                spr = Resources.Load<Sprite>("IconCharacterType/SwordMan");
                break;
            case CharacterType.DOCTOR:
                spr = Resources.Load<Sprite>("IconCharacterType/Doctor");
                break;
            case CharacterType.ENTERTAINER:
                spr = Resources.Load<Sprite>("IconCharacterType/Entertainer");
                break;
            case CharacterType.WIZARD:
                spr = Resources.Load<Sprite>("IconCharacterType/Wizard");
                break;
            case CharacterType.ASSASSIN:
                spr = Resources.Load<Sprite>("IconCharacterType/Assassin");
                break;
            case CharacterType.PET:
                spr = Resources.Load<Sprite>("IconCharacterType/Pet");
                break;
            default:
                spr = Resources.Load<Sprite>("IconCharacterType/Archer");
                break;
        }
        iconType.sprite = spr;
    }
    public void SetHealthBar(int max, int min)
    {
        healthBar.value = (float)min / max;
        healthText.text = min.ToString();
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
