using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Vectors;

namespace MA317G_Assignment2
{
    [System.Serializable]
    public class TRSMatrix
    {
        private const int SIZE = 4;
        [SerializeField]
        private float[] matrixArray;

        private TRSMatrix() {
            matrixArray = new float[SIZE * SIZE];
        }

        #region Properties

        /// <summary>
        /// Return the element in the matrix at the given position
        /// </summary>
        /// <param name="r">The row of the element</param>
        /// <param name="c">The column of the element</param>
        public float this[int r, int c] {
            get { return matrixArray[r * SIZE + c]; }
            set { matrixArray[r * SIZE + c] = value; }
        }
        public static TRSMatrix identity {
            get {
                TRSMatrix I = new TRSMatrix();
                for (int i = 0; i < SIZE; i++) {
                    I[i, i] = 1;
                }
                return I;
            }
        }

        public float determinant => CalculateDeterminant4x4(matrixArray);

        public Vector3 origin {
            get { return MultiplyPoint(new Vector3(0, 0, 0)); }
        }

        #endregion

        #region General Public Matrix Methods

        public Vector4 GetColumn(int index) {
            Vector4 column = new Vector4();
            for (int i = 0; i < SIZE; i++) column[i] = this[i, index];
            return column;
        }

        public void SetColumn(int index, Vector4 column) {
            for (int i = 0; i < SIZE; i++) this[i, index] = column[i];
        }

        public Vector4 GetRow(int index) {
            Vector4 row = new Vector4();
            for (int i = 0; i < SIZE; i++) row[i] = this[index, i];
            return row;
        }

        public void SetRow(int index, Vector4 row) {
            for (int i = 0; i < SIZE; i++) this[index, i] = row[i];
        }

        public static TRSMatrix operator *(TRSMatrix lhs, TRSMatrix rhs) {
            TRSMatrix product = new TRSMatrix();

            for (int i = 0; i < SIZE; i++) {
                for (int j = 0; j < SIZE; j++) {
                    product[i, j] = lhs.GetRow(i).Dot(rhs.GetColumn(j));
                }
            }

            return product;
        }

        public static Vector4 operator *(TRSMatrix lhs, Vector4 rhs) {
            Vector4 transformedVector = new Vector4();
            for (int r = 0; r < SIZE; r++) {
                transformedVector[r] = lhs.GetRow(r).Dot(rhs);
            }
            return transformedVector;
        }

        public static bool operator ==(TRSMatrix lhs, TRSMatrix rhs) {
            for (int r = 0; r < SIZE; r++) {
                for (int c = 0; c < SIZE; c++) {
                    if (lhs[r, c] != rhs[r, c]) return false;
                }
            }
            return true;
        }

        public static bool operator !=(TRSMatrix lhs, TRSMatrix rhs) {
            return !(lhs == rhs);
        }

        #region To avoid warnings from implementing == and != operators

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override bool Equals(object o) {
            if (!(o is TRSMatrix)) {
                return false;
            }
            else {
                return this == (TRSMatrix)o;
            }
        }

        #endregion

        public static TRSMatrix Copy(TRSMatrix original) {
            TRSMatrix copy = new TRSMatrix();
            for (int r = 0; r < SIZE; r++) {
                for (int c = 0; c < SIZE; c++) {
                    copy[r, c] = original[r, c];
                }
            }
            return copy;
        }

        /// <summary>
        /// Transforms a position by this matrix. From coordinates relative to this matrix, to world space coordinates.
        /// </summary>
        /// <param name="point">A position in the coordinate space of this matrix.</param>
        /// <returns>The world space coordinates of the given position.</returns>
        public Vector3 MultiplyPoint(Vector3 point) {
            return this * new Vector4(point.x, point.y, point.z, 1);
        }

        public Vector3 GetBasisVector(int index) {
            return GetColumn(index);
        }

        #endregion

        #region TRS specific Public Methods

        public Vector3 GetTranslation() {
            // implicit Vector3 conversion
            return GetColumn(3);
        }

        public Quaternion GetRotation() {
            // convert rotation of the matrix into 
            return RotationBetween(identity, this);
        }

