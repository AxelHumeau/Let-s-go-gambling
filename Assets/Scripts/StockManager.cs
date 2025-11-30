using System;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    private static StockManager _instance;

    [SerializeField] private StockGraph _stockGraph;
    [SerializeField] private UIStockMenu _uiStockMenu;

    private List<StockGraph.CandlestickData> _stockData = new();
    private float _currentPrice = 100f;
    private float _updateTimer = 0f;
    private float _updateInterval = 0.5f;

    private bool _isOpen = false;
    private Player _player;
    private Action _closeCallback;

    public static StockManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<StockManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("StockManager");
                    _instance = singletonObject.AddComponent<StockManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _uiStockMenu.gameObject.SetActive(false);
        GenerateStartData(50);
    }

    private void GenerateAndAddNewCandle()
    {
        float randomChange = UnityEngine.Random.Range(-5f, 5f);
        float open = _currentPrice;
        float close = _currentPrice + randomChange;
        float high = Mathf.Max(open, close) + UnityEngine.Random.Range(0f, 3f);
        float low = Mathf.Min(open, close) - UnityEngine.Random.Range(0f, 3f);

        StockGraph.CandlestickData newCandle = new StockGraph.CandlestickData
        {
            open = open,
            close = close,
            high = high,
            low = low,
            timestamp = Time.time
        };

        _currentPrice = close;
        _stockData.Add(newCandle);

        if (_stockGraph != null)
        {
            _stockGraph.AddCandle(newCandle);
        }
    }

    public float GetCurrentValue()
    {
        return _currentPrice;
    }

    public void Step()
    {
        GenerateAndAddNewCandle();
    }

    public void GenerateStartData(int count = 50)
    {
        _stockData.Clear();
        _currentPrice = 100f;

        List<StockGraph.CandlestickData> initialData = new();

        for (int i = 0; i < count; i++)
        {
            float randomChange = UnityEngine.Random.Range(-5f, 5f);
            float open = _currentPrice;
            float close = _currentPrice + randomChange;
            float high = Mathf.Max(open, close) + UnityEngine.Random.Range(0f, 3f);
            float low = Mathf.Min(open, close) - UnityEngine.Random.Range(0f, 3f);

            StockGraph.CandlestickData candle = new StockGraph.CandlestickData
            {
                open = open,
                close = close,
                high = high,
                low = low,
                timestamp = i
            };

            _stockData.Add(candle);
            initialData.Add(candle);
            _currentPrice = close;
        }

        if (_stockGraph != null)
        {
            _stockGraph.UpdateGraph(initialData);
        }
    }

    public void UpdateGraph() {
        _stockGraph.UpdateGraph(_stockData);
    }

    public void OpenMenu(Action closeCallback)
    {
        _isOpen = true;
        _player = FindAnyObjectByType<TurnManager>().CurrentPlayer;
        print("Opening stock menu for player: " + _player.playerName);
        _closeCallback = closeCallback;
        _uiStockMenu.gameObject.SetActive(true);
        _uiStockMenu.SetPlayer(_player);
    }

    public void CloseMenu()
    {
        _closeCallback?.Invoke();
        _isOpen = false;
        _player = null;
        _closeCallback = null;
        _uiStockMenu.gameObject.SetActive(false);
    }

    public bool IsOpen {
        get { return _isOpen; }
    }
}
