using Piratera.GUI;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DragableSailor : MonoBehaviour
{
    protected const int TransformHeight = 200;
    protected BoxCollider boxAround;
    protected Canvas canvas;
    public SquadSlot[] slots { get; set; }
    [SerializeField]
    protected short selectingIndex = -1;
    protected Sailor sailor;
    [SerializeField]
    protected short originIndex = -1;
    protected Image dragImage;
    protected bool draging = false;

    private Vector3 delta;
    private float originZ;

    public Action<string> UnEquipSailor;
    public Func<short, bool> IsSlotEmpty;
    public Action<string, short> OccupieSailor;
    public Action<string, short> SwapSailor;
    public Func<string, bool> CanUnEquip = delegate { return true; };

    protected void Start()
    {
        boxAround = GetComponent<BoxCollider>();
        sailor = GetComponent<Sailor>();
        //canvas = FindObjectOfType<Canvas>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    protected void OnMouseDown()
    {
        if (!SquadContainer.Draging || !SquadAContainer.Draging)
        {
            dragImage = SubSailorIcon.CreateDragSailorImage(sailor.Model, canvas.transform);
            dragImage.enabled = false;
            SetSailorOpacity(0.8f);
            for (short i = 0; i < slots.Length; i++)
            {
                slots[i].Selectable = true;
                if (slots[i].GetOwner() == sailor)
                {
                    originIndex = i;
                }
            }
            selectingIndex = originIndex;
            slots[originIndex].OnSelecting();

            Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            originZ = transform.position.z;
            delta = transform.position - movePos;
            SquadContainer.Draging = true;
            SquadAContainer.Draging = true;
            draging = true;
        }
        else
        {
            draging = false;
        }


    }


    protected void OnMouseDrag()
    {

        if (draging)
        {
            Vector2 mousePosition = Input.mousePosition;
            if (mousePosition.y > (TransformHeight * canvas.transform.localScale.y))
            {
                dragImage.enabled = false;
                ShowChild(true);
                OnDragSailor(mousePosition);
            }
            else
            {

                UpdateSlots(originIndex);
                dragImage.enabled = true;
                ShowChild(false);
                dragImage.transform.position = mousePosition;
            }

        }

    }
    private void ShowChild(bool isShow)
    {
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
            transform.GetChild(i).gameObject.SetActive(isShow);
    }

    protected void OnDragSailor(Vector2 mousePosition)
    {
        Vector3 movePos = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = movePos + delta;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        CheckNewSelecting(transform.position);
    }

    protected void OnMouseUp()
    {

        if (draging)
        {
            SquadContainer.Draging = false;
            SquadAContainer.Draging = false;
            DefenseSquadContainer.Draging = false;
            draging = false;
            if (!dragImage.enabled)
            {
                SetSailorOpacity(1f);
                for (short i = 0; i < slots.Length; i++)
                {
                    if (slots[i].GetOwner() == sailor)
                    {
                        originIndex = i;
                    }
                }
                if (selectingIndex >= 0)
                {
                    OnMouseUpWithSlot();
                }
                else
                {
                    OnMouseUpEmpty();
                }
            }
            else
            {
                if (CanUnEquip(sailor.Model.id))
                {
                    OnUnEquip();
                } else {
                    ShowChild(true);
                    OnMouseUpEmpty();
                    GuiManager.Instance.ShowPopupNotification("You must select at least one fighter");
                }
            }
        }

        if (dragImage != null)
        {
            Destroy(dragImage.gameObject);
        }
    }

    private void OnUnEquip()
    {
        UpdateSlots(originIndex);
        slots[originIndex].OnFree();
        Destroy(gameObject);
        UnEquipSailor(sailor.Model.id);

    }

    protected void OnMouseUpWithSlot()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, originZ);

        if (IsSlotEmpty(selectingIndex))
        {
            OccupieSailor(sailor.Model.id, selectingIndex);

        }
        else
        {
            SwapSailor(sailor.Model.id, selectingIndex);

        }
        slots[selectingIndex].SetSelectedSailer(sailor);
        originIndex = selectingIndex;
        selectingIndex = -1;
        TutorialMgr.Instance.CheckRunTutStartUp_StepLineUpBack2Lobby();
    }

    protected void OnMouseUpEmpty()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, originZ);
        if (originIndex >= 0)
        {
            slots[originIndex].SetSelectedSailer(sailor);
        }
    }


    private void CheckNewSelecting(Vector3 mousePositon)
    {
        for (short i = 0; i < slots.Length; i++)
        {
            SquadSlot slot = slots[i];
            if (slot.boxAround.Contains(new Vector3(mousePositon.x, mousePositon.y, slot.transform.position.z)) && slot.Selectable)
            {
                if (i != selectingIndex)
                {
                    UpdateSlots(i);
                }

            }
        }
    }

    protected virtual void UpdateSlots(short newSelecting)
    {
        // neu dang chiem 1 vi tri nao do => tra gia tri cho vi tri do
        if (selectingIndex >= 0)
        {
            slots[selectingIndex].Swap(slots[originIndex]);
        }

        // doi quan tuong o vi tri moi sang vi tri ban dau cua tướng đang drag
        slots[originIndex].Swap(slots[newSelecting]);

        // set trang thai cho vi tri moi
        slots[newSelecting].OnSelecting();

        // luu lai vi tri moi
        selectingIndex = newSelecting;
    }

    protected void SetSailorOpacity(float opacity)
    {
        try
        {
            Spine.Skeleton skeleton = transform.GetComponentInChildren<SkeletonMecanim>().skeleton;
            Color color = skeleton.GetColor();
            skeleton.SetColor(new Color(color.r, color.g, color.b, opacity));
        }
        catch
        {

        }
    }
}