        public Vector3 GetScale() {
            // return the length of each basis vector of the matrix, assuming the basis vectors are orthogonal
            float xScale = GetColumn(0).Magnitude();
            float yScale = GetColumn(1).Magnitude();
            float zScale = GetColumn(2).Magnitude();
            return new Vector3(xScale, yScale, zScale);
        }

        public void DrawCoordinateSystem(VectorRenderer vr) {
            // visualize the coordinate system represented by this matrix
            Vector4 point000 = MultiplyPoint(new Vector3(0, 0, 0));
            Vector4 point100 = MultiplyPoint(new Vector3(1, 0, 0));
            Vector4 point010 = MultiplyPoint(new Vector3(0, 1, 0));
            Vector4 point001 = MultiplyPoint(new Vector3(0, 0, 1));
            Vector4 point110 = MultiplyPoint(new Vector3(1, 1, 0));
            Vector4 point101 = MultiplyPoint(new Vector3(1, 0, 1));
            Vector4 point011 = MultiplyPoint(new Vector3(0, 1, 1));
            Vector4 point111 = MultiplyPoint(new Vector3(1, 1, 1));

            // draw all x-aligned edges
            vr.Draw(point000, point100, Color.red);
            vr.Draw(point010, point110, Color.red);
            vr.Draw(point001, point101, Color.red);
            vr.Draw(point011, point111, Color.red);

            // draw all y-aligned edges
            vr.Draw(point000, point010, Color.green);
            vr.Draw(point100, point110, Color.green);
            vr.Draw(point001, point011, Color.green);
            vr.Draw(point101, point111, Color.green);

            // draw all z-aligned edges
            vr.Draw(point000, point001, Color.blue);
            vr.Draw(point100, point101, Color.blue);
            vr.Draw(point010, point011, Color.blue);
            vr.Draw(point110, point111, Color.blue);
        }

        #endregion

        #region TRS specific Static methods

        private static TRSMatrix FromTranslation(Vector3 translation) {
            TRSMatrix translationMatrix = identity;
            translationMatrix[0, 3] = translation.x;
            translationMatrix[1, 3] = translation.y;
            translationMatrix[2, 3] = translation.z;
            translationMatrix[3, 3] = 1;
            return translationMatrix;
        }

        private static TRSMatrix FromRotation(Quaternion q) {
            #region define premultiplied variables

            float w = q.w;
            float x = q.x;
            float y = q.y;
            float z = q.z;

            float x2 = x * x;
            float y2 = y * y;
            float z2 = z * z;
            float xy = x * y;
            float yz = y * z;
            float zx = z * x;
            float wx = w * x;
            float wy = w * y;
            float wz = w * z;

            #endregion

            Vector4 row0 = new Vector4(1 - 2 * (y2 + z2), 2 * (xy - wz), 2 * (zx + wy), 0);
            Vector4 row1 = new Vector4(2 * (xy + wz), 1 - 2 * (z2 + x2), 2 * (yz - wx), 0);
            Vector4 row2 = new Vector4(2 * (zx - wy), 2 * (yz + wx), 1 - 2 * (x2 + y2), 0);

            TRSMatrix rotationMatrix = identity;
            rotationMatrix.SetRow(0, row0);
            rotationMatrix.SetRow(1, row1);
            rotationMatrix.SetRow(2, row2);

            return rotationMatrix;
        }

        private static TRSMatrix FromScale(Vector3 scale) {
            TRSMatrix scalingMatrix = identity;
            scalingMatrix[0, 0] = scale.x;
            scalingMatrix[1, 1] = scale.y;
            scalingMatrix[2, 2] = scale.z;
            return scalingMatrix;
        }

        /// <summary>
        /// Creates a new TRSMatrix from the given translation, rotation and scale
        /// </summary>
        /// <param name="translation">The position of the origin of the TRSMatrix coordinate space, relative to world space.</param>
        /// <param name="rotation">The local rotation of the TRSMatrix. Rotated around the origin of the TRSMatrix coordinate space.</param>
        /// <param name="scale">The local scale of the TRSMatrix. Scaled relative to the rotation of the TRSMatrix, not world space.</param>
        /// <returns></returns>
        public static TRSMatrix TRS(Vector3 translation, Quaternion rotation, Vector3 scale) {
            return FromTranslation(translation) * FromRotation(rotation) * FromScale(scale);
        }

