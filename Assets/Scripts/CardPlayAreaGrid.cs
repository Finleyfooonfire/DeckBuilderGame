using System.Collections.Generic;
using UnityEngine;

public class CardPlayAreaGrid : MonoBehaviour
{
    [SerializeField] Vector2 gridSize = new Vector2(3, 3);
    [SerializeField] Vector2 slotSize = new Vector2(1f, 1f);
    [SerializeField] Vector2 slotSpacing = new Vector2(0.25f, 0.25f);
    public List<Vector3> GridSlots { get; private set; }  = new List<Vector3>();
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
                GridSlots.Add(slotPosition);
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
}
