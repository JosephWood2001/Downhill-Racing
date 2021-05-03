using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(RacingLine))]
public class RacingLineEditor : Editor
{
    RacingLine racingLine;
    Vector3 debugPoint = Vector3.zero;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        racingLine = (RacingLine)target;

        if (GUILayout.Button("Add Point"))
        {
            racingLine.AddPoint(Vector3.zero);
            
        }

        if (GUILayout.Button("BakeDistance"))
        {
            racingLine.BakeDistance();

        }

        if (GUILayout.Button("Test Distance At Debug Point"))
        {
            Debug.Log( racingLine.FindDistanceAlongRaceLine(debugPoint));

        }

        if (GUILayout.Button("Test Unit Distance At Debug Point ( and bake )"))
        {
            racingLine.BakeDistance();
            Debug.Log(racingLine.GetUnitDistanceAlongCurve(debugPoint));

        }
    }

    private void OnSceneGUI()
    {
        racingLine = (RacingLine)target;
        Handles.Label(racingLine.transform.position, "deadly curve: " + racingLine.name);
        for (int i = 0; i < racingLine.lineSegments.Length; i++)
        {
            //Gizmos.DrawSphere(lineSegments[i].point1, .1f);
            racingLine.MovePoint(i, Handles.PositionHandle(racingLine.lineSegments[i].point1, Quaternion.identity));
        }
        racingLine.MovePoint(racingLine.lineSegments.Length, Handles.PositionHandle(racingLine.lineSegments[racingLine.lineSegments.Length-1].point2, Quaternion.identity));

        debugPoint = Handles.PositionHandle(debugPoint, Quaternion.identity);

    }
}
