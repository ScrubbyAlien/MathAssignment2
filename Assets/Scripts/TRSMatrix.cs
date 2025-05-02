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
                //TODO return a new identity matrix instance
                TRSMatrix I = new TRSMatrix();
                for (int i = 0; i < SIZE; i++) {
                    I[i, i] = 1;
                }
                return I;
            }
        }

        public float determinant {
            get {
                //TODO return the determinant of this matrix
                return 0f;
            }
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
            for (int r = 0; r < SIZE; r++) transformedVector[r] = lhs.GetRow(r).Dot(rhs);
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
            if (lhs == rhs) return false;
            else return true;
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
            Vector4 result = this * new Vector4(point.x, point.y, point.z, 1);
            return new Vector3(result.x, result.y, result.z);
        }

        #endregion

        #region TRS specific Public Methods

        public Vector3 GetTranslation() {
            //TODO return a new Vector3 instance that represents the translation of this matrix
            return new Vector3();
        }

        public Quaternion GetRotation() {
            //TODO return a new Quaternion instance that represents the rotation of this matrix
            return new Quaternion();
        }

        public Vector3 GetScale() {
            //TODO return a new Vector3 instance that represents the scale of this matrix
            return new Vector3();
        }

        public void DrawCoordinateSystem(VectorRenderer vr) {
            //TODO visualize the coordinate system represented by this matrix
        }

        #endregion

        #region TRS specific Static methods

        private static TRSMatrix FromTranslation(Vector3 translation) {
            //TODO return a new matrix instance that represents the given translation
            return new TRSMatrix();
        }

        private static TRSMatrix FromRotation(Quaternion rotation) {
            //TODO return a new matrix instance that represents the given rotation
            return new TRSMatrix();
        }

        private static TRSMatrix FromScale(Vector3 scale) {
            //TODO return a new matrix instance that represents the given scale
            return new TRSMatrix();
        }

        /// <summary>
        /// Creates a new TRSMatrix from the given translation, rotation and scale
        /// </summary>
        /// <param name="translation">The position of the origin of the TRSMatrix coordinate space, relative to world space.</param>
        /// <param name="rotation">The local rotation of the TRSMatrix. Rotated around the origin of the TRSMatrix coordinate space.</param>
        /// <param name="scale">The local scale of the TRSMatrix. Scaled relative to the rotation of the TRSMatrix, not world space.</param>
        /// <returns></returns>
        public static TRSMatrix TRS(Vector3 translation, Quaternion rotation, Vector3 scale) {
            return TRSMatrix.FromTranslation(translation) *
                   TRSMatrix.FromRotation(rotation) * TRSMatrix.FromScale(scale);
        }

        public static Vector3 LerpPosition(TRSMatrix matrixA, TRSMatrix matrixB, float t) {
            //TODO return the position between matrixA and matrixB based on interpolation value t
            return new Vector3();
        }

        public static Quaternion SlerpRotation(TRSMatrix matrixA, TRSMatrix matrixB, float time) {
            //TODO return the rotation between matrixA and matrixB based on interpolation value t
            return new Quaternion();
        }

        public static Vector3 LerpScale(TRSMatrix matrixA, TRSMatrix matrixB, float time) {
            //TODO return the scale between matrixA and matrixB based on interpolation value t
            return new Vector3();
        }

        #endregion

        public static TRSMatrix Interpolate(TRSMatrix lhs, TRSMatrix rhs, float t) {
            TRSMatrix interpolated = new TRSMatrix();
            for (int r = 0; r < SIZE; r++) {
                for (int c = 0; c < SIZE; c++) {
                    interpolated[r, c] = Mathf.Lerp(lhs[r, c], rhs[r, c], t);
                }
            }
            return interpolated;
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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label.text);
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