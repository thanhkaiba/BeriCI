using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    SHIPWRIGHT,
    SNIPER,
    ARCHER,
    SWORD_MAN,
    LOGISTICS,
    WIZARD,
    ASSASSIN,
    PET
};

public enum CharacterModel
{
    WARRIOR,
    GOBLIN_ARCHER,
};

public class Character
{
    public string name = "A Character";
    // do not change
    public CharacterType TYPE;

    public float POWER;
    public float HEALTH;
    public int SPEED;
    public float ARMOR;
    public float MAGIC_RESIST;

    public float POWER_P_LEVEL;
    public float HEALTH_P_LEVEL;
    public float ARMOR_P_LEVEL;
    public float MAGIC_RESIST_P_LEVEL;

    public int level = 1;
    public Skill skill;

    public CharacterModel model = CharacterModel.WARRIOR;
    public Character(CharacterType type)
    {
        this.TYPE = type;
    }
    // damage = damagedeal x 100/(100 + armor);
    public void SetRandomStats()
    {
        switch(TYPE)
        {
            case CharacterType.SHIPWRIGHT:
                POWER =                 RF(40f, 55f);
                POWER_P_LEVEL =         RF(4.0f, 5.5f);
                HEALTH =                RF(300, 400); // nhieu mau
                HEALTH_P_LEVEL =        RF(30, 40);
                SPEED =                 RI(95, 100);
                ARMOR =                 35; // nhieu giap
                ARMOR_P_LEVEL =         0.35f;
                MAGIC_RESIST =          20;
                MAGIC_RESIST_P_LEVEL =  0;
                model = CharacterModel.WARRIOR;
                break;
            case CharacterType.SNIPER:
                POWER = RF(120f, 160f); // damage to
                POWER_P_LEVEL = RF(12, 16);
                HEALTH = RF(140, 160);
                HEALTH_P_LEVEL = RF(14, 16);
                SPEED = 110; // ban cham
                ARMOR = 20;
                ARMOR_P_LEVEL = 0.5f;
                MAGIC_RESIST = 5;
                MAGIC_RESIST_P_LEVEL = 0.34f;
                model = CharacterModel.GOBLIN_ARCHER;
                break;
            case CharacterType.ARCHER:
                POWER = RF(32, 38); // damage it
                POWER_P_LEVEL = RF(3.2f, 3.8f);
                HEALTH = RF(140, 160);
                HEALTH_P_LEVEL = RF(14, 16);
                SPEED = RI(36, 42); // ban nhanh
                ARMOR = 15;
                ARMOR_P_LEVEL = 0.4f;
                MAGIC_RESIST = 0;
                MAGIC_RESIST_P_LEVEL = 0;
                model = CharacterModel.GOBLIN_ARCHER;
                break;
            case CharacterType.SWORD_MAN:
                POWER = RF(74, 88); 
                POWER_P_LEVEL = RF(7.4f, 8.8f);
                HEALTH = RF(200, 240);
                HEALTH_P_LEVEL = RF(20, 24);
                SPEED = RI(80, 90);
                ARMOR = 20;
                ARMOR_P_LEVEL = 0.25f;
                MAGIC_RESIST = 20;
                MAGIC_RESIST_P_LEVEL = 0;
                model = CharacterModel.WARRIOR;
                break;
            case CharacterType.LOGISTICS:
                POWER = RF(25, 30);
                POWER_P_LEVEL = RF(2.5f, 3.0f);
                HEALTH = RF(180, 220);
                HEALTH_P_LEVEL = RF(20, 26);
                SPEED = RI(68, 74);
                ARMOR = 10;
                ARMOR_P_LEVEL = 0.4f;
                MAGIC_RESIST = 50;
                MAGIC_RESIST_P_LEVEL = 0.5f;
                model = CharacterModel.WARRIOR;
                break;
            case CharacterType.WIZARD:
                POWER = RF(20, 25);
                POWER_P_LEVEL = RF(2.0f, 2.5f);
                HEALTH = RF(140, 160);
                HEALTH_P_LEVEL = RF(16, 20);
                SPEED = RI(92, 100);
                ARMOR = 10;
                ARMOR_P_LEVEL = 0.1f;
                MAGIC_RESIST = 30;
                MAGIC_RESIST_P_LEVEL = 0.3f;
                model = CharacterModel.GOBLIN_ARCHER;
                break;
            case CharacterType.ASSASSIN:
                POWER = RF(80, 96);
                POWER_P_LEVEL = RF(8.0f, 9.6f);
                HEALTH = RF(140, 160);
                HEALTH_P_LEVEL = RF(14, 16);
                SPEED = RI(108, 120);
                ARMOR = 15;
                ARMOR_P_LEVEL = 0.2f;
                MAGIC_RESIST = 10;
                MAGIC_RESIST_P_LEVEL = 0.4f;
                model = CharacterModel.WARRIOR;
                break;
            case CharacterType.PET:
                POWER = RF(1, 300);
                POWER_P_LEVEL = RF(0f, 20);
                HEALTH = RF(1, 1000);
                HEALTH_P_LEVEL = RF(1, 100);
                SPEED = RI(15, 200);
                ARMOR = RF(1, 100);
                ARMOR_P_LEVEL = RF(0.1f, 5f);
                MAGIC_RESIST = RF(1, 100);
                MAGIC_RESIST_P_LEVEL = RF(0.1f, 5f);
                model = CharacterModel.WARRIOR;
                break;
        }
    }
    public void SetSkill(Skill skill)
    {
        this.skill = skill;
    }
    public float GetPower()
    {
        return POWER + POWER_P_LEVEL * level;
    } 
    public float GetHealth()
    {
        return HEALTH + HEALTH_P_LEVEL * level;
    }
    public int GetSpeed()
    {
        return SPEED;
    }
    public float GetArmor()
    {
        return ARMOR + ARMOR_P_LEVEL * level;
    }
    public float GetMagicResist()
    {
        return MAGIC_RESIST + MAGIC_RESIST_P_LEVEL * level;
    }
    public float GetFury()
    {
        return skill != null ? skill.MAX_FURY : 0;
    }
    int RI(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
    float RF (float min, float max)
    {
        return Random.Range(min, max + 0.01f);
    }
}
