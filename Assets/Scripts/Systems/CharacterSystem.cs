using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSystem : Singleton<CharacterSystem>
{
    public Sprite characterSprite;
    public string characterName;
    
    public TMP_Text characterUIName;
    public Image characterUIImage;

    void OnEnable()
    {
        characterName = "민재";
        characterSprite = Resources.Load<Sprite>("Character/Minjae");
        Setup(); 
    }
    
    void Setup()
    {
        characterUIName.text = characterName;
        characterUIImage.sprite = characterSprite;

    }
}
