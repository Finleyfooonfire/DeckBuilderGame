using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPlayAreaGrid : MonoBehaviour
{
    [SerializeField] Vector2 gridSize = new Vector2(5, 2);
    [SerializeField] Vector2 slotSize = new Vector2(1f, 1f);
    [SerializeField] Vector2 slotSpacing = new Vector2(27, 50);
    public Dictionary<Vector3, bool> GridSlots { get; private set; } = new Dictionary<Vector3, bool>();
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
            -1,
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
                GridSlots.Add(slotPosition, false);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (cardPlayArea == null) return;
        Gizmos.color = Color.yellow;
        Vector3 startPosition = cardPlayArea.position - new Vector3(
            (gridSize.x - 1) * (slotSize.x + slotSpacing.x) * gridScale / 2,
            0,
            (gridSize.y - 1) * (slotSize.y + slotSpacing.y) * gridScale / 2);

        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                Vector3 slotPosition = startPosition + new Vector3(
                    col * (slotSize.x + slotSpacing.x) * gridScale,
                    0,
                    row * (slotSize.y + slotSpacing.y) * gridScale);
                Gizmos.DrawWireCube(slotPosition, new Vector3(slotSize.x, 0.1f, slotSize.y));
            }
        }
    }

    public Vector3 FindClosestSlot(Vector3 currentPosition)
    {
        KeyValuePair<Vector3, bool> closestSlot = GridSlots.First();
        float shortestDistance = Vector3.Distance(currentPosition, closestSlot.Key);
        foreach (KeyValuePair<Vector3, bool> slot in GridSlots)
        {
            if (slot.Value) continue;//Skip in use slots
            float distance = Vector3.Distance(currentPosition, slot.Key);
            if (distance < shortestDistance)
            {
                closestSlot = slot;
                shortestDistance = distance;
            }
        }
        return closestSlot.Key;

    }

    public void Remove(Vector3 slotToRemove)
    {
        GridSlots[slotToRemove] = true;
    }

    public void Free(Vector3 slotToFree)
    {
        GridSlots[slotToFree] = false;
    }
}