        public static Vector3 LerpPosition(TRSMatrix A, TRSMatrix B, float t) {
            Vector3 translationA = A.GetTranslation();
            Vector3 translationB = B.GetTranslation();
            Vector3 difference = translationB - translationA;
            return translationA + difference * t;
        }

        // public static Quaternion SlerpRotation(TRSMatrix A, TRSMatrix B, float t) {
        //     // slerp computation from chapter 8.5.12
        //     Quaternion q0 = A.GetRotation();
        //     Quaternion q1 = B.GetRotation();
        //
        //     // get angle between them with dot product
        //     float cosOmega = q0.Dot(q1);
        //
        //     // invert of cosine is negative
        //     if (cosOmega < 0) {
        //         q1 = q1.Negate();
        //         cosOmega = -cosOmega;
        //     }
        //
        //     float k0, k1;
        //     // if orientations are very close use lerp
        //     if (cosOmega > 0.9999f) {
        //         k0 = 1f - t;
        //         k1 = t;
        //     }
        //     else { // compute slerp
        //         // compute sin with trig identity sin2 + cos2 = 1
        //         float sinOmega = Mathf.Sqrt(1 - (cosOmega * cosOmega));
        //         float omega = Mathf.Atan2(sinOmega, cosOmega);
        //         float omegaInverse = 1f / omega;
        //         k0 = Mathf.Sin((1f - t) * omega) * omegaInverse;
        //         k1 = Mathf.Sin(t * omega) * omegaInverse;
        //     }
        //
        //     // interpolate with k0 and k1
        //     float w = q0.w * k0 + q1.w * k1;
        //     float x = q0.x * k0 + q1.x * k1;
        //     float y = q0.y * k0 + q1.y * k1;
        //     float z = q0.z * k0 + q1.z * k1;
        //     return new Quaternion(x, y, z, w);
        // }

        public static Quaternion SlerpRotation(TRSMatrix A, TRSMatrix B, float t) {
            Quaternion q1 = A.GetRotation();
            Quaternion q2 = B.GetRotation();
            Quaternion betweenAB = q2 * q1.Conjugate();

            // return the identity if A and B have same orientation
            if (Mathf.Abs(betweenAB.w) > 0.9999f) return Quaternion.identity;

            float halfAngle = Mathf.Acos(betweenAB.w);
            float interpolatedAngle = halfAngle * t;
            float wNew = Mathf.Cos(interpolatedAngle);

            float vectorScale = Mathf.Sin(interpolatedAngle) / Mathf.Sin(halfAngle);
            float xNew = betweenAB.x * vectorScale;
            float yNew = betweenAB.y * vectorScale;
            float zNew = betweenAB.z * vectorScale;

            // Debug.Log(vectorScale);

            return new Quaternion(xNew, yNew, zNew, wNew) * A.GetRotation();
        }

        public static Vector3 LerpScale(TRSMatrix A, TRSMatrix B, float t) {
            Vector3 scaleA = A.GetScale();
            Vector3 scaleB = B.GetScale();
            Vector3 difference = scaleB - scaleA;
            return scaleA + difference * t;
        }

        #endregion

        public static TRSMatrix Interpolate(
            TRSMatrix lhs,
            TRSMatrix rhs,
            float t,
            bool translate,
            bool rotate,
            bool scale
        ) {
            Vector3 lerpedTranslation = LerpPosition(lhs, rhs, t);
            Quaternion slerpedRotation = SlerpRotation(lhs, rhs, t);
            Vector3 lerpedScale = LerpScale(lhs, rhs, t);

            return TRS(
                !translate ? lerpedTranslation : lhs.GetTranslation(),
                !rotate ? slerpedRotation : lhs.GetRotation(),
                !scale ? lerpedScale : lhs.GetScale());
        }

