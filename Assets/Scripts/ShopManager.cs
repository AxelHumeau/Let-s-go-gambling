using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using static UnityEngine.InputSystem.InputAction;
using System;
using UnityEngine.InputSystem;

public class ShopManager : MonoBehaviour
{
	public bool IsOpen = false;
	public List<IItem> ItemsForSale = new List<IItem>();
    private List<IItem> possibleItems = new List<IItem> { new DoubleDice(), new LesserDice() };
    private int selectedOption = 0;
    private Player player;
    private Action closeCallback;

    public int SelectedOption
    {
        get { return selectedOption; }
    }

    public void OnNavigate(CallbackContext context)
    {
        if (!IsOpen || !context.started) return;
        var input = context.ReadValue<Vector2>();
        if (input.y != 0)
        {
            selectedOption = selectedOption == ItemsForSale.Count ? 0 : ItemsForSale.Count; // Select/Deselect "Exit Shop" option
        }
        if (input.x > 0)
        {
            selectedOption++;
            if (selectedOption >= ItemsForSale.Count)
            {
                selectedOption = 0;
            }
        }
        else if (input.x < 0)
        {
            selectedOption--;
            if (selectedOption < 0)
            {
                selectedOption = ItemsForSale.Count - 1;
            }
        }

    }

    public void OnSubmit(CallbackContext context)
    {
        if (!IsOpen || !context.started) return;
        if (selectedOption == ItemsForSale.Count)
        {
            CloseShop();
        }
        else
        {
            IItem selectedItem = ItemsForSale[selectedOption];
            int price = MapItemPrice(selectedItem);
            if (player.Money >= price) // Assuming each item costs 100
            {
                player.SubtractMoney(price);
                player.Inventory.Add(selectedItem);
                ItemsForSale.RemoveAt(selectedOption);
                CloseShop();
            }
            else
            {
                Debug.Log("Not enough money to buy this item.");
            }
        }
    }

    public void OpenShop(Player player, Action closeCallback)
    {
        IsOpen = true;
        selectedOption = 0;
        this.player = player;
        this.closeCallback = closeCallback;
        for (int i = 0; i < 4; i++)
		{
            int randomIndex = UnityEngine.Random.Range(0, possibleItems.Count);
            ItemsForSale.Add(possibleItems[randomIndex]);
        }
    }

    public void CloseShop()
    {
        closeCallback?.Invoke();
        IsOpen = false;
        closeCallback = null;
        ItemsForSale.Clear();
    }

    public int MapItemPrice(IItem item)
    {
        switch (item)
        {
            case DoubleDice:
                return 100;
            case LesserDice:
                return 80;
            default:
                return 50;
        }
    }

    private void Update()
    {
    }
}