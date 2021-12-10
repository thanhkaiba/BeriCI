using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SojephAnim : MonoBehaviour
{
    public Sojeph parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<Sojeph>();
    }

    public void StartEffHealth()
    {
        parent.StartEffHealth();
    }
    public void StartEffSkill()
    {
        parent.StartEffDame();
    }
}
