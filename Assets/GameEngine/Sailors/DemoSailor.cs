using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DemoSailor : Sailor
{
    public DemoSailor()
    {
        using (StreamReader r = new StreamReader("Assets/Config/Sailors/DemoSailor.json"))
        {
            string json = r.ReadToEnd();
            s = JsonConvert.DeserializeObject<ConfigStats>(json);
            Debug.Log("DemoSailor" + GetPower());
        }
    }
}
