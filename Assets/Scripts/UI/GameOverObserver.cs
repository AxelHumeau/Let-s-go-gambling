using System.Linq;
using TMPro;
using UnityEngine;

public class GameOverObserver : MonoBehaviour
{
    public TurnManager turnManager;

    void Update()
    {
        if (turnManager.IsGameOver)
        {
            GetComponent<TMP_Text>().text = "Game Over! Winner: " + turnManager.Players.OrderBy(p => p.Money).First().playerName;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
