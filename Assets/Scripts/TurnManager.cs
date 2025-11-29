using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public enum SelectedAction { UseItem, Move, ViewMap }

public class TurnManager : MonoBehaviour
{
    [SerializeField] private Player[] players;
    private int currentPlayerIndex = 0;
    public Player CurrentPlayer
    {
        get { return players[currentPlayerIndex]; }
    }
    private int turnCount = 0;
    public int TurnCount
    {
        get { return turnCount; }
    }
    private bool canUseItem = true;
    private bool isSelectingItem = false;
    private SelectedAction selectedAction = SelectedAction.Move;
    private bool isOnCooldown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ActionCooldown(float cooldownDuration)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }

    void CycleUpOnOption()
    {
        if (selectedAction == SelectedAction.UseItem)
        {
            selectedAction = SelectedAction.ViewMap;
        }
        else
        {
            selectedAction--;
        }
    }

    void CycleDownOnOption()
    {
        if (selectedAction == SelectedAction.ViewMap)
        {
            selectedAction = SelectedAction.UseItem;
        }
        else
        {
            selectedAction++;
        }
    }

    public void OnNavigate(CallbackContext context)
    {
        if (isOnCooldown) return;
        float verticalDirection = context.ReadValue<Vector2>().y;
        float horizontalDirection = context.ReadValue<Vector2>().x;
        if (isSelectingItem)
        {
            // Handle item selection navigation
        }
        else
        {
            if (verticalDirection > 0)
            {
                CycleUpOnOption();
            }
            else if (verticalDirection < 0)
            {
                CycleDownOnOption();
            }
        }
        StartCoroutine(ActionCooldown(0.2f));
    }

    public void OnSubmit(CallbackContext context)
    {
        if (isOnCooldown) return;
        if (isSelectingItem)
        {
            // Handle item selection confirmation
        }
        else
        {
            switch (selectedAction)
            {
                case SelectedAction.UseItem:
                    if (canUseItem)
                    {
                        isSelectingItem = true;
                    }
                    break;
                case SelectedAction.Move:
                    int amountToMove = UnityEngine.Random.Range(1, 7);
                    Debug.Log($"{CurrentPlayer.name} rolled a {amountToMove}.");
                    CurrentPlayer.StartMove(amountToMove);
                    EndTurn();
                    break;
                case SelectedAction.ViewMap:
                    // Implement map viewing
                    break;
            }
        }
        StartCoroutine(ActionCooldown(0.2f));
    }

    private void EndTurn()
    {
        StartCoroutine(ActionCooldown(1.0f));
        Debug.Log($"{CurrentPlayer.name}'s turn has ended.");
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        if (currentPlayerIndex == 0)
        {
            turnCount++;
        }
        canUseItem = true;
        isSelectingItem = false;
        selectedAction = SelectedAction.Move;
    }
}
