using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIMoney : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;
    private TMP_Text moneyText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moneyText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = "Money: $" + turnManager.CurrentPlayer.Money.ToString();
    }
}
