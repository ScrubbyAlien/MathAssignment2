using MA317G_Assignment2;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[EditorTool("Matrix Manipulator Tool", typeof(Interpolator))]
public class MatrixManipulatorTool : EditorTool
{
    [Shortcut("Activate Matrix Manipulator Tool", KeyCode.U)]
    static void PathManipulatorToolShortcut() {
        if (Selection.GetFiltered<Interpolator>(SelectionMode.TopLevel).Length > 0) {
            ToolManager.SetActiveTool<MatrixManipulatorTool>();
        }
    }

    public override void OnToolGUI(EditorWindow window) {
        if (!(window is SceneView)) return;

        foreach (var obj in targets) {
            if (!(obj is Interpolator interpolator)) continue;

            TRSMatrix start = interpolator.startMatrix;
            Vector3 startPosition = start.GetTranslation();
            Quaternion startRotation = start.GetRotation();
            Vector3 startScale = start.GetScale();

            TRSMatrix end = interpolator.endMatrix;
            Vector3 endPosition = end.GetTranslation();
            Quaternion endRotation = end.GetRotation();
            Vector3 endScale = end.GetScale();

            EditorGUI.BeginChangeCheck();

            Handles.TransformHandle(ref startPosition, ref startRotation, ref startScale);
            Handles.TransformHandle(ref endPosition, ref endRotation, ref endScale);

            // this prevents funky behaviour when doing certain rotations around the cardinal axis with the tool
            // because TransformHandle seems to return two different rotations after rotating 180 deg around a cardinal axis
            // I don't know why but the best solution I've come up with is below
            // This doesn't solve the issue completely but at least it prevents several cubes from being drawn simultaneaously
            // For now when rotating around a cardinal axis you can only rotate a maximum of 180 degrees
            // But rotating around noncardinal axis seems to work just fine

            bool startxyisZero = Mathf.Approximately(startRotation.x, 0) && Mathf.Approximately(startRotation.y, 0);
            bool startyzisZero = Mathf.Approximately(startRotation.y, 0) && Mathf.Approximately(startRotation.z, 0);
            bool startzxisZero = Mathf.Approximately(startRotation.z, 0) && Mathf.Approximately(startRotation.x, 0);

            bool endxyisZero = Mathf.Approximately(endRotation.x, 0) && Mathf.Approximately(endRotation.y, 0);
            bool endyzisZero = Mathf.Approximately(endRotation.y, 0) && Mathf.Approximately(endRotation.z, 0);
            bool endzxisZero = Mathf.Approximately(endRotation.z, 0) && Mathf.Approximately(endRotation.x, 0);

            if (startxyisZero && startRotation.z * startRotation.w < 0) startRotation = startRotation.Conjugate();
            if (startyzisZero && startRotation.x * startRotation.w < 0) startRotation = startRotation.Conjugate();
            if (startzxisZero && startRotation.y * startRotation.w < 0) startRotation = startRotation.Conjugate();

            if (endxyisZero && endRotation.z * endRotation.w < 0) endRotation = endRotation.Conjugate();
            if (endyzisZero && endRotation.x * endRotation.w < 0) endRotation = endRotation.Conjugate();
            if (endzxisZero && endRotation.y * endRotation.w < 0) endRotation = endRotation.Conjugate();

            // set the new matrises
            interpolator.startMatrix = TRSMatrix.TRS(startPosition, startRotation, startScale);
            interpolator.endMatrix = TRSMatrix.TRS(endPosition, endRotation, endScale);

            // call update immediately for smooth feedback
            interpolator.Update();

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(interpolator, "Interpolator");
            }
        }
    }
}