using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShip : MonoBehaviour
{
    [SerializeField] private GameObject sail, helm;
    private int sailLevel, helmLevel;
    private const int MAX_SAIL_LEVEL = 7;
    private const int MAX_HELM_LEVEL = 7;

    // Start is called before the first frame update
    void Start()
    {
        sailLevel = 1;
        helmLevel = 1;
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

    // Upgrade Helm
    public void upgradeHelm()
    {
        if (helmLevel < MAX_HELM_LEVEL)
        {
            helmLevel++;

            Sprite newHelm = Resources.Load("UpgradeShip/ship_1", typeof(Sprite)) as Sprite;
            SpriteRenderer helmRenderer = helm.GetComponent<SpriteRenderer>();

            if (helmLevel > 4) {
                newHelm = Resources.Load("UpgradeShip/ship_2", typeof(Sprite)) as Sprite;
            }
            if (helmLevel > 6)
            {
                //helm.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            helmRenderer.sprite = newHelm;
        }
    }
}
