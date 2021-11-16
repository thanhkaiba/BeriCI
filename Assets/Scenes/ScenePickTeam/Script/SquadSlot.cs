using UnityEngine;

public class SquadSlot : MonoBehaviour
{
    public enum State
    {
        AVAIABLE, SELECTING, SELECTED
    }
    public Bounds boxAround;
    private Sailor sailor;
    [SerializeField]
    private State state = State.AVAIABLE;
    public bool selectable { get; set; }

    private void Start()
    {
        boxAround = GetComponent<SpriteRenderer>().bounds;
    }
    public void OnSelecting()
    {
        sailor = null;
        state = State.SELECTING;
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    public void OnFree()
    {
        sailor = null;
        state = State.AVAIABLE;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void SetSelectedSailer(Sailor s)
    {
        if (s != null)
        {
            state = State.SELECTED;
            sailor = s;
            Debug.Log(sailor.transform.position.z);
            Debug.Log(transform.localPosition.z);
            Debug.Log(sailor.transform.position.z + transform.localPosition.z);
            sailor.transform.position = new Vector3(transform.position.x, transform.position.y, -4 - transform.localPosition.z / 4);
            GetComponent<SpriteRenderer>().color = Color.red;
        } else
        {
            OnFree();
        }
       
    }

 

    public void Swap(SquadSlot slot)
    {
        Sailor s = GetOwner();
        SetSelectedSailer(slot.GetOwner());
        slot.SetSelectedSailer(s);
    }

    public Sailor GetOwner()
    {
        return sailor;
    }

    public State GetState()
    {
        return this.state;
    }
}
