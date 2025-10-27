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

    public int ManaMultiplier;
    public int MoneyMultiplier;
    public int CharmMultiplier;
    
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
        ManaMultiplier = cardData.ManaMultiplier;
        currentMana = Mana * ManaMultiplier;
        Money = cardData.Money;
        MoneyMultiplier = cardData.MoneyMultiplier;
        currentMoney = Money * MoneyMultiplier;
        Charm = cardData.Charm;
        CharmMultiplier = cardData.CharmMultiplier;
        currentCharm = Charm * CharmMultiplier;
    }
    
    public void RecomputeCurrent()
    {
        currentMana  = Mana  * Mathf.Max(1, ManaMultiplier);
        currentMoney = Money * Mathf.Max(1, MoneyMultiplier);
        currentCharm = Charm * Mathf.Max(1, CharmMultiplier);
    }

    public void ResetRuntime()
    {
        ManaMultiplier  = 1;
        MoneyMultiplier = 1;
        CharmMultiplier = 1;
        RecomputeCurrent();
    }
}
