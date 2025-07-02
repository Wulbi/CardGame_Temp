using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardData cardData;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Card card = new Card(cardData);
            CardView cardview = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(cardview));
        }
    }
}
