using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Card
{
    public int Mana { get; private set; }
    public int Money { get; private set; }
    public int Charm { get; private set; }

    public int currentMana;
    public int currentMoney;
    public int currentCharm;
    
    public Sprite Image => data.Image;
    
    public string CardName => data.CardName;

    public string Desc => data.Desc;
    
    public CardType CardType => data.CardType;
    
    public readonly CardData data;
    
    public List<Effect> Effects => data.Effects;

    public Card(CardData cardData)
    {
        data = cardData;
        Mana = cardData.Mana;
        currentMana = Mana;
        Money = cardData.Money;
        currentMoney = Money;
        Charm = cardData.Charm;
        currentCharm = Charm;
    }
    
}