        public static float CalculateDeterminant4x4(float[] matrix) {
            if (matrix.Length != 16) throw new Exception("Matrix is not 4x4");
            float[] minor11 = Minor(matrix, 4, 0, 0, out float coeff11);
            float[] minor12 = Minor(matrix, 4, 0, 1, out float coeff12);
            float[] minor13 = Minor(matrix, 4, 0, 2, out float coeff13);
            float[] minor14 = Minor(matrix, 4, 0, 3, out float coeff14);
            return coeff11 * CalculateDeterminant3x3(minor11) +
                   coeff12 * CalculateDeterminant3x3(minor12) +
                   coeff13 * CalculateDeterminant3x3(minor13) +
                   coeff14 * CalculateDeterminant3x3(minor14);
        }
        public static float CalculateDeterminant3x3(float[] matrix) {
            if (matrix.Length != 9) throw new Exception("Matrix is not 3x3");
            float[] minor11 = Minor(matrix, 3, 0, 0, out float coeff11);
            float[] minor12 = Minor(matrix, 3, 0, 1, out float coeff12);
            float[] minor13 = Minor(matrix, 3, 0, 2, out float coeff13);
            return coeff11 * CalculateDeterminant2x2(minor11) +
                   coeff12 * CalculateDeterminant2x2(minor12) +
                   coeff13 * CalculateDeterminant2x2(minor13);
        }
        public static float CalculateDeterminant2x2(float[] matrix) {
            if (matrix.Length != 4) throw new Exception("Matrix is not 2x2");
            return (matrix[0] * matrix[3]) - (matrix[1] * matrix[2]);
        }
        public static float[] Minor(float[] matrix, int size, int i, int j, out float coefficient) {
            coefficient = matrix[i * size + j] * (i + j % 2 == 0 ? 1 : -1);
            List<float> minor = new List<float>();
            for (int k = 0; k < size; k++) {
                for (int m = 0; m < size; m++) {
                    if (k != i && m != j) {
                        minor.Add(matrix[k * size + m]);
                    }
                }
            }
            return minor.ToArray();
        }

        public static Quaternion RotationBetween(TRSMatrix lhs, TRSMatrix rhs) {
            Vector3 leftXbasis = lhs.GetBasisVector(0);
            Vector3 leftYbasis = lhs.GetBasisVector(1);
            Vector3 leftZbasis = lhs.GetBasisVector(2);

            Vector3 rightXbasis = rhs.GetBasisVector(0);
            Vector3 rightYbasis = rhs.GetBasisVector(1);
            Vector3 rightZbasis = rhs.GetBasisVector(2);

            // calculate weighted components of final rotation axis
            Vector3 rotateX = leftXbasis.Normalized().Cross(rightXbasis.Normalized());
            Vector3 rotateY = leftYbasis.Normalized().Cross(rightYbasis.Normalized());
            Vector3 rotateZ = leftZbasis.Normalized().Cross(rightZbasis.Normalized());
            Vector3 rotationAxis = rotateX + rotateY + rotateZ;

            // if any cross products are the zero vector that axis is the rotation axis
            // this avoids division by zero in the coming projection steps
            if (rotateX.IsZeroVector()) rotationAxis = leftXbasis.Normalized();
            if (rotateY.IsZeroVector()) rotationAxis = leftYbasis.Normalized();
            if (rotateZ.IsZeroVector()) rotationAxis = leftZbasis.Normalized();

            Vector4 normalisedRotationAxis = rotationAxis.Normalized();

            // project basis vectors of each matrix onto plane defined by rotation axis
            Vector4 leftXProjection = leftXbasis - leftXbasis.ProjectOnto(rotationAxis);
            Vector4 leftYProjection = leftYbasis - leftYbasis.ProjectOnto(rotationAxis);
            Vector4 leftZProjection = leftZbasis - leftZbasis.ProjectOnto(rotationAxis);

            Vector4 rightXProjection = rightXbasis - rightXbasis.ProjectOnto(rotationAxis);
            Vector4 rightYProjection = rightYbasis - rightYbasis.ProjectOnto(rotationAxis);
            Vector4 rightZProjection = rightZbasis - rightZbasis.ProjectOnto(rotationAxis);

            // Check which projected pair of vectores are least parallell to the rotation axis...
            float AbsSummedDotX = Mathf.Abs(leftXbasis.Dot(normalisedRotationAxis) +
                                            rightXbasis.Dot(normalisedRotationAxis));
            float AbsSummedDotY = Mathf.Abs(leftYbasis.Dot(normalisedRotationAxis) +
                                            rightYbasis.Dot(normalisedRotationAxis));
            float AbsSummedDotZ = Mathf.Abs(leftZbasis.Dot(normalisedRotationAxis) +
                                            rightZbasis.Dot(normalisedRotationAxis));

            // ...and calculate the angle between those vectors
            float angularDisplacement;
            float rotationDirection = 1;
            if (AbsSummedDotX < AbsSummedDotY && AbsSummedDotX < AbsSummedDotZ) {
                // x is closest to the plane
                angularDisplacement = Mathf.Acos(leftXProjection.Normalized().Dot(rightXProjection.Normalized()));
                rotationDirection = Mathf.Sign(leftXProjection.Cross(rightXProjection).Dot(rotationAxis));
            }
            else if (AbsSummedDotY < AbsSummedDotZ) {
                // y is closest to the plane
                angularDisplacement = Mathf.Acos(leftYProjection.Normalized().Dot(rightYProjection.Normalized()));
                rotationDirection = Mathf.Sign(leftYProjection.Cross(rightYProjection).Dot(rotationAxis));
            }
            else {
                // z is closest to the plane
                angularDisplacement = Mathf.Acos(leftZProjection.Normalized().Dot(rightZProjection.Normalized()));
                rotationDirection = Mathf.Sign(leftZProjection.Cross(rightZProjection).Dot(rotationAxis));
            }
            // flip angularDisplacement if rotating against left hand rule direction
            // solution inspired by https://stackoverflow.com/questions/5188561/signed-angle-between-two-3d-vectors-with-same-origin-within-the-same-plane
            angularDisplacement *= rotationDirection;

            // create a quaternion from the rotation axis and angular displacement

            float scalar = Mathf.Cos(angularDisplacement / 2);
            Vector4 vector = Mathf.Sin(angularDisplacement / 2) * normalisedRotationAxis;
            return new Quaternion(vector.x, vector.y, vector.z, scalar);
        }

