using System.IO;
using UnityEngine;

public class RenderAllConfig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] files = System.IO.Directory.GetFiles("Assets/Config/Resources/ScriptableObject/Sailors");
        foreach (string file in files)
        {
            //Do work on the files here
            if (!file.Contains(".asset.meta") && file.Contains(".asset"))
            {
                string nameAsset = file.Split('\\')[1];
                string name = nameAsset.Split('.')[0];
                RenderSailorConfig("ScriptableObject/Sailors/" + name, name);
            }
        }
        RenderCombatConfig("ScriptableObject/Combat", "Combat");
        RenderSailorGereralConfig("ScriptableObject/SailorGeneralConfig", "SailorGeneralConfig");
        RenderClassBonusConfig("ScriptableObject/ClassBonus/ContainerClassBonus", "ContainerClassBonus");
        RenderStaminaConfig("ScriptableObject/Stamina/Stamina", "Stamina");
        RenderLineUpSlotConfig("ScriptableObject/LineUpSlot/LineUpSlot", "LineUpSlot");
        RenderPvEConfig("ScriptableObject/PvE/PvE", "PvE");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RenderSailorConfig(string src, string file_name)
    {
        Debug.Log("RenderConfig " + src + " " + file_name);
        SailorConfig a = Resources.Load<SailorConfig>(src);
        string json = a.Serialize(a);
        string filesrc = "Assets/Config/json/Sailors/" + file_name + ".json";
        File.WriteAllText(filesrc, "");
        StreamWriter writer = new StreamWriter(filesrc, true);
        writer.WriteLine(json);
        writer.Close();
    }

    void RenderCombatConfig(string src, string file_name)
    {
        Debug.Log("RenderConfig " + src + " " + file_name);
        CombatConfig a = Resources.Load<CombatConfig>(src);
        string json = a.Serialize(a);
        string filesrc = "Assets/Config/json/" + file_name + ".json";
        File.WriteAllText(filesrc, "");
        StreamWriter writer = new StreamWriter(filesrc, true);
        writer.WriteLine(json);
        writer.Close();
    }
    void RenderSailorGereralConfig(string src, string file_name)
    {
        Debug.Log("RenderConfig " + src + " " + file_name);
        SailorGeneralConfig a = Resources.Load<SailorGeneralConfig>(src);
        string json = a.Serialize(a);
        string filesrc = "Assets/Config/json/" + file_name + ".json";
        File.WriteAllText(filesrc, "");
        StreamWriter writer = new StreamWriter(filesrc, true);
        writer.WriteLine(json);
        writer.Close();
    }
    void RenderClassBonusConfig(string src, string file_name)
    {
        Debug.Log("RenderConfig " + src + " " + file_name);
        ContainerClassBonus a = Resources.Load<ContainerClassBonus>(src);
        string json = a.Serialize(a);
        string filesrc = "Assets/Config/json/" + file_name + ".json";
        File.WriteAllText(filesrc, "");
        StreamWriter writer = new StreamWriter(filesrc, true);
        writer.WriteLine(json);
        writer.Close();
    }
    void RenderStaminaConfig(string src, string file_name)
    {
        Debug.Log("RenderConfig " + src + " " + file_name);
        UserStaminaConfig a = Resources.Load<UserStaminaConfig>(src);
        string json = a.Serialize(a);
        string filesrc = "Assets/Config/json/" + file_name + ".json";
        File.WriteAllText(filesrc, "");
        StreamWriter writer = new StreamWriter(filesrc, true);
        writer.WriteLine(json);
        writer.Close();
    }
    void RenderLineUpSlotConfig(string src, string file_name)
    {
        Debug.Log("RenderConfig " + src + " " + file_name);
        LineUpSlot a = Resources.Load<LineUpSlot>(src);
        string json = a.Serialize(a);
        string filesrc = "Assets/Config/json/" + file_name + ".json";
        File.WriteAllText(filesrc, "");
        StreamWriter writer = new StreamWriter(filesrc, true);
        writer.WriteLine(json);
        writer.Close();
    }
    void RenderPvEConfig(string src, string file_name)
    {
        Debug.Log("RenderConfig " + src + " " + file_name);
        PvEConfig a = Resources.Load<PvEConfig>(src);
        string json = a.Serialize(a);
        string filesrc = "Assets/Config/json/" + file_name + ".json";
        File.WriteAllText(filesrc, "");
        StreamWriter writer = new StreamWriter(filesrc, true);
        writer.WriteLine(json);
        writer.Close();
    }
}
