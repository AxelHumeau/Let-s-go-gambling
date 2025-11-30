using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class UITurnOptionDisplayer : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameObject optionButtonPrefab;
    private TMP_Text textMeshPro;
    private List<GameObject> optionButtons = new List<GameObject>();
    private bool isUpdating = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        turnManager.OnChangeOptionCallback = UpdateDisplay;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        isUpdating = true;
        foreach (var button in optionButtons)
        {
            Destroy(button);
        }
        optionButtons.Clear();
        if (turnManager.endOfTurnReached)
        {
            textMeshPro.text = "";
            return;
        }
        else if (turnManager.SelectedAction == SelectedAction.FreeCam)
        {
            textMeshPro.text = $"Press 'Esc' to go back.";
            return;
        }
        switch (turnManager.SelectedAction)
        {
            case SelectedAction.UseItem:
            case SelectedAction.Move:
            case SelectedAction.ViewMap:
                textMeshPro.text = $"{turnManager.CurrentPlayer.playerName}, choose an option:";
                optionButtons.Add(CreateOptionButton("Use an Item", new Vector2(0, -150)));
                optionButtons.Add(CreateOptionButton("Move", new Vector2(0, -300)));
                optionButtons.Add(CreateOptionButton("View Map", new Vector2(0, -450)));
                break;
            case SelectedAction.BluffSelection:
                textMeshPro.text = $"You rolled {turnManager.RolledAmount}, would like to bluff ?";
                optionButtons.Add(CreateOptionButton("Yes", new Vector2(0, -150)));
                optionButtons.Add(CreateOptionButton("No", new Vector2(0, -300)));
                break;
            case SelectedAction.Bluff:
                textMeshPro.text = "How much do you want to move";
                optionButtons.Add(CreateOptionButton("1", new Vector2(0, -150)));
                break;
            case SelectedAction.BluffCalling:
                textMeshPro.text = $"{turnManager.CallingPlayer.playerName}, {turnManager.CurrentPlayer.playerName} could be bluffing, do you want to call them out?";
                optionButtons.Add(CreateOptionButton("Yes", new Vector2(0, -150)));
                optionButtons.Add(CreateOptionButton("No", new Vector2(0, -300)));
                break;
            case SelectedAction.SelectPath:
                textMeshPro.text = "Select your path";
                optionButtons.Add(CreateOptionButton("0", new Vector2(0, -150)));
                break;
            case SelectedAction.SelectItem:
                textMeshPro.text = "Select an item to use:";
                int xOffset = 0;
                foreach (var item in turnManager.CurrentPlayer.Inventory)
                {
                    optionButtons.Add(CreateOptionButton(item.type.ToString(), new Vector2(xOffset, -150)));
                    xOffset += 150;
                }
                break;
            case SelectedAction.SelectItemTarget:
                textMeshPro.text = "Select a target for your item:";
                optionButtons.Add(CreateOptionButton("You", new Vector2(0, -150)));
                break;
        }
        isUpdating = false;
    }

    GameObject CreateOptionButton(string optionText, Vector2 offset)
    {
        GameObject buttonObject = Instantiate(optionButtonPrefab);
        TMP_Text buttonText = buttonObject.GetComponent<TMP_Text>();
        buttonText.text = optionText;
        Image image = buttonObject.GetComponentInChildren<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        buttonObject.transform.SetParent(this.transform);
        buttonObject.transform.localPosition = new Vector3(offset.x, offset.y, 0);
        return buttonObject;
    }

    private void Update()
    {
        if (isUpdating)
        {
            return;
        }
        int index = 0;
        switch (turnManager.SelectedAction)
        {
            case SelectedAction.UseItem:
            case SelectedAction.Move:
            case SelectedAction.ViewMap:
                index = (int)turnManager.SelectedAction;
                break;
            case SelectedAction.BluffSelection:
            case SelectedAction.BluffCalling:
                index = turnManager.YesNoSelection ? 0 : 1;
                break;
            case SelectedAction.Bluff:
                int amount = turnManager.AmountToBluff.HasValue ? turnManager.AmountToBluff.Value : 1;
                optionButtons[0].GetComponent<TMP_Text>().text = amount.ToString();
                return;
            case SelectedAction.SelectPath:
                optionButtons[0].GetComponent<TMP_Text>().text = turnManager.PathChosen.ToString();
                return;
            case SelectedAction.SelectItem:
                index = turnManager.SelectedItemIndex;
                break;
            case SelectedAction.SelectItemTarget:
                Player selectedPlayer = turnManager.Players[turnManager.SelectedItemTargetIndex];
                optionButtons[0].GetComponent<TMP_Text>().text = selectedPlayer == turnManager.CurrentPlayer ? "You" : selectedPlayer.playerName;
                return;
        }
        ClearHoverEffect();
        var image = optionButtons[index].GetComponentInChildren<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
    }

    private void ClearHoverEffect()
    {
        foreach (var button in optionButtons)
        {
            var image = button.GetComponentInChildren<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
    }
}

