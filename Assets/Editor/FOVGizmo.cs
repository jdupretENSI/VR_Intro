using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyController))]
public class FOVGizmo : Editor
{
    private void OnSceneGUI()
    {
        EnemyController fov = (EnemyController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov._radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov._angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov._angle / 2);

        Handles.color = Color.red;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov._radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov._radius);

        // if (fov.canSeePlayer)
        // {
        //     Handles.color = Color.green;
        //     Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        // }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
