using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cell))]
public class DrawCellConnections : Editor
{
    void OnSceneGUI()
    {
        Cell cell = (Cell)target;
        if (cell == null) return;

        // Draw arrows toward next cells (cyan)
        if (cell.nextCells != null)
        {
            Handles.color = Color.cyan;
            foreach (Cell next in cell.nextCells)
            {
                if (next == null) continue;

                Vector3 start = cell.transform.position;
                Vector3 end = next.transform.position;

                Handles.DrawLine(start, end);
                DrawArrow(end, (end - start).normalized);
            }
        }

        // Draw arrows toward previous cells (red)
        if (cell.previousCells != null)
        {
            Handles.color = Color.red;
            foreach (Cell prev in cell.previousCells)
            {
                if (prev == null) continue;

                Vector3 start = prev.transform.position;
                Vector3 end = cell.transform.position;

                Handles.DrawLine(start, end);
                DrawArrow(end, (end - start).normalized);
            }
        }
    }

    void DrawArrow(Vector3 position, Vector3 direction)
    {
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
