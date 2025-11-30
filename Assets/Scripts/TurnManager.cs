using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;


public enum SelectedAction { UseItem, Move, ViewMap, BluffSelection, Bluff, BluffCalling, SelectPath }

public class TurnManager : MonoBehaviour
{
    [SerializeField] private Player[] players;
    [SerializeField] private int unsuccessfulBluffPenalty = 10;
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
    private bool yesNoSelection = false; // false = No, true = Yes
    private int amountToMove = 1;
    private int? amountToBluff = null;
    private Player callingPlayer;
    public Action OnChangeOptionCallback;
    private bool endOfTurn = false;
    private int pathChosen = 0;

    public int PathChosen
    {
        get { return pathChosen; }
    }
    public bool endOfTurnReached
    {
        get { return endOfTurn; }
    }
    public SelectedAction SelectedAction
    {
        get { return selectedAction; }
    }
    public bool CanUseItem
    {
        get { return canUseItem; }
    }
    public bool YesNoSelection
    {
        get { return yesNoSelection; }
    }
    public Player CallingPlayer
    {
        get { return callingPlayer; }
    }
    public int RolledAmount
    {
        get { return amountToMove; }
    }
    public int? AmountToBluff
    {
        get { return amountToBluff; }
    }


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
            if (selectedAction > SelectedAction.ViewMap)
            {
                selectedAction = SelectedAction.UseItem;
            }
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
            switch (selectedAction)
            {
                case SelectedAction.BluffSelection:
                case SelectedAction.BluffCalling:
                    yesNoSelection = !yesNoSelection;
                    break;
                case SelectedAction.Bluff:
                    if (verticalDirection > 0) verticalDirection = 1;
                    else if (verticalDirection < 0) verticalDirection = -1;
                    if (amountToBluff == null)
                        amountToBluff = 1;
                    amountToBluff += (int)verticalDirection;
                    if (amountToBluff < 1) amountToBluff = 1;
                    if (amountToBluff > 6) amountToBluff = 6;
                    break;
                case SelectedAction.SelectPath:
                    if (horizontalDirection > 0) horizontalDirection = 1;
                    else if (horizontalDirection < 0) horizontalDirection = -1;
                    pathChosen += (int)horizontalDirection;
                    if (pathChosen < 0) pathChosen = 0;
                    if (pathChosen >= CurrentPlayer.CurrentCell.nextCells.Count)
                        pathChosen = CurrentPlayer.CurrentCell.nextCells.Count - 1;
                    break;
                default:
                    if (verticalDirection > 0)
                    {
                        CycleUpOnOption();
                    }
                    else if (verticalDirection < 0)
                    {
                        CycleDownOnOption();
                    }
                    break;
            }
        }
        StartCoroutine(ActionCooldown(0.1f));
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
                    amountToMove = UnityEngine.Random.Range(1, 7);
                    Debug.Log($"{CurrentPlayer.name} rolled a {amountToMove}.");
                    selectedAction = SelectedAction.BluffSelection;
                    OnChangeOptionCallback();
                    break;
                case SelectedAction.ViewMap:
                    // Implement map viewing
                    break;
                case SelectedAction.BluffSelection: // Player chooses whether to bluff
                    if (yesNoSelection)
                    {
                        selectedAction = SelectedAction.Bluff;
                        yesNoSelection = false;
                        OnChangeOptionCallback();
                    }
                    else
                    {
                        EndTurn();
                    }
                    break;
                case SelectedAction.Bluff: // Player A selects amount to bluff, player B is randomly selected to call bluff
                    Debug.Log($"{CurrentPlayer.name} is bluffing with {amountToBluff}.");
                    selectedAction = SelectedAction.BluffCalling;
                    int index = UnityEngine.Random.Range(0, players.Length - 1);
                    callingPlayer = players.Where(player => player != CurrentPlayer).ToList()[index];
                    OnChangeOptionCallback();
                    break;
                case SelectedAction.BluffCalling: // Player B decides whether to call the bluff
                    if (yesNoSelection)
                    {
                        Debug.Log($"{callingPlayer.name} called a bluff");
                        if (amountToBluff.HasValue)
                        {
                            amountToMove = -amountToBluff.Value;
                            Debug.Log($"{CurrentPlayer.name} will move {amountToMove} due to the successful bluff call.");
                        }
                        else
                        {
                            callingPlayer.SubtractMoney(unsuccessfulBluffPenalty);
                            Debug.Log($"{callingPlayer.name} called a bluff unsuccessfully and loses {unsuccessfulBluffPenalty} money.");
                        }
                    }
                    else
                    {
                        amountToMove = amountToBluff.Value;
                        Debug.Log($"{callingPlayer.name} did not call the bluff and {CurrentPlayer.name} will move {amountToMove}.");
                    }
                    EndTurn();
                    break;
                case SelectedAction.SelectPath:
                    CurrentPlayer.PathIndex = pathChosen;
                    break;
            }
        }
        StartCoroutine(ActionCooldown(0.2f));
    }

    private void EndTurn()
    {
        CurrentPlayer.StartMove(amountToMove);
        endOfTurn = true;
        OnChangeOptionCallback();
        StartCoroutine(WaitForPlayerToStopMoving());
    }

    private IEnumerator WaitForPlayerToStopMoving()
    {
        while (CurrentPlayer.IsMoving)
        {
            yield return new WaitUntil(() => !CurrentPlayer.IsMoving || CurrentPlayer.WaitingForPathChoice);
            if (CurrentPlayer.WaitingForPathChoice)
            {
                endOfTurn = false;
                selectedAction = SelectedAction.SelectPath;
                OnChangeOptionCallback();
            }
            yield return new WaitUntil(() => !CurrentPlayer.WaitingForPathChoice);
            endOfTurn = true;
            OnChangeOptionCallback();
        }
        Debug.Log($"{CurrentPlayer.name}'s turn has ended.");
        NextTurn();
    }

    private void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        if (currentPlayerIndex == 0)
        {
            turnCount++;
        }
        canUseItem = true;
        isSelectingItem = false;
        selectedAction = SelectedAction.Move;
        yesNoSelection = false;
        amountToBluff = null;
        callingPlayer = null;
        pathChosen = 0;
        endOfTurn = false;
        StartCoroutine(ActionCooldown(1.0f));
        OnChangeOptionCallback();
    }
}
