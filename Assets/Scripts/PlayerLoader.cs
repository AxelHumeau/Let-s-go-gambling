using UnityEngine;

[RequireComponent(typeof(TurnManager))]
public class PlayerLoader : MonoBehaviour
{
    private TurnManager turnManager;
    public GameObject PlayerPrefab;
    public Cell StartingCell;
    public void Awake()
    {
        turnManager = GetComponent<TurnManager>();
        foreach (string playerName in GameStartManager.players)
        {
            GameObject playerObject = Instantiate(PlayerPrefab);
            Player player = playerObject.GetComponent<Player>();
            player.playerName = playerName;
            player.CurrentCell = StartingCell;
            player.playerColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f);
            turnManager.Players.Add(player);
        }
    }
}
