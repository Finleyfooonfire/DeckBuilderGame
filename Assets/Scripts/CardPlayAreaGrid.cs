using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPlayAreaGrid : MonoBehaviour
{
    public struct CardPlayAreaSlot
    {
        public CardPlayAreaSlot(Vector3 slotPosition, bool isPlayerSlot, bool hasCard, bool hasSpellCard)
        {
            SlotPosition = slotPosition;
            IsPlayerSlot = isPlayerSlot;
            HasCard = hasCard;
            HasSpellCard = hasSpellCard;
        }

        public Vector3 SlotPosition { get; }
        public bool IsPlayerSlot { get; }
        public bool HasCard { get; set; }
        public bool HasSpellCard { get; set; }


        public override string ToString()
        {
            return $"{SlotPosition.ToString()} {IsPlayerSlot} {HasCard} {HasSpellCard}";
        }
    }


    [SerializeField] Vector2 gridSize = new Vector2(5, 2);
    [SerializeField] Vector2 slotSize = new Vector2(0.635f, 0.889f);
    [SerializeField] Vector2 slotSpacing = new Vector2(27, 50);
    public List<CardPlayAreaSlot> GridSlots { get; private set; } = new List<CardPlayAreaSlot>();
    [SerializeField] float gridScale = 0.025f;
    // public float gridHeightOffset = 0.33f;

    Transform cardPlayArea;

    void Start()
    {
        GameObject playAreaObject = GameObject.Find("CardPlayArea");
        if (playAreaObject != null)
        {
            cardPlayArea = playAreaObject.transform;
        }
        else
        {
            Debug.LogError("CardPlayArea GameObject not found in the scene.");
        }

        InitializeGrid();
    }

    void InitializeGrid()
    {
        if (cardPlayArea == null) return;

        Vector3 startPosition = cardPlayArea.position - new Vector3(
            (gridSize.x - 1) * (slotSize.x + slotSpacing.x) * gridScale / 2,
            -.1f,
            (gridSize.y - 1) * (slotSize.y + slotSpacing.y) * gridScale / 2
        );


        //startPosition.y += gridHeightOffset;

        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                Vector3 slotPosition = startPosition + new Vector3(
                    col * (slotSize.x + slotSpacing.x) * gridScale,
                    0,
                    row * (slotSize.y + slotSpacing.y) * gridScale);
                GridSlots.Add(new CardPlayAreaSlot(slotPosition, row == 0, false, false));
            }
        }
    }

    void OnDrawGizmos()
    {
        if (cardPlayArea == null) return;

        foreach (CardPlayAreaSlot slot in GridSlots)
        {
            Vector3 slotPosition = slot.SlotPosition;
            if (slot.HasCard)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            //Debug.Log(slotPosition);
            Gizmos.DrawWireCube(transform.TransformPoint(slotPosition), new Vector3(slotSize.x, 0.1f, slotSize.y));

        }
    }

    public Vector3 FindClosestSlot(Vector3 currentPosition, bool isPlayerCard, bool findUsedSlots = false)
    {
        CardPlayAreaSlot closestSlot = GridSlots.First();
        float shortestDistance = Vector3.Distance(currentPosition, closestSlot.SlotPosition);
        foreach (CardPlayAreaSlot slot in GridSlots)
        {
            if (isPlayerCard != (slot.IsPlayerSlot || (slot.HasCard ^ findUsedSlots))) continue;//Skip in use slots (unless findUsedSlots is set to true)
            float distance = Vector3.Distance(currentPosition, slot.SlotPosition);
            if (distance < shortestDistance)
            {
                closestSlot = slot;
                shortestDistance = distance;
            }
        }
        return closestSlot.SlotPosition;

    }

    //sets the slot's HasCard field to true
    public void FillSlot(Vector3 slotToRemove, bool isPlayerSlot)
    {
        CardPlayAreaSlot slot = GridSlots.Find(x => x.SlotPosition == slotToRemove && x.IsPlayerSlot == isPlayerSlot);
        slot.HasCard = true;
    }

    //sets the slot's HasCard field to false
    public void FreeSlot(Vector3 slotToFree, bool isPlayerSlot)
    {
        CardPlayAreaSlot slot = GridSlots.Find(x => x.SlotPosition == slotToFree && x.IsPlayerSlot == isPlayerSlot);
        slot.HasCard = false;
    }

    //sets the slot's HasSpellCard field to true
    public void FillSpellSlot(Vector3 slotToRemove, bool isPlayerSlot)
    {
        CardPlayAreaSlot slot = GridSlots.Find(x => x.SlotPosition == slotToRemove && x.IsPlayerSlot == isPlayerSlot);
        slot.HasSpellCard = false;
    }

    //sets the slot's HasSpellCard field to false
    public void FreeSpellSlot(Vector3 slotToFree, bool isPlayerSlot)
    {
        CardPlayAreaSlot slot = GridSlots.Find(x => x.SlotPosition == slotToFree && x.IsPlayerSlot == isPlayerSlot);
        slot.HasSpellCard = false;
    }
}
