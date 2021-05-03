using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    Path path;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        path = (Path)target;

        if (GUILayout.Button("Reset"))
        {
            Debug.Log("reset");
            path.Initalize(path.cursor);
            path.cursor = Vector3.zero;
            path.Selected = path.nodes[0];

        }
        

        if (GUILayout.Button("Bake"))
        {
            path.BakeCrudeDistances();
            path.DebugStart();

        }
        
    }

    private void OnSceneGUI()
    {

        
        path = (Path) target;
        Handles.Label(path.transform.position, "deadly curve: " + path.name);
        
        if (path.Selected != null)
            if(path.Selected.alive)
        {
            //draws circle around selected node
            Handles.color = Color.white;
            Handles.CircleHandleCap(0, path.Selected.Location, Quaternion.LookRotation(Vector3.up), 1f, EventType.Repaint);
            //makes a handle to move the seleced
            path.Selected.Location = Handles.PositionHandle(path.Selected.Location, Quaternion.identity);

            if (path.SelectedCurve != null)
                    if (path.SelectedCurve.alive)
            {
                path.SelectedCurve.Anchor1 = Handles.PositionHandle(path.SelectedCurve.Anchor1, Quaternion.identity);
                path.SelectedCurve.Anchor2 = Handles.PositionHandle(path.SelectedCurve.Anchor2, Quaternion.identity);
            }
            
        }

        path.cursor = Handles.PositionHandle(path.cursor, Quaternion.identity);

        Handles.color = Color.red;
        foreach (Path.BezierCurve curve in path.curves)
        {
            if(curve != null)
                if (curve.alive)
            {
                Handles.CircleHandleCap(0, curve.PointAtProjection(path.cursor), Quaternion.LookRotation(Vector3.up), .2f, EventType.Repaint);

            }
        }

        if (path.SelectedCurve != null)
            if (path.Selected.alive)
        {
            Handles.CircleHandleCap(0, path.ClosestPoint(path.cursor), Quaternion.LookRotation(Vector3.up), .5f, EventType.Repaint);

        }

        HandleKeyboard();

    }

    private void HandleKeyboard()
    {
        Event current = Event.current;
        if (current.type != EventType.KeyDown)
            return;

        switch (current.keyCode)
        {
            case KeyCode.A:
                path.SelectNextCurve();
                current.Use();
                break;
            case KeyCode.D:
                path.SelectNextNode();
                current.Use();
                break;
            case KeyCode.W:
                path.AddSegment(path.Selected, path.Selected.Location + (path.cursor - path.Selected.Location).normalized * 3f, path.cursor, path.cursor + (path.Selected.Location - path.cursor).normalized * 3f);
                path.cursor = Vector3.zero;
                current.Use();
                break;
            case KeyCode.S:
                if (path.MergeSelected == null)
                {
                    path.MergeSelected = path.Selected;
                    path.SelectNextNode();
                }
                else
                {
                    if (path.MergeSelected == path.Selected)
                    {
                        path.MergeSelected = null;
                    }
                    path.AddSegment(path.MergeSelected, path.MergeSelected.Location + (path.Selected.Location - path.MergeSelected.Location).normalized * 3f, path.Selected, path.Selected.Location + (path.MergeSelected.Location - path.Selected.Location).normalized * 3f);
                    path.cursor = Vector3.zero;
                    path.MergeSelected = null;
                }
                current.Use();
                break;
            case KeyCode.R:
                Path.BezierCurve curve = path.SelectedCurve;
                path.SelectNextCurve();
                path.RemoveSegment(curve);
                current.Use();
                break;
        }
    }

}
