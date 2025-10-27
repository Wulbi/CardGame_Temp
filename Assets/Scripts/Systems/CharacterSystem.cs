using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSystem : Singleton<CharacterSystem>
{
    public Sprite characterSprite;
    public string characterNameText;
    
    public TMP_Text characterName;
    public SpriteRenderer characterImage;

    void OnEnable()
    {
        characterNameText = "민재";
        characterSprite = Resources.Load<Sprite>("Character/Minjae1");
        Setup(); 
    }
    
    void Setup()
    {
        characterName.text = characterNameText;
        characterImage.sprite = characterSprite;

    }
}
