using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class UIStockMenu : MonoBehaviour
{
    private Player _player;

    private int _stocksOwned;
    private int _transactionStockAmount = 0;

    private int _money;
    private int _newMoney;

    private int _selectedOption = 0;

    [SerializeField] private Color _bullishColor = Color.green;
    [SerializeField] private Color _bearishColor = Color.red;

    [SerializeField] private TextMeshProUGUI _stocksOwnedText;
    [SerializeField] private TextMeshProUGUI _newMoneyAmountText;

    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _confirmButton;

    public void SetPlayer(Player player) {
        _player = player;
        _money = _player.Money;
        _stocksOwned = _player.GetStocksOwned();
        _transactionStockAmount = 0;
        _selectedOption = 0;
        _cancelButton.image.color = new Color(1f, 1f, 1f, 0.25f);
        _confirmButton.image.color = new Color(1f, 1f, 1f, 1.0f);
        UpdateUI();
    }

    public void Buy(int amount)
    {
        int costPerStock = Mathf.RoundToInt(StockManager.Instance.GetCurrentValue());
        int totalCost = costPerStock * amount;
        int availableMoney = _money - Mathf.RoundToInt(StockManager.Instance.GetCurrentValue() * _transactionStockAmount);

        if (totalCost > availableMoney)
            return;
        _transactionStockAmount += amount;
        UpdateUI();
    }

    public void BuyMaximum()
    {
        int costPerStock = Mathf.RoundToInt(StockManager.Instance.GetCurrentValue());
        if (costPerStock <= 0)
            return;

        int availableMoney = _money - Mathf.RoundToInt(StockManager.Instance.GetCurrentValue() * _transactionStockAmount);
        int maxAffordable = availableMoney / costPerStock;

        if (maxAffordable > 0)
        {
            _transactionStockAmount += maxAffordable;
            UpdateUI();
        }
    }

    public void Sell(int amount)
    {
        if (amount > _stocksOwned + _transactionStockAmount)
            return;
        _transactionStockAmount -= amount;
        UpdateUI();
    }

    public void SellMaximum()
    {
        int maxToSell = _stocksOwned + _transactionStockAmount;
        if (maxToSell > 0)
        {
            _transactionStockAmount = -_stocksOwned;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (_transactionStockAmount > 0) {
            _stocksOwnedText.text = $"{_stocksOwned + _transactionStockAmount} (+{_transactionStockAmount})";
            _stocksOwnedText.color = _bullishColor;
        } else if (_transactionStockAmount < 0) {
            _stocksOwnedText.text = $"{_stocksOwned + _transactionStockAmount} (-{_transactionStockAmount})";
            _stocksOwnedText.color = _bearishColor;
        } else {
            _stocksOwnedText.text = $"{_stocksOwned + _transactionStockAmount}";
            _stocksOwnedText.color = Color.white;
        }

        _newMoney = _money - Mathf.RoundToInt(StockManager.Instance.GetCurrentValue() * _transactionStockAmount);
        if (_newMoney > _money) {
            _newMoneyAmountText.text = $"${_newMoney} (+${_newMoney - _money})";
            _newMoneyAmountText.color = _bullishColor;
        } else if (_newMoney < _money) {
            _newMoneyAmountText.text = $"${_newMoney} (-${_money - _newMoney})";
            _newMoneyAmountText.color = _bearishColor;
        } else {
            _newMoneyAmountText.text = $"${_newMoney}";
            _newMoneyAmountText.color = Color.white;
        }
    }

    public void ConfirmTransaction()
    {
        if (_transactionStockAmount == 0)
        {
            CloseMenu();
            return;
        }

        int costPerStock = Mathf.RoundToInt(StockManager.Instance.GetCurrentValue());
        int totalCost = costPerStock * _transactionStockAmount;

        if (_transactionStockAmount > 0)
        {
            if (totalCost <= _player.Money)
            {
                _player.SubtractMoney(totalCost);
                _player.AddStocks(_transactionStockAmount);
            }
        }
        else if (_transactionStockAmount < 0)
        {
            int stocksToSell = -_transactionStockAmount;
            if (stocksToSell <= _player.GetStocksOwned())
            {
                _player.AddMoney(costPerStock * stocksToSell);
                _player.RemoveStocks(stocksToSell);
            }
        }

        CloseMenu();
    }

    public void CloseMenu() {
        StockManager.Instance.CloseMenu();
        StockManager.Instance.UpdateGraph();
    }

    public void OnNavigate(CallbackContext context)
    {
        if (!StockManager.Instance.IsOpen || !context.started)
            return;
        var input = context.ReadValue<Vector2>();
        if (input.x > 0)
        {
            _selectedOption++;
            if (_selectedOption >= 2)
            {
                _selectedOption = 0;
            }
        }
        else if (input.x < 0)
        {
            _selectedOption--;
            if (_selectedOption < 0)
            {
                _selectedOption = 2 - 1;
            }
        }
        if (_selectedOption == 0)
        {
            _cancelButton.image.color = new Color(1f, 1f, 1f, 0.25f);
            _confirmButton.image.color = new Color(1f, 1f, 1f, 1.0f);
        } else if (_selectedOption == 1)
        {
            _confirmButton.image.color = new Color(1f, 1f, 1f, 0.25f);
            _cancelButton.image.color = new Color(1f, 1f, 1f, 1.0f);
        }
    }

    public void OnSubmit(CallbackContext context)
    {
        if (!StockManager.Instance.IsOpen || !context.started)
            return;

        if (_selectedOption == 0) {
            CloseMenu();
        } else if (_selectedOption == 1) {
            ConfirmTransaction();
        }
    }
}
