using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShip : MonoBehaviour
{
    [SerializeField] private GameObject sail, helm;
    private int sailLevel;
    private const int MAX_SAIL_LEVEL = 7;

    // Start is called before the first frame update
    void Start()
    {
        sailLevel = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Upgrade Sail
    public void upgradeSail()
    {
        if (sailLevel < MAX_SAIL_LEVEL)
        {
            sailLevel++;
            Sprite newSail = Resources.Load("UpgradeShip/sail_" + sailLevel, typeof(Sprite)) as Sprite;
            SpriteRenderer sailRenderer = sail.GetComponent<SpriteRenderer>();
            sailRenderer.sprite = newSail;
        }
    }
}
