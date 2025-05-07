using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MA317G_Assignment2;
using Vectors;

[ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class Interpolator : MonoBehaviour
{
    [SerializeField]
    private TRSMatrix startMatrix;
    [SerializeField]
    private TRSMatrix endMatrix;

    private VectorRenderer vr;

    [SerializeField, Range(0, 1)]
    private float t = 0.5f;

    private TRSMatrix interpolatedMatrix {
        get { return TRSMatrix.Interpolate(startMatrix, endMatrix, t); }
    }

    // Start is called before the first frame update
    void OnEnable() {
        vr = GetComponent<VectorRenderer>();
    }

    // Update is called once per frame
    void Update() {
        using (vr.Begin()) {
            startMatrix.DrawCoordinateSystem(vr);
            endMatrix.DrawCoordinateSystem(vr);
            interpolatedMatrix.DrawCoordinateSystem(vr);
        }
    }
}