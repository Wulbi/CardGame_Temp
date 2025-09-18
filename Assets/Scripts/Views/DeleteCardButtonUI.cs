using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCardButtonUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image    artworkImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text charmText;
    [SerializeField] private TMP_Text mapTagText; // (선택) 맵 태그 표시용

    private Button _button;

    private CardData    _source;
    private Card        _runtime;
    private CardMapType _mapType;

    private Action<CardData, CardMapType> _onClick;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => _onClick?.Invoke(_source, _mapType));
        }
    }

    public void Setup(Card runtimeCard, CardData source, CardMapType map, Action<CardData, CardMapType> onClick)
    {
        _runtime  = runtimeCard;
        _source   = source;
        _mapType  = map;
        _onClick  = onClick;

        if (nameText)  nameText.text  = runtimeCard.CardName;
        if (descText)  descText.text  = runtimeCard.Desc;
        if (manaText)  manaText.text  = (runtimeCard.CardType == CardType.ACTION) ? runtimeCard.currentMana.ToString() : "M";
        if (moneyText) moneyText.text = runtimeCard.currentMoney.ToString();
        if (charmText) charmText.text = runtimeCard.currentCharm.ToString();
        if (artworkImage) artworkImage.sprite = runtimeCard.Image;

        if (mapTagText) mapTagText.text = MapShort(map);
    }

    private string MapShort(CardMapType m)
    {
        switch (m)
        {
            case CardMapType.COMMON:    return "Common";
            case CardMapType.CLASSROOM: return "Classroom";
            case CardMapType.STREET:    return "Street";
            case CardMapType.ROOM:      return "Room";
            case CardMapType.CLASSROOMSTREET: return "Cls+Str";
            case CardMapType.CLASSROOMROOM:   return "Cls+Room";
            case CardMapType.STREETROOM:      return "Str+Room";
            default: return m.ToString();
        }
    }
}
