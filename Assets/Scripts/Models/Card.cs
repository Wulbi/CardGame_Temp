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

    public int addMana;
    public int addMoney;
    public int addCharm;
    
    public float ManaMultiplier;
    public float MoneyMultiplier;
    public float CharmMultiplier;
    
    public Sprite Image => data.Image;
    
    public string CardName => data.CardName;

    public string Desc { get; private set; }
    
    public CardType CardType => data.CardType;
    
    public readonly CardData data;
    
    public List<Effect> Effects => data.Effects;

    public Card(CardData cardData)
    {
        data = cardData;
        addMana = 0;
        addMoney = 0;
        addCharm = 0;
        Mana = cardData.Mana;
        ManaMultiplier = cardData.ManaMultiplier;
        currentMana = Mathf.CeilToInt((Mana + addMana) * ManaMultiplier);
        Money = cardData.Money;
        MoneyMultiplier = cardData.MoneyMultiplier;
        currentMoney = Mathf.CeilToInt((Money + addMoney)* MoneyMultiplier);
        Charm = cardData.Charm;
        CharmMultiplier = cardData.CharmMultiplier;
        currentCharm = Mathf.CeilToInt((Charm + addCharm) * CharmMultiplier);
        Desc = cardData.Desc;
    }
    
    public void RecomputeCurrent()
    {
        currentMana = Mathf.CeilToInt((Mana + addMana) * ManaMultiplier);
        currentMoney = Mathf.CeilToInt((Money + addMoney)* MoneyMultiplier);
        currentCharm = Mathf.CeilToInt((Charm + addCharm) * CharmMultiplier);
    }

    public void SetDesc(string newDesc)
    {
        Desc = newDesc;
    }
    public void ResetNums()
    {
        addMana = 0;
        addMoney = 0;
        addCharm = 0;
        RecomputeCurrent();
    }
    public void ResetMultiplier()
    {
        ManaMultiplier  = 1;
        MoneyMultiplier = 1;
        CharmMultiplier = 1;
        RecomputeCurrent();
    }

    public void ResetAll()
    {
        ResetNums();
        ResetMultiplier();
        RecomputeCurrent();
    }
}