        public override string ToString() {
            string returnString = "\n";
            for (int i = 0; i < SIZE; i++) {
                for (int j = 0; j < SIZE; j++) {
                    returnString += this[i, j] + " ";
                }
                returnString += "\n";
            }
            return returnString;
        }
    }

    // Custom inspector displaying the matrix as a grid of floats, with a reset button.
    [CustomPropertyDrawer(typeof(TRSMatrix))]
    public class MatrixDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty matrix = property.FindPropertyRelative("matrixArray");

            int size = 4;
            float[] matrixArray = new float[size * size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    matrixArray[i * size + j] = matrix.GetArrayElementAtIndex(i * size + j).floatValue;
                }
            }

            float determinant = TRSMatrix.CalculateDeterminant4x4(matrixArray);

            EditorGUILayout.BeginHorizontal();
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;

            EditorGUILayout.PrefixLabel(label.text + $"\n(det: {determinant})", GUIStyle.none, labelStyle);
            EditorGUILayout.BeginVertical();

            // Display the matrix as a grid of float fields. 4 rows and 4 columns.
            for (int i = 0; i < size; i++) //number of rows
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < size; j++) //number of columns
                {
                    matrix.GetArrayElementAtIndex(i * size + j).floatValue =
                        EditorGUILayout.FloatField(matrix.GetArrayElementAtIndex(i * size + j).floatValue);
                }

                EditorGUILayout.EndHorizontal();
            }

            // Include a button that resets the matrix to an identity matrix
            if (GUILayout.Button("Reset")) {
                Debug.Log("Reset matrix");
                for (int i = 0; i < size; i++) //number of rows
                {
                    for (int j = 0; j < size; j++) //number of columns
                    {
                        matrix.GetArrayElementAtIndex(i * size + j).floatValue = i == j ? 1 : 0;
                    }
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndProperty();
        }
    }

    #region ReadOnly Attribute

    // Attribute to show a variable in the inspector without being able to change the value
    // Can be added to any (serialized) member variable with [ReadOnly]
    public class ReadOnlyAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

    #endregion
}