using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    private static StockManager _instance;

    [SerializeField] private StockGraph _stockGraph;

    private List<StockGraph.CandlestickData> _stockData = new();
    private float _currentPrice = 100f;
    private float _updateTimer = 0f;
    private float _updateInterval = 0.5f;

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
        GenerateStartData(50);
    }

    void Update()
    {
        _updateTimer += Time.deltaTime;
        if (_updateTimer >= _updateInterval)
        {
            _updateTimer = 0f;
            Step();
        }
    }
    
    private void GenerateAndAddNewCandle()
    {
        float randomChange = Random.Range(-5f, 5f);
        float open = _currentPrice;
        float close = _currentPrice + randomChange;
        float high = Mathf.Max(open, close) + Random.Range(0f, 3f);
        float low = Mathf.Min(open, close) - Random.Range(0f, 3f);

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

    // Public method to get the current stock value
    public float GetCurrentValue()
    {
        return _currentPrice;
    }

    // Public method to generate one new value and update the graph
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
            float randomChange = Random.Range(-5f, 5f);
            float open = _currentPrice;
            float close = _currentPrice + randomChange;
            float high = Mathf.Max(open, close) + Random.Range(0f, 3f);
            float low = Mathf.Min(open, close) - Random.Range(0f, 3f);

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
}
