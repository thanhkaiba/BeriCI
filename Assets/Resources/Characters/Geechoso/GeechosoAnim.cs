using UnityEngine;

public class GeechosoAnim : MonoBehaviour
{
    public Geechoso parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<Geechoso>();
    }

    public void StartEff()
    {
        parent.StartEff();
    }
}
