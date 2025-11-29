using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;


public enum SelectedAction { UseItem, Move, ViewMap, BluffSelection, Bluff, BluffCalling }

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
                    amountToMove = UnityEngine.Random.Range(1, 7);
                    Debug.Log($"{CurrentPlayer.name} rolled a {amountToMove}.");
                    selectedAction = SelectedAction.BluffSelection;
                    break;
                case SelectedAction.ViewMap:
                    // Implement map viewing
                    break;
                case SelectedAction.BluffSelection: // Player chooses whether to bluff
                    if (yesNoSelection)
                    {
                        selectedAction = SelectedAction.Bluff;
                        yesNoSelection = false;
                    }
                    else
                    {
                        EndTurn();
                    }
                    break;
                case SelectedAction.Bluff: // Player A selects amount to bluff, player B is randomly selected to call bluff
                    Debug.Log($"{CurrentPlayer.name} is bluffing with {amountToBluff}.");
                    selectedAction = SelectedAction.BluffCalling;
                    int index = Random.Range(0, players.Length - 1);
                    callingPlayer = players.Where(player => player != CurrentPlayer).ToList()[index];
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
            }
        }
        StartCoroutine(ActionCooldown(0.2f));
    }

    private void EndTurn()
    {
        CurrentPlayer.StartMove(amountToMove);
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
        yesNoSelection = false;
        amountToBluff = null;
        callingPlayer = null;
    }
}
