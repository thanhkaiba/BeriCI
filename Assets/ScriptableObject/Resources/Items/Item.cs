using UnityEngine;

public enum ItemPosition
{
    WEAPON,
    COAT,
    SHOES,
    ACCESSORY,
}
public enum ItemColor
{
    WHITE,
    GREEN,
    BLUE,
    PURPLE,
    ORANGE,
}

[CreateAssetMenu(fileName = "New Item", menuName = "config/Item")]
public class Item : ScriptableObjectPro
{
    [SerializeField]
    private string item_name = "";
    [SerializeField]
    private ItemColor color = ItemColor.WHITE;
    [SerializeField]
    private ItemPosition pos = ItemPosition.ACCESSORY;
    [SerializeField]
    private float power = 50;
    [SerializeField]
    private float health = 0;
    [SerializeField]
    private float speed = 0;
    [SerializeField]
    private float crit = 0;
    [SerializeField]
    private float armor = 0;
    [SerializeField]
    private float magic_resist = 0;
    [SerializeField]
    private int fury = 0;

    public SailorClass class_buff = SailorClass.CYBORG;

    public int quality = 0;
    public string Name
    {
        get { return item_name; }
    }
    public ItemColor Color
    {
        get { return color; }
    }
    public ItemPosition Position
    {
        get { return pos; }
    }
    public float Power
    {
        get { return power; }
    }
    public float Health
    {
        get { return health; }
    }
    public float Speed
    {
        get { return speed; }
    }
    public float Crit
    {
        get { return crit; }
    }
    public float Armor
    {
        get { return armor; }
    }
    public float MagicResist
    {
        get { return magic_resist; }
    }
    public int Fury
    {
        get { return fury; }
    }
}
