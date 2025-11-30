using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cell))]
public class DrawCellConnections : Editor
{
    void OnSceneGUI()
    {
        Cell cell = (Cell)target;
        if (cell == null || cell.nextCells == null)
            return;

        Handles.color = Color.cyan; // Arrow color

        foreach (Cell next in cell.nextCells)
        {
            if (next == null) continue;

            Vector3 start = cell.transform.position;
            Vector3 end = next.transform.position;

            // Draw the line
            Handles.DrawLine(start, end);

            // Draw arrow head
            DrawArrow(end, (end - start).normalized);
        }
    }

    void DrawArrow(Vector3 position, Vector3 direction)
    {
        // Arrow head size
        float size = HandleUtility.GetHandleSize(position) * 0.2f;

        Handles.ConeHandleCap(
            0,
            position - direction * size,
            Quaternion.LookRotation(direction),
            size,
            EventType.Repaint
        );
    }
}
