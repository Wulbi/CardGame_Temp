using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Mana { get; private set; }
    public float ManaMultiplier { get; private set; } = 1;
    [field: SerializeField] public int Money { get; private set; }
    public float MoneyMultiplier { get; private set; } = 1;
    [field: SerializeField] public int Charm { get; private set; }
    public float CharmMultiplier { get; private set; } = 1;
    [field: SerializeField] public string CardName { get; private set; }
    [field: SerializeField] public string Desc { get; private set; }
    [field: SerializeField] public CardType CardType { get; private set; }
    
    [field: SerializeField] public CardMapType CardMapType { get; private set; }
    [field: SerializeReference, SR] public List<Effect> Effects { get; private set; }
    
    
    
}
