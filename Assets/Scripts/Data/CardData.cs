using UnityEngine;

[CreateAssetMenu(menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Mana { get; private set; }
    [field: SerializeField] public string CardName { get; private set; }
    [field: SerializeField] public string Desc { get; private set; }
    
}
