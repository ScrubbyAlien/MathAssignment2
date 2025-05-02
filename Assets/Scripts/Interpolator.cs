using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MA317G_Assignment2;

[ExecuteAlways]
public class Interpolator : MonoBehaviour
{
    [SerializeField]
    private TRSMatrix startMatrix;
    [SerializeField]
    private TRSMatrix endMatrix;

    [SerializeField, Range(0, 1)]
    private float t = 0.5f;

    private TRSMatrix interpolatedMatrix {
        get { return TRSMatrix.Interpolate(startMatrix, endMatrix, t); }
    }

    [SerializeField]
    private VectorCube startCube;
    [SerializeField]
    private VectorCube endCube;
    [SerializeField]
    private VectorCube interpolatedCube;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        startCube.transform = startMatrix;
        endCube.transform = endMatrix;
        interpolatedCube.transform = interpolatedMatrix;
    }
}