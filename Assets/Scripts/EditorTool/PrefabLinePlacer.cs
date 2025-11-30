using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CellLinePlacer : EditorWindow
{
    private GameObject prefab;
    private int count = 5;

    private Vector3 startPosition = Vector3.zero;
    private Vector3 direction = Vector3.right;
    private float spacing = 2f;

    [MenuItem("Tools/Cell Line Placer Tool")]
    public static void ShowWindow()
    {
        GetWindow<CellLinePlacer>("Cell Line Placer Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cell Line Placer Tool", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        count = EditorGUILayout.IntField("Count", count);

        startPosition = EditorGUILayout.Vector3Field("Start Position", startPosition);
        direction = EditorGUILayout.Vector3Field("Direction", direction);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);

        GUILayout.Space(10);

        if (GUILayout.Button("Place Prefabs"))
        {
            PlacePrefabs();
        }
    }

    void PlacePrefabs()
    {
        if (prefab == null)
        {
            Debug.LogError("No prefab assigned!");
            return;
        }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Place Prefabs");

        Vector3 dir = direction.normalized;

        // store references to spawned cells
        List<Cell> spawnedCells = new List<Cell>();

        // --- Create cells ---
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = startPosition + dir * (i * spacing);

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.name = prefab.name + "_" + i;
            Undo.RegisterCreatedObjectUndo(obj, "Place Prefab");
            obj.transform.position = pos;

            Cell cell = obj.GetComponent<Cell>();
            if (cell == null)
            {
                Debug.LogError("Prefab has no Cell component!");
                return;
            }

            // clear any existing links
            cell.nextCells = new List<Cell>();
            cell.previousCells = new List<Cell>();

            spawnedCells.Add(cell);
        }

        // --- Link cells ---
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            Cell current = spawnedCells[i];

            // previous
            if (i > 0)
            {
                Cell prev = spawnedCells[i - 1];
                current.previousCells.Add(prev);
                prev.nextCells.Add(current);
            }

            // no else necessary; first cell simply has no previous
        }

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

        Debug.Log("Placed and linked " + count + " cells.");
    }
}
