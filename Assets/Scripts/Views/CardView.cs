using System;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private GameObject wrapper;
    
    [SerializeField] private TMP_Text mana;
    
    [SerializeField] private SpriteRenderer cardImage;
    
    [SerializeField] private TMP_Text cardName;
    
    [SerializeField] private TMP_Text desc;

    public Card ThisCard { get; private set; }
    public void Setup(Card card)
    {
        ThisCard = card;
        cardName.text = card.CardName;
        desc.text = card.Desc;
        mana.text = card.Mana.ToString();
        cardImage.sprite = card.Image;
    }

    private void OnMouseEnter()
    {
        wrapper.SetActive(false);
        Vector3 pos = new Vector3(this.transform.position.x, -2.0f, 0.0f);
        CardViewHoverSystem.Instance.ShowCard(ThisCard, pos);
    }

    private void OnMouseExit()
    {
        CardViewHoverSystem.Instance.HideCard();
        wrapper.SetActive(true);
    }
}
