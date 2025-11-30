using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Effect { Double, Half, LuckIncrease, LuckDecrease, NoBluff, MineResistant }

public class Player : MonoBehaviour
{
    public string playerName;
    private int money = 50;
    public Cell CurrentCell;
    private Dictionary<Effect, int> activeEffects = new Dictionary<Effect, int>();
    private List<IItem> inventory = new List<IItem>();
    private int stocksOwned = 0;
    private bool isMoving = false;
    private int? pathIndex = null;
    private bool waitingForPathChoice = false;

    public List<IItem> Inventory
    {
        get { return inventory; }
    }
    public bool WaitingForPathChoice
    {
        get { return waitingForPathChoice; }
    }
    public int? PathIndex
    {
        set { pathIndex = value; }
    }
    public bool IsMoving
    {
        get { return isMoving; }
    }

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
        if (money < 0) money = 0;
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

    public bool HasEffect(Effect effect)
    {
        return activeEffects.GetValueOrDefault(effect) > 0;
    }

    public bool AddItem(IItem item)
    {
        if (inventory.Count >= 4) return false;
        inventory.Add(item);
        return true;
    }

    public void UseItem(int index, Player target)
    {
        if (index < 0 || index >= inventory.Count) return;
        IItem item = inventory[index];
        item.Use(target, this);
        inventory.RemoveAt(index);
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= inventory.Count) return;
        inventory.RemoveAt(index);
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
            if ((CurrentCell.nextCells.Count == 0 && !isBackward) || (CurrentCell.previousCells.Count == 0 && isBackward))
            {
                break;
            }
            if (!isBackward && CurrentCell.nextCells.Count > 1)
            {
                int? index = pathIndex;
                waitingForPathChoice = true;
                yield return new WaitUntil(() => index != pathIndex);
            }
            waitingForPathChoice = false;
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
                int index = Random.Range(0, CurrentCell.previousCells.Count);
                CurrentCell = CurrentCell.previousCells[index];
                return;
            }
            CurrentCell = CurrentCell.previousCells.First();
            return;
        }
        if (CurrentCell.nextCells.Count > 0 && pathIndex.HasValue)
        {
            CurrentCell = CurrentCell.nextCells[pathIndex.Value];
            pathIndex = null;
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
