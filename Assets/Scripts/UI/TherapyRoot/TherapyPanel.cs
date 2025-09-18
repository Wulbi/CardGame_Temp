using UnityEngine;
using UnityEngine.UI;

public class TherapyPanel : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button cardAddButton;
    [SerializeField] private Button cardDeleteButton;

    [Header("Panels")]
    [SerializeField] private TherapyCardAddPanel    cardAddPanel;
    [SerializeField] private TherapyCardDeletePanel cardDeletePanel;

    [Header("기본 후보 카드(없으면 패널 내부 fallback 사용)")]
    [SerializeField] private CardData[] defaultAddCandidates;

    void Awake()
    {
        if (cardAddButton)    cardAddButton.onClick.AddListener(OnClickAdd);
        if (cardDeleteButton) cardDeleteButton.onClick.AddListener(OnClickDelete);
    }

    private void OnClickAdd()
    {
        if (!cardAddPanel) return;
        SetAddDeleteVisible(false);

        cardAddPanel.Open(
            defaultAddCandidates,
            onClosed: () => SetAddDeleteVisible(true)
        );
    }

    private void OnClickDelete()
    {
        if (!cardDeletePanel) return;
        SetAddDeleteVisible(false);

        cardDeletePanel.Open(
            onClosed: () => SetAddDeleteVisible(true)
        );
    }

    private void SetAddDeleteVisible(bool visible)
    {
        if (cardAddButton)    cardAddButton.gameObject.SetActive(visible);
        if (cardDeleteButton) cardDeleteButton.gameObject.SetActive(visible);
    }
}