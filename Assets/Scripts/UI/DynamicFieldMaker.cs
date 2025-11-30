using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DynamicInputGrid : MonoBehaviour
{
    [Header("Configuration")]
    public int count = 2;
    public GameObject inputFieldPrefab;
    public Transform container;
    public int columns = 4;
    public float cellWidth = 150f;
    public float cellHeight = 40f;
    public float spacingX = 20f;
    public float spacingY = 20f;

    public List<GameObject> CreatedFields = new List<GameObject>();

    private int previousCount = -1;

    void Update()
    {
        if (previousCount != count)
        {
            RebuildGrid();
            previousCount = count;
        }
    }

    public void RebuildGrid()
    {
        foreach (var field in CreatedFields)
            Destroy(field);
        CreatedFields.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject field = Instantiate(inputFieldPrefab, container);
            CreatedFields.Add(field);

            int row = i / 4;
            int col = i % 4;

            field.GetComponent<TMP_InputField>().text = $"Player {i + 1}";
            RectTransform rt = field.GetComponent<RectTransform>();

            float totalWidth = (columns * cellWidth) + ((columns - 1) * spacingX);
            float startX = -totalWidth / 2f + cellWidth / 2f;

            float posX = startX + col * (cellWidth + spacingX);
            float posY = -row * (cellHeight + spacingY);

            rt.anchoredPosition = new Vector2(posX, posY);
        }
    }
    public void OnSliderChanged(float value)
    {
        count = (int)value;
    }
}
