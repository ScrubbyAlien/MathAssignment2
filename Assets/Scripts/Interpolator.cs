using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MA317G_Assignment2;
using Vectors;

[ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class Interpolator : MonoBehaviour
{
    public bool disableTranslation;
    public bool disableRotation;
    public bool disableScale;
    [SerializeField, Range(0, 1)]
    private float t = 0.5f;
    public TRSMatrix startMatrix;
    [ReadOnly]
    public TRSMatrix interpolatedMatrix;
    public TRSMatrix endMatrix;

    private VectorRenderer vr;

    // Start is called before the first frame update
    void OnEnable() {
        vr = GetComponent<VectorRenderer>();
    }

    // Update is called once per frame
    public void Update() {
        using (vr.Begin()) {
            startMatrix.DrawCoordinateSystem(vr);
            endMatrix.DrawCoordinateSystem(vr);
            interpolatedMatrix = TRSMatrix.Interpolate(
                startMatrix, endMatrix, t,
                disableTranslation, disableRotation, disableScale);
            interpolatedMatrix.DrawCoordinateSystem(vr);
            vr.Draw(startMatrix.origin, endMatrix.origin, Color.white);
        }
    }
}