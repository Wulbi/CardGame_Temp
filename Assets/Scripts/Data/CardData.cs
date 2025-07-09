using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Mana { get; private set; }
    [field: SerializeField] public string CardName { get; private set; }
    [field: SerializeField] public string Desc { get; private set; }
    
    [field: SerializeReference, SR] public List<Effect> Effects { get; private set; }
    
}
