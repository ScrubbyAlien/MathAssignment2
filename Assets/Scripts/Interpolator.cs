using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MA317G_Assignment2;
using Vectors;

[ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class Interpolator : MonoBehaviour
{
    public TRSMatrix startMatrix;
    public TRSMatrix endMatrix;

    [ReadOnly]
    public TRSMatrix interpolatedMatrix;

    private VectorRenderer vr;

    [SerializeField, Range(0, 1)]
    private float t = 0.5f;

    // Start is called before the first frame update
    void OnEnable() {
        vr = GetComponent<VectorRenderer>();
    }

    // Update is called once per frame
    void Update() {
        using (vr.Begin()) {
            startMatrix.DrawCoordinateSystem(vr);
            endMatrix.DrawCoordinateSystem(vr);
            interpolatedMatrix = TRSMatrix.Interpolate(startMatrix, endMatrix, t);
            interpolatedMatrix.DrawCoordinateSystem(vr);
            vr.Draw(startMatrix.origin, endMatrix.origin, Color.white);
        }
    }
}