using System;
using TMPro;
using UnityEngine;

public class CardButton : MonoBehaviour
{
    [SerializeField] private GameObject wrapper;

    [SerializeField] private TMP_Text mana;
    [SerializeField] private CardType cardType;

    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text charm;

    [SerializeField] private SpriteRenderer cardImage;

    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text desc;

    [SerializeField] private CardMapType cardMapType;

    public Card ThisCard { get; private set; }
    public CardData SourceData { get; private set; } // Dream에서 주입
    
    public Action<CardButton> OnClickedExternally;

    public void Setup(Card card, CardData sourceData = null)
    {
        ThisCard   = card;
        SourceData = sourceData;

        cardType = card.CardType;
        cardName.text = card.CardName;
        desc.text = card.Desc;

        mana.text = (cardType == CardType.ACTION) ? card.currentMana.ToString() : "M";
        money.text = card.currentMoney.ToString();
        charm.text = card.currentCharm.ToString();

        cardImage.sprite = card.Image;
    }
    
    public void SetMapTypeIfExists(CardMapType mapType)  // SendMessage로도 호출 가능
    {
        cardMapType = mapType;
    }

    public CardMapType GetMapTypeOrDefault(CardMapType fallback)
    {
        // enum 기본값이 COMMON이면 그냥 cardMapType 반환해도 OK
        return cardMapType != 0 ? cardMapType : fallback;
    }


    void OnMouseEnter()
    {
        if (!InteractionSystem.Instance.PlayerCanHover()) return;
        //if (wrapper) wrapper.SetActive(false);
    }

    void OnMouseExit()
    {
        if (!InteractionSystem.Instance.PlayerCanHover()) return;
        //if (wrapper) wrapper.SetActive(true);
    }

    void OnMouseDown()
    {
        if (!InteractionSystem.Instance.PlayerCanInteract()) return;
        InteractionSystem.Instance.PlayerIsDragging = true;
        if (wrapper) wrapper.SetActive(true);
    }

    void OnMouseUp()
    {
        if (!InteractionSystem.Instance.PlayerCanInteract()) return;
        
        // 🔹 TherapyCardAddPanel에서 외부 콜백으로만 처리하는 경우
        if (OnClickedExternally != null)
        {
            OnClickedExternally.Invoke(this);
            return; // EnemyTurnGA 같은 Dream 전투용 처리 안 함
        }
        
        // 🔹 DREAM 선택: CardData를 맵 타입에 맞게 MatchSetupSystem에 등록
        if (SourceData != null)
        {
            MatchSetupSystem.Instance.RegisterCardForMap(SourceData, cardMapType);
        }
        else
        {
            Debug.LogWarning("[CardButton] SourceData is null. Did you pass CardData when creating the button?");
        }

        // 기존 흐름 유지: 적 턴 시작(맵 전환 포함)
        ActionSystem.Instance.Perform(new EnemyTurnGA());
    }

    private void OnDestroy()
    {
        if (CardViewCreator.Instance != null && ThisCard != null)
            CardViewCreator.Instance.RemoveCardView(ThisCard);
    }
    
    
}
