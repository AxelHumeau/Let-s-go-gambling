using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;


public enum SelectedAction { UseItem, Move, ViewMap, BluffSelection, Bluff, BluffCalling, SelectPath, SelectItem, SelectItemTarget, Shop, FreeCam }

public class TurnManager : MonoBehaviour
{
    public List<Player> Players;
    public int turnLimit = 20;
    [SerializeField] private int unsuccessfulBluffPenalty = 10;
    private int currentPlayerIndex = 0;
    public Player CurrentPlayer
    {
        get { return Players[currentPlayerIndex]; }
    }
    private int turnCount = 0;
    public int TurnCount
    {
        get { return turnCount; }
    }
    private bool canUseItem = true;
    private SelectedAction selectedAction = SelectedAction.Move;
    private bool yesNoSelection = false; // false = No, true = Yes
    private int amountToMove = 1;
    private int? amountToBluff = null;
    private Player callingPlayer;
    public Action OnChangeOptionCallback;
    private bool endOfTurn = false;
    private int pathChosen = 0;
    private int selectedItemIndex = 0;
    private int selectedPlayerIndex = 0;
    private Dictionary<Cell, bool> cellActionCompletionStatus = new Dictionary<Cell, bool>();
    private bool isGameOver = false;

    public bool IsGameOver
    {
        get { return isGameOver; }
    }
    public bool WaitingForCellAction { get { return cellActionCompletionStatus.Any(pair  => pair.Value); } }
    public int SelectedItemIndex
    {
        get { return selectedItemIndex; }
    }
    public int SelectedItemTargetIndex
    {
        get { return selectedPlayerIndex; }
    }
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
        get { return canUseItem && CurrentPlayer.Inventory.Count != 0; }
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


    void CycleUpOnOption()
    {
        if (selectedAction == SelectedAction.UseItem)
        {
            selectedAction = SelectedAction.ViewMap;
        }
        else
        {
            selectedAction--;
            if (selectedAction == SelectedAction.UseItem && !CanUseItem)
            {
                CycleUpOnOption();
            }
        }
    }

    void CycleDownOnOption()
    {
        selectedAction++;
        if (selectedAction > SelectedAction.ViewMap)
        {
            selectedAction = SelectedAction.UseItem;
            if (!CanUseItem)
            {
                selectedAction++;
            }
        }
    }

