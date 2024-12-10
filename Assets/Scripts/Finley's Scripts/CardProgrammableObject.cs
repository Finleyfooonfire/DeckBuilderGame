using UnityEngine;

[System.Serializable]
public class CardDetails : MonoBehaviour
{
    // Basic card data fields
    public string cardName;
    public int cardAttack;
    public int cardHealth;
    public Sprite cardImage; // Could also be a Texture if using a 3D texture on the mesh
    public MeshRenderer cardMeshRenderer; // Reference to mesh renderer to change textures, materials, etc.

    // Add any other attributes you need for the card
    public string cardType;
    public string cardDescription;
    public string cardFaction;
}
