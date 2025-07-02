using UnityEngine;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
   [SerializeField] private CardView cardViewHover;

   public void ShowCard(Card card, Vector3 position)
   {
      cardViewHover.gameObject.SetActive(true);
      cardViewHover.Setup(card);
      cardViewHover.transform.position = position;
   }

   public void HideCard()
   {
      cardViewHover.gameObject.SetActive(false);
   }
}
