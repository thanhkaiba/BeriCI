using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RenderAllConfig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RenderSailorConfig("ScriptableObject/Sailors/Meechic", "Meechic");
        RenderSailorConfig("ScriptableObject/Sailors/Helti", "Helti");
        RenderSailorConfig("ScriptableObject/Sailors/Tons", "Tons");
        RenderSailorConfig("ScriptableObject/Sailors/Herminia", "Herminia");
        RenderSailorConfig("ScriptableObject/Sailors/Obonbee", "Obonbee");
        RenderSailorConfig("ScriptableObject/Sailors/Scrub", "Scrub");
        RenderSailorConfig("ScriptableObject/Sailors/RowT", "RowT");
        RenderCombatConfig("ScriptableObject/Combat", "Combat");
        RenderClassBonusConfig("ScriptableObject/ClassBonus/ContainerClassBonus", "ContainerClassBonus");
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
}
