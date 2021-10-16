using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquadSlot : MonoBehaviour
{
    public enum State
    {
        AVAIABLE, SELECTING, SELECTED
    }
    public Collider boxAround;
    private Sailor sailor;
    [SerializeField]
    private State state = State.AVAIABLE;

    private void Start()
    {
        boxAround = GetComponent<Collider>();
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
            sailor.transform.position = transform.position;
            GetComponent<SpriteRenderer>().color = Color.red;
        } else
        {
            this.OnFree();
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
