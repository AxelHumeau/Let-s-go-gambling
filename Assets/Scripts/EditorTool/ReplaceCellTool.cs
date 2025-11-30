using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ReplaceCellWithPrefabEditor : EditorWindow
{
    GameObject baseObject;   // the object to replace
    GameObject prefab;       // prefab to instantiate

    [MenuItem("Tools/Replace Cell With Prefab")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceCellWithPrefabEditor>("Replace Cell With Prefab");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Cell Object", EditorStyles.boldLabel);

        baseObject = (GameObject)EditorGUILayout.ObjectField("Base Object", baseObject, typeof(GameObject), true);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        GUI.enabled = baseObject != null && prefab != null;

        if (GUILayout.Button("Replace"))
        {
            ReplaceObject();
        }

        GUI.enabled = true;
    }

    void ReplaceObject()
    {
        if (baseObject == null || prefab == null)
            return;

        // Get base Cell component
        Cell baseCell = baseObject.GetComponent<Cell>();
        if (baseCell == null)
        {
            Debug.LogError("Base object does not have a Cell component!");
            return;
        }

        // Instantiate prefab
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, baseObject.transform.parent);
        Undo.RegisterCreatedObjectUndo(newObj, "Replace Cell");

        // Copy transform
        newObj.transform.position = baseObject.transform.position;
        newObj.transform.rotation = baseObject.transform.rotation;
        newObj.transform.localScale = baseObject.transform.localScale;

        // Copy Cell links
        Cell newCell = newObj.GetComponent<Cell>();
        if (newCell == null)
        {
            Debug.LogError("Prefab does not have a Cell component!");
            return;
        }

        // Copy previous and next connections
        newCell.previousCells = new List<Cell>(baseCell.previousCells);
        newCell.nextCells = new List<Cell>(baseCell.nextCells);

        // Update references in previousCells
        foreach (Cell prev in newCell.previousCells)
        {
            if (prev.nextCells.Contains(baseCell))
            {
                prev.nextCells.Remove(baseCell);
                prev.nextCells.Add(newCell);
            }
        }

        // Update references in nextCells
        foreach (Cell next in newCell.nextCells)
        {
            if (next.previousCells.Contains(baseCell))
            {
                next.previousCells.Remove(baseCell);
                next.previousCells.Add(newCell);
            }
        }

        // Delete original
        Undo.DestroyObjectImmediate(baseObject);

        Debug.Log("Replaced base object with prefab and preserved Cell links.");
    }
}
