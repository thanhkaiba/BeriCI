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
    private Sprite spriteOpen;

    [SerializeField]
    private Sprite spriteLock;

    [SerializeField]
    private GameObject blackCircle;

    [SerializeField]
    private State state = State.AVAIABLE;

    private bool selectable;
    public bool Selectable
    {
        get
        {
            return selectable;
        }
        set
        {
            selectable = value;
            GetComponent<SpriteRenderer>().sprite = value ? spriteOpen : spriteLock;
        }
    }

    private void Start()
    {
        boxAround = GetComponent<SpriteRenderer>().bounds;
    }
    public void OnSelecting()
    {
        sailor = null;
        state = State.SELECTING;
        if (blackCircle != null)
        {
            blackCircle.SetActive(true);
            blackCircle.GetComponent<SpriteRenderer>().color = Color.magenta;
        }

    }

    public void OnFree()
    {
        sailor = null;
        state = State.AVAIABLE;
        if (blackCircle != null)
        {
            blackCircle.SetActive(false);
        }

    }

    public void SetSelectedSailer(Sailor s)
    {
        if (s != null)
        {
            state = State.SELECTED;
            sailor = s;
            sailor.transform.position = new Vector3(transform.position.x, transform.position.y, -4 - transform.localPosition.z / 4);
            if (blackCircle != null)
            {
                blackCircle.SetActive(true);
                blackCircle.GetComponent<SpriteRenderer>().color = Color.white;
            }

        }
        else
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
        return state;
    }
}
