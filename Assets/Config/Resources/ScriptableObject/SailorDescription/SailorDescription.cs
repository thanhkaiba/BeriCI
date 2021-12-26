using System.Collections.Generic;
using UnityEngine;

public class SailorDescription : ScriptableObject
{
    public List<Sheet> sheets = new List<Sheet>();

    [System.SerializableAttribute]
    public class Sheet
    {
        public string name = string.Empty;
        public List<Param> list = new List<Param>();
    }

    [System.SerializableAttribute]
    public class Param
    {

        public string root_name;
        public string present_name;
        public string title;
        public string skill_description;
    }
}

