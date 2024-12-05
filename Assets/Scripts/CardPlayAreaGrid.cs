using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPlayAreaGrid : MonoBehaviour
{
    [SerializeField] Vector2 gridSize = new Vector2(5, 2);
    [SerializeField] Vector2 slotSize = new Vector2(0.635f, 0.889f);
    [SerializeField] Vector2 slotSpacing = new Vector2(27, 50);
    public Dictionary<KeyValuePair<Vector3, bool>, bool> GridSlots { get; private set; } = new Dictionary<KeyValuePair<Vector3, bool>, bool>();
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
                GridSlots.Add(new KeyValuePair<Vector3, bool>(slotPosition, row == 0), false);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (cardPlayArea == null) return;
        
        foreach (KeyValuePair<KeyValuePair<Vector3, bool>, bool> slot in GridSlots)
        {
            Vector3 slotPosition = slot.Key.Key;
            if (slot.Value)
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
        KeyValuePair<KeyValuePair<Vector3, bool>, bool> closestSlot = GridSlots.First();
        float shortestDistance = Vector3.Distance(currentPosition, closestSlot.Key.Key);
        foreach (KeyValuePair<KeyValuePair<Vector3, bool>, bool> slot in GridSlots)
        {
            if (isPlayerCard != slot.Key.Value || (slot.Value && !findUsedSlots)) continue;//Skip in use slots (unless findUsedSlots is set to true)
            float distance = Vector3.Distance(currentPosition, slot.Key.Key);
            if (distance < shortestDistance)
            {
                closestSlot = slot;
                shortestDistance = distance;
            }
        }
        return closestSlot.Key.Key;

    }

    public void Remove(Vector3 slotToRemove, bool isPlayerSlot)
    {
        GridSlots[new KeyValuePair<Vector3, bool>(slotToRemove, isPlayerSlot)] = true;
    }

    public void Free(Vector3 slotToFree, bool isPlayerSlot)
    {
        GridSlots[new KeyValuePair<Vector3, bool>(slotToFree, isPlayerSlot)] = false;
    }
}
