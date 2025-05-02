using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectors;
using MA317G_Assignment2;

[ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class VectorCube : MonoBehaviour
{
    private TRSMatrix _transform = TRSMatrix.identity;
    public TRSMatrix transform {
        get => _transform;
        set {
            _transform = value;
            UpdatePoints();
        }
    }

    [HideInInspector]
    public Vector4
        point000,
        point100,
        point010,
        point110,
        point001,
        point101,
        point011,
        point111;

    private VectorRenderer vectorRenderer;

    private void OnEnable() {
        vectorRenderer = GetComponent<VectorRenderer>();

        UpdatePoints();
    }

    void Update() {
        UpdatePoints();
        DrawVectors();
    }

    private void DrawVectors() {
        using (vectorRenderer.Begin()) {
            // draw all x-aligned axis
            vectorRenderer.Draw(point000, point100, Color.red);
            vectorRenderer.Draw(point010, point110, Color.red);
            vectorRenderer.Draw(point001, point101, Color.red);
            vectorRenderer.Draw(point011, point111, Color.red);

            // draw all y-aligned axis
            vectorRenderer.Draw(point000, point010, Color.green);
            vectorRenderer.Draw(point100, point110, Color.green);
            vectorRenderer.Draw(point001, point011, Color.green);
            vectorRenderer.Draw(point101, point111, Color.green);

            // draw all z-aligned axis
            vectorRenderer.Draw(point000, point001, Color.blue);
            vectorRenderer.Draw(point100, point101, Color.blue);
            vectorRenderer.Draw(point010, point011, Color.blue);
            vectorRenderer.Draw(point110, point111, Color.blue);
        }
    }

    private void UpdatePoints() {
        point000 = transform.MultiplyPoint(new Vector4(0, 0, 0, 1));
        point100 = transform.MultiplyPoint(new Vector4(1, 0, 0, 1));
        point010 = transform.MultiplyPoint(new Vector4(0, 1, 0, 1));
        point001 = transform.MultiplyPoint(new Vector4(0, 0, 1, 1));
        point110 = transform.MultiplyPoint(new Vector4(1, 1, 0, 1));
        point101 = transform.MultiplyPoint(new Vector4(1, 0, 1, 1));
        point011 = transform.MultiplyPoint(new Vector4(0, 1, 1, 1));
        point111 = transform.MultiplyPoint(new Vector4(1, 1, 1, 1));
    }
}