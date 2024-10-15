using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public int numberOfCards = 6;

    void Start()
    {
        DisplayCards();
    }

    void DisplayCards()
    {
        RectTransform panelRectTransform = GetComponent<RectTransform>();
        float panelWidth = panelRectTransform.rect.width;
        float cardWidth = cardPrefab.GetComponent<RectTransform>().rect.width;

        float totalCardsWidth = numberOfCards * cardWidth;

        float spacing = (panelWidth - totalCardsWidth) / (numberOfCards + 1);

        float startX = -panelWidth / 2 + spacing + cardWidth / 2;

        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);

            RectTransform cardRect = card.GetComponent<RectTransform>();

            float posX = startX + i * (cardWidth + spacing);
            cardRect.anchoredPosition = new Vector2(posX, cardRect.anchoredPosition.y);

            Card cardScript = card.GetComponent<Card>();
            cardScript.isPlayerCard = true;
        }

    }
}
