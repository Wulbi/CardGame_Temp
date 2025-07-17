using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private GameObject wrapper;
    
    [SerializeField] private TMP_Text mana;
    [SerializeField] private CardType cardType;
    
    [SerializeField] private TMP_Text money;
    
    [SerializeField] private TMP_Text charm;
    
    [SerializeField] private SpriteRenderer cardImage;
    
    [SerializeField] private TMP_Text cardName;
    
    [SerializeField] private TMP_Text desc;

    [SerializeField] private LayerMask dropAreaLayer;
    public Card ThisCard { get; private set; }

    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;
    public void Setup(Card card)
    {
        ThisCard = card;
        cardType = card.CardType;
        cardName.text = card.CardName;
        desc.text = card.Desc;
        if (cardType == CardType.ACTION)
            mana.text = card.Mana.ToString();
        else
            mana.text = "M";
        money.text = card.Money.ToString();
        charm.text = card.Charm.ToString();
        cardImage.sprite = card.Image;
    }

    void OnMouseEnter()
    {
        if (!InteractionSystem.Instance.PlayerCanHover()) return;
        wrapper.SetActive(false);
        Vector3 pos = new Vector3(this.transform.position.x, -2.0f, 0.0f);
        CardViewHoverSystem.Instance.ShowCard(ThisCard, pos);
    }

    void OnMouseExit()
    {
        if (!InteractionSystem.Instance.PlayerCanHover()) return;
        CardViewHoverSystem.Instance.HideCard();
        wrapper.SetActive(true);
    }

    void OnMouseDown()
    {
        if (!InteractionSystem.Instance.PlayerCanInteract()) return;
        InteractionSystem.Instance.PlayerIsDragging = true;
        wrapper.SetActive(true);
        CardViewHoverSystem.Instance.HideCard();
        dragStartPosition = transform.position;
        dragStartRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0,0,0);
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    void OnMouseDrag()
    {
        if (!InteractionSystem.Instance.PlayerCanInteract()) return;
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    void OnMouseUp()
    {
        if (!InteractionSystem.Instance.PlayerCanInteract()) return;
        if (ManaSystem.Instance.HasEnoughMana(ThisCard.Mana) 
            && Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, dropAreaLayer))
        {
            PlayCardGA playCardGA = new(ThisCard);
            ActionSystem.Instance.Perform(playCardGA);
        }
        else
        {
            transform.position = dragStartPosition;
            transform.rotation = dragStartRotation;
        }
        InteractionSystem.Instance.PlayerIsDragging = false;
    }
}
