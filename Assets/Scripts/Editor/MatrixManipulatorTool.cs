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
            
            interpolator.startMatrix = TRSMatrix.TRS(startPosition, startRotation, startScale);
            interpolator.endMatrix = TRSMatrix.TRS(endPosition, endRotation, endScale);
            
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(interpolator, "Interpolator");
            }
        }
    }
}