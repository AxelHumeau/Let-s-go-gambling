using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    static public List<string> players = new List<string>();
    [SerializeField] private DynamicInputGrid inputGrid;
    public void OnQuit()
    {
        Application.Quit();
    }

    public void OnStart()
    {
        inputGrid.CreatedFields.Select(e => e.GetComponent<TMPro.TMP_InputField>().text).ToList().ForEach(name =>
        {
            if (!string.IsNullOrWhiteSpace(name) && !players.Contains(name))
                players.Add(name);
        });
        if (players.Count < 2)
        {
            Debug.LogWarning("At least two players are required to start the game.");
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("TrueBoard");
    }
}
