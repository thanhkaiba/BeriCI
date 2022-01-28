using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SailorDesc", menuName = "config/SailorDesc")]
public class SailorDesc : ScriptableObject
{
    public List<Param> list = new List<Param>();

    [System.Serializable]
    public class Param
    {
        public string root_name;
        public string present_name;
        public string title;
        public string skill_description;
    }
}

