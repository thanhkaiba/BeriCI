using Piratera.GUI;
using System.Collections;
using UnityEngine;

public class MultiRoyalCollectingController : MonoBehaviour
{
    public RoyalCollectingAnimation.EXPANSION_MODE _expansionMode = RoyalCollectingAnimation.EXPANSION_MODE.Going_Up;
    public float emissionRate = 0.2f;
    public Transform[] itemDisplayer;
    [HideInInspector]
    public Transform popPosition;
    public GameObject[] itemPrefab;
    [HideInInspector]
    public static MultiRoyalCollectingController _instance;
    private GameObject layer;


    private void Awake()
    {
        _instance = this;

    }

    private void Start()
    {
        layer = GuiManager.Instance.GetLayer(LayerId.LOADING);
        popPosition = layer.transform;
    }

    public void CollectItem(int index, int quantity)
    {
        StartCoroutine(PopItems(index, quantity));
    }

    public void CollectItem(int index, int quantity, int value, System.Action OnComplete)
    {
        int valueExtend = 0;
        if (quantity != 0) valueExtend = value / quantity;
        StartCoroutine(PopItems(index, quantity, valueExtend, OnComplete));
    }



    // Collect some items with animation at a fixed position
    public void CollectItemAtPosition(int index, int quantity, Vector3 position)
    {
        // Set the position
        popPosition.position = position;
        StartCoroutine(PopItems(index, quantity));
    }

    // Here we pop all the necessary items
    public IEnumerator PopItems(int index, int quantity)
    {
        WaitForSeconds delay = new WaitForSeconds(emissionRate);
        for (int i = 0; i < quantity; i++)
        {
            RoyalCollectingAnimation animation = null;
            GameObject go = Instantiate(itemPrefab[index], layer.transform);
            animation = go.GetComponent<RoyalCollectingAnimation>();
            Destroy(go, 2.2f);//Remove go Object

            // Initialize object
            animation.Initialize(itemDisplayer[index], popPosition, Vector3.zero, Vector3.one, _expansionMode);
            // Start animation
            animation.StartAnimation();
            yield return delay;
        }
    }

    IEnumerator PopItems(int index, int quantity, int newValue, System.Action action)
    {
        WaitForSeconds delay = new WaitForSeconds(emissionRate);
        int number = quantity;
        for (int i = 0; i < quantity; i++)
        {
            RoyalCollectingAnimation animation = null;
            // No free object has been found in pool, so we instantiate a new one
            GameObject go = Instantiate(itemPrefab[index], layer.transform);
            go.transform.SetParent(layer.transform);
            animation = go.GetComponent<RoyalCollectingAnimation>();
            Destroy(go, 2.2f);//Remove go Object

            // Initialize object
            animation.Initialize(itemDisplayer[index], popPosition, Vector3.zero, Vector3.one, _expansionMode, newValue, () =>
            {
                number--;
                if (number <= 0)
                {
                    action();
                    action = null;
                }
            });
            // Start animation
            animation.StartAnimation();
            yield return delay;
        }
    }

}
