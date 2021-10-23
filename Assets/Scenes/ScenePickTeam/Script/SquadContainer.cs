using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadContainer : MonoBehaviour
{
    private TeamColor teamColor;
    [SerializeField] SquadSlot[] slots;

    void Start()
    {
        foreach (KeyValuePair<short, string> squadSlot in SquadData.Instance.Squad)
        {
            SquadSlot slot = slots[squadSlot.Key];
            string sailorId = squadSlot.Value;

            if (sailorId.Length > 0)
            {

                Sailor sailor = AddSailor(sailorId);
                slot.SetSelectedSailer(sailor);
            }
            else
            {
                slot.SetSelectedSailer(null);
            }

        }
        teamColor = FindObjectOfType<TeamColor>();

        OnUpdateSquad();
        GameEvent.SquadChange.AddListener(OnUpdateSquad);
        StartCoroutine(MyCoroutine());

    }

    public Sailor AddSubSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        DragableSubsailor drag = sailor.gameObject.AddComponent<DragableSubsailor>();
        sailor.transform.parent = transform;
        drag.slots = slots;

        return sailor;
    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        DragableSailor drag = sailor.gameObject.AddComponent<DragableSailor>();
        sailor.transform.parent = transform;
        drag.slots = slots;

        return sailor;
    }

    public void OnUpdateSquad()
    {
        teamColor.ShowClassBonus();
    }

    IEnumerator MyCoroutine()
    {
        print("Coroutine has started");
        yield return MyOtherCoroutine();
        print("Coroutine has ended");
    }

    IEnumerator MyOtherCoroutine()
    {

        int i = 3;

        while (i > 0)
        {
            // Do something 3 times
            i--;
            yield return new WaitForSeconds(1);
        }
        print("All Done!");
    }
}
