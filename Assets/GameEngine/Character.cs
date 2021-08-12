using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    SHIPWRIGHT,
    SNIPER,
    ARCHER,
    SWORD_MAN,
    DOCTOR,
    ENTERTAINER,
    WIZARD,
    ASSASSIN,
    PET
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
                POWER =                 RF(30f, 50f);
                POWER_P_LEVEL =         RF(2.5f, 4.0f);
                HEALTH =                RF(520, 680); // nhieu mau
                HEALTH_P_LEVEL =        RF(45, 60);
                SPEED =                 RI(95, 105);
                ARMOR =                 RF(20, 25); // nhieu giap
                ARMOR_P_LEVEL =         RF(1.5f, 2);
                MAGIC_RESIST =          RF(20, 25);
                MAGIC_RESIST_P_LEVEL =  RF(0.5f, 1);
                break;
            case CharacterType.SNIPER:
                POWER = RF(120f, 160f); // damage to
                POWER_P_LEVEL = RF(12f, 16f);
                HEALTH = RF(180, 240);
                HEALTH_P_LEVEL = RF(20, 25);
                SPEED = RI(135, 165); // ban cham
                ARMOR = RF(10, 12);
                ARMOR_P_LEVEL = RF(0.2f, 0.4f);
                MAGIC_RESIST = RF(35, 40);
                MAGIC_RESIST_P_LEVEL = RF(0.3f, 0.5f); // nhieu khang phep
                break;
            case CharacterType.ARCHER:
                POWER = RF(18, 20); // damage it
                POWER_P_LEVEL = RF(3.5f, 4.0f);
                HEALTH = RF(180, 240);
                HEALTH_P_LEVEL = RF(16, 20);
                SPEED = RI(24, 36); // ban nhanh
                ARMOR = RF(10, 14);
                ARMOR_P_LEVEL = RF(0.2f, 0.4f);
                MAGIC_RESIST = RF(10, 14);
                MAGIC_RESIST_P_LEVEL = RF(0.1f, 0.15f);
                break;
            case CharacterType.SWORD_MAN:
                POWER = RF(70, 80); 
                POWER_P_LEVEL = RF(6f, 10f);
                HEALTH = RF(400, 550);
                HEALTH_P_LEVEL = RF(28, 32);
                SPEED = RI(95, 105);
                ARMOR = RF(10, 12);
                ARMOR_P_LEVEL = RF(0.2f, 0.4f);
                MAGIC_RESIST = RF(10, 14);
                MAGIC_RESIST_P_LEVEL = RF(0.1f, 0.15f);
                break;
            case CharacterType.DOCTOR:
                POWER = RF(25, 30);
                POWER_P_LEVEL = RF(3f, 5f);
                HEALTH = RF(180, 240);
                HEALTH_P_LEVEL = RF(25, 30);
                SPEED = RI(95, 105);
                ARMOR = RF(10, 14);
                ARMOR_P_LEVEL = RF(0.2f, 0.4f);
                MAGIC_RESIST = RF(10, 14);
                MAGIC_RESIST_P_LEVEL = RF(0.1f, 0.15f);
                break;
            case CharacterType.ENTERTAINER:
                POWER = RF(25, 30);
                POWER_P_LEVEL = RF(3f, 8f);
                HEALTH = RF(180, 240);
                HEALTH_P_LEVEL = RF(22, 34);
                SPEED = RI(58, 68);
                ARMOR = RF(10, 14);
                ARMOR_P_LEVEL = RF(0.2f, 0.4f);
                MAGIC_RESIST = RF(10, 14);
                MAGIC_RESIST_P_LEVEL = RF(0.1f, 0.15f);
                break;
            case CharacterType.WIZARD:
                POWER = RF(20, 25);
                POWER_P_LEVEL = RF(2f, 4f);
                HEALTH = RF(300, 350);
                HEALTH_P_LEVEL = RF(30, 32);
                SPEED = RI(95, 105);
                ARMOR = RF(10, 14);
                ARMOR_P_LEVEL = RF(0.2f, 0.4f);
                MAGIC_RESIST = RF(50, 60);
                MAGIC_RESIST_P_LEVEL = RF(0.3f, 0.8f); // nhieu khang phep
                break;
            case CharacterType.ASSASSIN:
                POWER = RF(80, 100);
                POWER_P_LEVEL = RF(10f, 12f);
                HEALTH = RF(250, 300);
                HEALTH_P_LEVEL = RF(25, 30);
                SPEED = RI(115, 125);
                ARMOR = RF(10, 14);
                ARMOR_P_LEVEL = RF(0.2f, 0.4f);
                MAGIC_RESIST = RF(35, 40);
                MAGIC_RESIST_P_LEVEL = RF(0.3f, 0.5f); // nhieu khang phep
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
