using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class StockGraph : MonoBehaviour
{
    [System.Serializable]
    public class CandlestickData
    {
        public float open;
        public float close;
        public float high;
        public float low;
        public float timestamp;
    }

    private RectTransform _graphContainer;
    [SerializeField] private Color _bullishColor = Color.green;
    [SerializeField] private Color _bearishColor = Color.red;
    [SerializeField] private float _candleSpacing = 5f;
    [SerializeField] private int _maxVisibleCandles = 50;
    [SerializeField] private int _priceScaleSteps = 5;
    [SerializeField] private Color _scaleTextColor = Color.white;
    [SerializeField] private int _scaleFontSize = 12;

    private List<CandlestickData> _stockData = new();
    private List<GameObject> _visualObjects = new();

    [SerializeField] private RectTransform _valueBanner;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _playerValueText;

    [SerializeField] private Image _stonk;
    [SerializeField] private Sprite _stonksSprite;
    [SerializeField] private Sprite _notstonksSprite;

    private void Awake()
    {
        _graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
    }

    public void AddCandle(CandlestickData candle)
    {
        _stockData.Add(candle);
        DrawGraph();
        UpdateValueBanner();
    }

    public void UpdateGraph(List<CandlestickData> data)
    {
        _stockData = new List<CandlestickData>(data);
        DrawGraph();
        UpdateValueBanner();
    }

    private void ClearGraph()
    {
        foreach (GameObject obj in _visualObjects)
        {
            Destroy(obj);
        }
        _visualObjects.Clear();
    }

    private void DrawGraph()
    {
        if (_stockData.Count == 0) return;

        ClearGraph();

        int startIndex = Mathf.Max(0, _stockData.Count - _maxVisibleCandles);
        int count = Mathf.Min(_maxVisibleCandles, _stockData.Count);
        List<CandlestickData> visibleCandles = _stockData.GetRange(startIndex, count);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        foreach (var candle in visibleCandles)
        {
            if (candle.low < minValue) minValue = candle.low;
            if (candle.high > maxValue) maxValue = candle.high;
        }

        float graphHeight = _graphContainer.rect.height;
        float graphWidth = _graphContainer.rect.width;
        float valueRange = maxValue - minValue;

        float labelWidth = 65f;
        float verticalPadding = 10f;
        float topPadding = 5f;
        float usableHeight = graphHeight - verticalPadding - topPadding;
        float chartAreaWidth = graphWidth - labelWidth;
        float chartStartX = labelWidth;

        float totalSpacing = _candleSpacing * (visibleCandles.Count - 1);
        float availableWidth = chartAreaWidth - totalSpacing;
        float optimalCandleWidth = availableWidth / visibleCandles.Count;

        DrawPriceScale(minValue, maxValue, usableHeight, labelWidth, chartAreaWidth, verticalPadding);

        for (int i = 0; i < visibleCandles.Count; i++)
        {
            DrawCandlestick(visibleCandles[i], i, minValue, valueRange, usableHeight, optimalCandleWidth, chartStartX, verticalPadding);
        }
    }

    private void DrawCandlestick(CandlestickData data, int index, float minValue, float valueRange, float graphHeight, float candleWidth, float xOffset, float yOffset)
    {
        float xPosition = xOffset + index * (candleWidth + _candleSpacing);

        bool isBullish = data.close >= data.open;
        Color candleColor = isBullish ? _bullishColor : _bearishColor;

        GameObject wick = CreateLine(
            new Vector2(xPosition + candleWidth / 2, yOffset + GetYPosition(data.low, minValue, valueRange, graphHeight)),
            new Vector2(xPosition + candleWidth / 2, yOffset + GetYPosition(data.high, minValue, valueRange, graphHeight)),
            2f,
            candleColor
        );
        wick.name = $"Wick_{index}";
        _visualObjects.Add(wick);

        float bodyBottom = isBullish ? data.open : data.close;
        float bodyTop = isBullish ? data.close : data.open;
        float bodyHeight = Mathf.Abs(GetYPosition(bodyTop, minValue, valueRange, graphHeight) -
                                      GetYPosition(bodyBottom, minValue, valueRange, graphHeight));

        if (bodyHeight < 1f) bodyHeight = 1f;

        GameObject body = CreateRectangle(
            new Vector2(xPosition, yOffset + GetYPosition(bodyBottom, minValue, valueRange, graphHeight)),
            new Vector2(candleWidth, bodyHeight),
            candleColor
        );
        body.name = $"Body_{index}";
        _visualObjects.Add(body);
    }

    private float GetYPosition(float value, float minValue, float valueRange, float graphHeight)
    {
        if (valueRange == 0) return graphHeight / 2;
        return (value - minValue) / valueRange * graphHeight;
    }

    private GameObject CreateLine(Vector2 start, Vector2 end, float thickness, Color color)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(_graphContainer, false);

        Image image = lineObj.AddComponent<Image>();
        image.color = color;
        image.sprite = CreateLineSprite((int)thickness);

        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();

        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(thickness, distance);
        rectTransform.anchoredPosition = start + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90);

        return lineObj;
    }

    private GameObject CreateRectangle(Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject rectObj = new GameObject("Rectangle");
        rectObj.transform.SetParent(_graphContainer, false);

        Image image = rectObj.AddComponent<Image>();
        image.color = color;
        image.sprite = CreateRectangleSprite((int)size.x, (int)size.y);

        RectTransform rectTransform = rectObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
        rectTransform.pivot = new Vector2(0, 0);

        return rectObj;
    }

    private Sprite CreateLineSprite(int thickness)
    {
        int width = Mathf.Max(1, thickness);
        int height = 1;

        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateRectangleSprite(int width, int height)
    {
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);

        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private void DrawPriceScale(float minValue, float maxValue, float graphHeight, float labelWidth, float chartAreaWidth, float yOffset)
    {
        float valueRange = maxValue - minValue;
        if (valueRange == 0) return;

        float chartStartX = labelWidth;

        for (int i = 0; i <= _priceScaleSteps; i++)
        {
            float normalizedPosition = (float)i / _priceScaleSteps;
            float priceValue = minValue + (valueRange * normalizedPosition);
            float yPosition = yOffset + (normalizedPosition * graphHeight);

            GameObject line = CreateLine(
                new Vector2(chartStartX, yPosition),
                new Vector2(chartStartX + chartAreaWidth, yPosition),
                1f,
                new Color(1f, 1f, 1f, 0.2f)
            );
            line.name = $"ScaleLine_{i}";
            _visualObjects.Add(line);

            GameObject labelObj = new($"ScaleLabel_{i}");
            labelObj.transform.SetParent(_graphContainer, false);

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = priceValue.ToString("F2");
            label.fontSize = _scaleFontSize;
            label.color = _scaleTextColor;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.fontStyle = FontStyles.Bold;

            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0, 0);
            labelRect.pivot = new Vector2(0, 0.5f);
            labelRect.anchoredPosition = new Vector2(5, yPosition);
            labelRect.sizeDelta = new Vector2(labelWidth - 10, 20);

            _visualObjects.Add(labelObj);
        }
    }

    private void UpdateValueBanner()
    {
        if (_valueText != null && _stockData.Count > 0)
        {
            CandlestickData lastCandle = _stockData[_stockData.Count - 1];
            float change = lastCandle.close - lastCandle.open;
            float changePercent = (change / lastCandle.open) * 100f;
            string sign = change >= 0 ? "+" : "";
            Color valueColor = change >= 0 ? _bullishColor : _bearishColor;

            _valueText.text = $"${lastCandle.close:F2} ({sign}{changePercent:F2}%)";
            _valueText.color = valueColor;

            if (_stonk != null)
            {
                if (change >= 0 && _stonksSprite != null)
                {
                    _stonk.sprite = _stonksSprite;
                }
                else if (change < 0 && _notstonksSprite != null)
                {
                    _stonk.sprite = _notstonksSprite;
                }
            }
        }
        if (_playerValueText != null)
        {
            int stocksOwned = FindAnyObjectByType<TurnManager>().CurrentPlayer.GetStocksOwned();
            float stocksOwnedValue = StockManager.Instance.GetCurrentValue() * stocksOwned;
            Color valueColor = stocksOwnedValue >= 0 ? _bullishColor : _bearishColor;
            _playerValueText.text = $"{stocksOwned} Stocks (${stocksOwnedValue:F2})";
            _playerValueText.color = valueColor;
        }
    }
}