    public void OnNavigate(CallbackContext context)
    {
        if (!context.started || WaitingForCellAction || selectedAction == SelectedAction.FreeCam) return;
        float verticalDirection = context.ReadValue<Vector2>().y;
        float horizontalDirection = context.ReadValue<Vector2>().x;
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
            case SelectedAction.SelectItem:
                if (horizontalDirection > 0) horizontalDirection = 1;
                else if (horizontalDirection < 0) horizontalDirection = -1;
                selectedItemIndex += (int)horizontalDirection;
                if (selectedItemIndex < 0) selectedItemIndex = 0;
                if (selectedItemIndex >= CurrentPlayer.Inventory.Count)
                    selectedItemIndex = CurrentPlayer.Inventory.Count - 1;
                break;
            case SelectedAction.SelectItemTarget:
                if (horizontalDirection > 0) horizontalDirection = 1;
                else if (horizontalDirection < 0) horizontalDirection = -1;
                selectedPlayerIndex += (int)horizontalDirection;
                if (selectedPlayerIndex < 0) selectedPlayerIndex = 0;
                if (selectedPlayerIndex >= Players.Count)
                    selectedPlayerIndex = Players.Count - 1;
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

    public void OnSubmit(CallbackContext context)
    {
        if (!context.started || WaitingForCellAction || selectedAction == SelectedAction.FreeCam) return;
        switch (selectedAction)
        {
            case SelectedAction.UseItem:
                if (CanUseItem)
                {
                    selectedAction = SelectedAction.SelectItem;
                    OnChangeOptionCallback();
                }
                break;
            case SelectedAction.Move:
                amountToMove = UnityEngine.Random.Range(1, 7);
                Debug.Log($"{CurrentPlayer.playerName} rolled a {amountToMove}.");
                if (CurrentPlayer.HasEffect(Effect.NoBluff))
                {
                    EndTurn();
                }
                else
                {
                    selectedAction = SelectedAction.BluffSelection;
                    OnChangeOptionCallback();
                }
                break;
            case SelectedAction.ViewMap:
                selectedAction = SelectedAction.FreeCam;
                OnChangeOptionCallback();
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
                Debug.Log($"{CurrentPlayer.playerName} is bluffing with {amountToBluff}.");
                selectedAction = SelectedAction.BluffCalling;
                int index = UnityEngine.Random.Range(0, Players.Count - 1);
                callingPlayer = Players.Where(player => player != CurrentPlayer).ToList()[index];
                OnChangeOptionCallback();
                break;
            case SelectedAction.BluffCalling: // Player B decides whether to call the bluff
                if (yesNoSelection)
                {
                    Debug.Log($"{callingPlayer.playerName} called a bluff");
                    if (amountToBluff.HasValue)
                    {
                        amountToMove = -amountToBluff.Value;
                        Debug.Log($"{CurrentPlayer.playerName} will move {amountToMove} due to the successful bluff call.");
                    }
                    else
                    {
                        callingPlayer.SubtractMoney(unsuccessfulBluffPenalty);
                        Debug.Log($"{callingPlayer.playerName} called a bluff unsuccessfully and loses {unsuccessfulBluffPenalty} money.");
                    }
                }
                else
                {
                    amountToMove = amountToBluff.Value;
                    Debug.Log($"{callingPlayer.playerName} did not call the bluff and {CurrentPlayer.playerName} will move {amountToMove}.");
                }
                EndTurn();
                break;
            case SelectedAction.SelectPath:
                CurrentPlayer.PathIndex = pathChosen;
                break;
            case SelectedAction.SelectItem:
                IItem itemToUse = CurrentPlayer.Inventory[selectedItemIndex];
                Debug.Log($"{CurrentPlayer.playerName} will use {itemToUse}.");
                if (itemToUse.target == ItemTarget.None)
                {
                    CurrentPlayer.UseItem(selectedItemIndex, null);
                    canUseItem = false;
                    selectedAction = SelectedAction.Move;
                }
                else
                {
                    selectedAction = SelectedAction.SelectItemTarget;
                }
                OnChangeOptionCallback();
                break;
            case SelectedAction.SelectItemTarget:
                Player targetPlayer = Players[selectedPlayerIndex];
                CurrentPlayer.UseItem(selectedItemIndex, targetPlayer);
                Debug.Log($"{CurrentPlayer.playerName} used item on {targetPlayer.playerName}.");
                canUseItem = false;
                selectedAction = SelectedAction.Move;
                OnChangeOptionCallback();
                break;
        }
    }

    public void OnCancel(CallbackContext context)
    {
        if (selectedAction == SelectedAction.FreeCam)
        {
            selectedAction = SelectedAction.Move;
            OnChangeOptionCallback();
        }
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
        CurrentPlayer.CurrentCell.OnStopOnCell(CurrentPlayer);
        yield return new WaitUntil(() => !WaitingForCellAction);
        Debug.Log($"{CurrentPlayer.playerName}'s turn has ended.");
        NextTurn();
    }

    private void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % Players.Count;
        if (currentPlayerIndex == 0)
        {
            turnCount++;
        }
        if (turnCount >= turnLimit)
        {
            Debug.Log("Turn limit reached. Game over.");
            endOfTurn = true;
            OnChangeOptionCallback();
            isGameOver = true;
            return;
        }
        canUseItem = true;
        selectedAction = SelectedAction.Move;
        selectedItemIndex = 0;
        selectedPlayerIndex = 0;
        yesNoSelection = false;
        amountToBluff = null;
        callingPlayer = null;
        pathChosen = 0;
        endOfTurn = false;
        OnChangeOptionCallback();
    }

    public void PauseForCellAction(Cell cell)
    {
        cellActionCompletionStatus[cell] = true;
    }

    public void CompleteCellAction(Cell cell)
    {
        if (cellActionCompletionStatus.ContainsKey(cell))
        {
            cellActionCompletionStatus[cell] = false;
        }
    }
}
