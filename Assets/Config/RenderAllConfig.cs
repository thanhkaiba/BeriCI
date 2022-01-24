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
