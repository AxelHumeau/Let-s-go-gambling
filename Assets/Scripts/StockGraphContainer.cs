using System;
using UnityEngine;
using UnityEngine.UI;

public class StockGraphContainer : MonoBehaviour
{

    [SerializeField] private Button _minimizeButton;
    [SerializeField] private RectTransform _graphContainer;
    [SerializeField] private CanvasGroup _graphCanvasGroup;

    private bool _isMinimized = false;

    void Awake()
    {
        _minimizeButton.onClick.AddListener(ToggleMinimize);

        // If no CanvasGroup is assigned, try to get or add one
        if (_graphCanvasGroup == null && _graphContainer != null)
        {
            _graphCanvasGroup = _graphContainer.GetComponent<CanvasGroup>();
            if (_graphCanvasGroup == null)
            {
                _graphCanvasGroup = _graphContainer.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    private void ToggleMinimize()
    {
        if (_graphCanvasGroup == null) return;

        _isMinimized = !_isMinimized;

        _graphCanvasGroup.alpha = _isMinimized ? 0f : 1f;
        _graphCanvasGroup.interactable = !_isMinimized;
        _graphCanvasGroup.blocksRaycasts = !_isMinimized;
    }
}
