using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Effect { Double, Half, LuckIncrease, LuckDecrease, NoBluff, MineResistant }
public enum Item { Juggernaut, LuckyCharm, DoubleDice, LesserDice, TruthLasso, CurseDoll, Krach, TeleportXCellAhead, SwapPlayer, SwapCells, TeleportToStocksCell, StealItem }

public class Player : MonoBehaviour
{
    private int money = 50;
    public Cell CurrentCell;
    private Dictionary<Effect, int> activeEffects = new Dictionary<Effect, int>();
    private List<Item> inventory = new List<Item>();
    private int stocksOwned = 0;
    private bool isMoving = false;

    public int Money {
        get { return money; }
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
    }

    public void AddEffect(Effect effect, int duration)
    {
        if (activeEffects.ContainsKey(effect))
        {
            activeEffects[effect] = duration;
        }
        else
        {
            activeEffects.Add(effect, duration);
        }
    }

    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public void BuyStock()
    {
        // To implement
    }

    public void SellStock()
    {
        // To implement
    }

    public int GetStocksValue()
    {
        // To implement
        return 0;
    }

    private IEnumerator Move(int amount)
    {
        isMoving = true;
        bool isBackward = amount < 0;
        if (isBackward)
        {
            amount = -amount;
        }
        for (int i = 0; i < amount; i++)
        {
            MoveToNextCell(isBackward);
            yield return new WaitForSeconds(0.5f);
        }
        isMoving = false;
    }

    private void MoveToNextCell(bool isBackward)
    {
        if (isBackward)
        {
            if (CurrentCell.previousCells.Count > 0)
            {
                // implement branching paths later
                CurrentCell = CurrentCell.previousCells.First();
                return;
            }
            CurrentCell = CurrentCell.previousCells.First();
            return;
        }
        if (CurrentCell.nextCells.Count > 0)
        {
            // implement branching paths later
            CurrentCell = CurrentCell.nextCells.First();
            return;
        }
        CurrentCell = CurrentCell.nextCells.First();
    }

    public void StartMove(int amount)
    {
        if (!isMoving)
        {
            float multiplier = 1.0f;
            if (activeEffects.ContainsKey(Effect.Double))
            {
                multiplier *= 2.0f;
            }
            if (activeEffects.ContainsKey(Effect.Half))
            {
                multiplier *= 0.5f;
            }
            StartCoroutine(Move((int)(amount * multiplier)));
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = CurrentCell.transform.position;
    }
}
