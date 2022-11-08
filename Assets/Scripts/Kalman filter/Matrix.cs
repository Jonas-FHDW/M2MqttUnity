using System;
using UnityEngine;

// Modified from https://msdn.microsoft.com/en-us/magazine/mt736457.aspx
namespace Kalman_filter {
    public class Matrix
    {
        public float[][] Mat;
        public int Rows
        {
            get { return Mat.Length; }
        }
        public int Cols
        {
            get { return Mat[0].Length; }
        }

        public Matrix(int rows, int cols)
        {
            Mat = MatrixCreate(rows, cols);
        }

        public Matrix(float[][] values)
        {
            Mat = new float[values.GetLength(0)][];
            for (var i = 0; i < values.GetLength(0); i++)
            {
                Mat[i] = new float[values[0].Length];
            }
            for (var i = 0; i < Rows; ++i)
            for (var j = 0; j < Cols; ++j)
                Mat[i][j] = values[i][j];
        }


        public Matrix(Matrix4x4 m)
        {
            var res = new Matrix(new float[] {
                m.m00, m.m01, m.m02, m.m03,
                m.m10, m.m11, m.m12, m.m13,
                m.m20, m.m21, m.m22, m.m23,
                m.m30, m.m31, m.m32, m.m33
            }, 4, 4);

            Mat = res.Mat;
        }
        public Matrix(Transform transform, bool useLocalValues)
        {
            /*
        Matrix4x4 m = transform.localToWorldMatrix;
        Matrix res = new Matrix(new float[] {
            m.m00, m.m01, m.m02, m.m03,
            m.m10, m.m11, m.m12, m.m13,
            m.m20, m.m21, m.m22, m.m23,
            m.m30, m.m31, m.m32, m.m33
        }, 4, 4);

        mat = res.mat;
        */

            if (useLocalValues)
            {
                var rot = new Matrix(Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale));
                Mat = rot.Mat;
            }
            else
            {
                var rot = new Matrix(Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale));
                Mat = rot.Mat;
            }
        }

        public Matrix T
        {
            get {
                var resValues = new float[Rows * Cols];

                var pos = 0;
                for (var i = 0; i < Cols; ++i)
                for (var j = 0; j < Rows; ++j)
                    resValues[pos++] = Mat[j][i];
                return new Matrix(resValues, Cols, Rows);
            }
        }

        public Matrix(Vector4 vec)
        {
            var res = new Matrix(new float[] { vec.x, vec.y, vec.z, vec.w }, 4, 1);
            Mat = res.Mat;
        }

        public Matrix(Vector3 vec)
        {
            var res = new Matrix(new float[] { vec.x, vec.y, vec.z }, 3, 1);
            Mat = res.Mat;
        }

        public Matrix(Vector2 vec)
        {
            var res = new Matrix(new float[] { vec.x, vec.y }, 2, 1);
            Mat = res.Mat;
        }

        // Fro http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/
        public Matrix(Quaternion quat)
        {
            Mat = MatrixCreate(3, 3);

            var q = new Vector4(quat.x, quat.y, quat.z, quat.w);
            q = q.normalized;

            var x = q.x;
            var y = q.y;
            var z = q.z;
            var w = q.w;

            Mat[0][0] = 1 - 2 * y * y - 2 * z * z;
            Mat[0][1] = 2 * x * y - 2 * z * w;
            Mat[0][2] = 2 * x * z + 2 * y * w;

            Mat[1][0] = 2 * x * y + 2 * z * w;
            Mat[1][1] = 1 - 2 * x * x - 2 * z * z;
            Mat[1][2] = 2 * y * z - 2 * x * w;

            Mat[2][0] = 2 * x * z - 2 * y * w;
            Mat[2][1] = 2 * y * z + 2 * x * w;
            Mat[2][2] = 1 - 2 * x * x - 2 * y * y;
        }

        public Matrix(float[] values, int rows, int cols)
        {
            if (rows*cols != values.Length)
            {
                throw new Exception("rows x cols: " + rows + "x" + cols + " = " + rows * cols + " does not equal given data of size " + values.Length);
            }

            Mat = MatrixCreate(rows, cols);

            var pos = 0;
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    Mat[i][j] = values[pos];
                    pos++;
                }
            }
        }

        public Matrix(double[] values, int rows, int cols)
        {
            if (rows * cols != values.Length)
            {
                throw new Exception("rows x cols: " + rows + "x" + cols + " = " + rows * cols + " does not equal given data of size " + values.Length);
            }

            Mat = MatrixCreate(rows, cols);

            var pos = 0;
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    Mat[i][j] = (float)values[pos];
                    pos++;
                }
            }
        }

        // From http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
        // Uses only top left 3x3
        public Quaternion ToQuaterion()
        {
            var a = Mat;
            var q = new Quaternion();
            var trace = a[0][0] + a[1][1] + a[2][2]; // I removed + 1.0f; see discussion with Ethan
            if (trace > 0)
            {// I changed M_EPSILON to 0
                var s = 0.5f / Mathf.Sqrt(trace + 1.0f);
                q.w = 0.25f / s;
                q.x = (a[2][1] - a[1][2]) * s;
                q.y = (a[0][2] - a[2][0]) * s;
                q.z = (a[1][0] - a[0][1]) * s;
            }
            else
            {
                if (a[0][0] > a[1][1] && a[0][0] > a[2][2])
                {
                    var s = 2.0f * Mathf.Sqrt(1.0f + a[0][0] - a[1][1] - a[2][2]);
                    q.w = (a[2][1] - a[1][2]) / s;
                    q.x = 0.25f * s;
                    q.y = (a[0][1] + a[1][0]) / s;
                    q.z = (a[0][2] + a[2][0]) / s;
                }
                else if (a[1][1] > a[2][2])
                {
                    var s = 2.0f * Mathf.Sqrt(1.0f + a[1][1] - a[0][0] - a[2][2]);
                    q.w = (a[0][2] - a[2][0]) / s;
                    q.x = (a[0][1] + a[1][0]) / s;
                    q.y = 0.25f * s;
                    q.z = (a[1][2] + a[2][1]) / s;
                }
                else
                {
                    var s = 2.0f * Mathf.Sqrt(1.0f + a[2][2] - a[0][0] - a[1][1]);
                    q.w = (a[1][0] - a[0][1]) / s;
                    q.x = (a[0][2] + a[2][0]) / s;
                    q.y = (a[1][2] + a[2][1]) / s;
                    q.z = 0.25f * s;
                }
            }
            return q;
        }

        public Vector4 ToVec4()
        {
            var arr = ToArray();

            if (!((Rows == 4 && Cols == 1) || (Rows == 1 && Cols == 4)))
            {
                throw new ArgumentException("Matrix is of dimension (" + Rows + ", " + Cols + ") which cannot be converted to a Vector4");
            }

            return new Vector4(arr[0], arr[1], arr[2], arr[3]);
        }

        public Vector3 ToVec3()
        {
            var arr = ToArray();

            if (!((Rows == 3 && Cols == 1) || (Rows == 1 && Cols == 3)))
            {
                throw new ArgumentException("Matrix is of dimension (" + Rows + ", " + Cols + ") which cannot be converted to a Vector3");
            }

            return new Vector3(arr[0], arr[1], arr[2]);
        }

        public Vector2 ToVec2()
        {
            var arr = ToArray();

            if (!((Rows == 2 && Cols == 1) || (Rows == 1 && Cols == 2)))
            {
                throw new ArgumentException("Matrix is of dimension (" + Rows + ", " + Cols + ") which cannot be converted to a Vector2");
            }

            return new Vector2(arr[0], arr[1]);
        }



        public override string ToString()
        {
            return MatrixAsString(Mat);
        }

        public float[] ToArray()
        {
            var arr = new float[Rows * Cols];
            var pos = 0;
            for (var i = 0; i < Rows; ++i)
            for (var j = 0; j < Cols; ++j)
                arr[pos++] = Mat[i][j];
            return arr;
        }

        public float this[int i, int j]
        {
            get { return Mat[i][j]; }
            set { Mat[i][j] = value; }
        }

        public static Matrix operator *(Matrix m, float s)
        {
            var res = new Matrix(m.Rows, m.Cols);
            for (var i = 0; i < m.Rows; ++i)
            for (var j = 0; j < m.Cols; ++j)
                res[i, j] = m[i, j] * s;
            return res;
        }

        public static Matrix operator *(float s, Matrix m)
        {
            return m * s;
        }

        public static Matrix operator +(Matrix m, float s)
        {
            var res = new Matrix(m.Rows, m.Cols);
            for (var i = 0; i < m.Rows; ++i)
            for (var j = 0; j < m.Cols; ++j)
                res[i, j] = m[i, j] + s;
            return res;
        }

        public static Matrix operator +(float s, Matrix m)
        {
            return m + s;
        }

        public static Matrix operator -(Matrix m, float s)
        {
            return m + (-s);
        }

        public static Matrix operator -(float s, Matrix m)
        {
            return (-m) + s;
        }


        public static Matrix operator *(Matrix c1, Matrix c2)
        {
            return new Matrix(MatrixProduct(c1.Mat, c2.Mat));
        }

        public static Matrix operator *(Matrix c1, Vector3 c2)
        {
            return new Matrix(MatrixProduct(c1.Mat, (new Matrix(c2)).Mat));
        }
        public static Matrix operator *(Vector3 c1, Matrix c2)
        {
            return new Matrix(MatrixProduct((new Matrix(c1)).T.Mat, c2.Mat));
        }

        public static Matrix operator *(Matrix c1, Vector4 c2)
        {
            return new Matrix(MatrixProduct(c1.Mat, (new Matrix(c2)).Mat));
        }
        public static Matrix operator *(Vector4 c1, Matrix c2)
        {
            return new Matrix(MatrixProduct((new Matrix(c1)).T.Mat, c2.Mat));
        }

        public static Matrix operator -(Matrix m)
        {
            var res = new Matrix(m.Rows, m.Cols);

            for (var i = 0; i < m.Rows; ++i)
            for (var j = 0; j < m.Cols; ++j)
                res[i, j] = -m[i, j];
            return res;
        }

        public static Matrix operator +(Matrix c1, Matrix c2)
        {
            if (c1.Rows != c2.Rows || c1.Cols != c2.Cols)
            {
                throw new ArgumentException("Left hand side size: (" + c1.Rows + ", " + c1.Cols + ") != Right hand side size: (" + c2.Rows + ", " + c2.Cols + ")");
            }
            var res = new Matrix(c1.Rows, c1.Cols);
            for (var i = 0; i < c1.Rows; ++i)
            for (var j = 0; j < c1.Cols; ++j)
                res[i, j] = c1[i, j] + c2[i, j];
            return res;
        }

        public static Matrix operator -(Matrix c1, Matrix c2)
        {
            return c1 + (-c2);
        }

        public Matrix Inverse()
        {
            return new Matrix(MatrixInverse(Mat));
        }


        public static float[][] MatrixInverse(float[][] matrix)
        {
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            var n = matrix.Length;
            var result = MatrixCreate(n, n); // make a copy of matrix
            for (var i = 0; i < n; ++i)
            for (var j = 0; j < n; ++j)
                result[i][j] = matrix[i][j];

            float[][] lum; // combined lower & upper
            int[] perm;
            int toggle;
            toggle = MatrixDecompose(matrix, out lum, out perm);

            var b = new float[n];
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0f;
                    else
                        b[j] = 0.0f;

                var x = Helper(lum, b); // 
                for (var j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        } // MatrixInverse

        public static int MatrixDecompose(float[][] m, out float[][] lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum[][]
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            var toggle = +1; // even (+1) or odd (-1) row permutatuions
            var n = m.Length;

            // make a copy of m[][] into result lu[][]
            lum = MatrixCreate(n, n);
            for (var i = 0; i < n; ++i)
            for (var j = 0; j < n; ++j)
                lum[i][j] = m[i][j];


            // make perm[]
            perm = new int[n];
            for (var i = 0; i < n; ++i)
                perm[i] = i;

            for (var j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                var max = System.Math.Abs(lum[j][j]);
                var piv = j;

                for (var i = j + 1; i < n; ++i) // find pivot index
                {
                    var xij = System.Math.Abs(lum[i][j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                } // i

                if (piv != j)
                {
                    var tmp = lum[piv]; // swap rows j, piv
                    lum[piv] = lum[j];
                    lum[j] = tmp;

                    var t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                var xjj = lum[j][j];
                if (xjj != 0.0)
                {
                    for (var i = j + 1; i < n; ++i)
                    {
                        var xij = lum[i][j] / xjj;
                        lum[i][j] = xij;
                        for (var k = j + 1; k < n; ++k)
                            lum[i][k] -= xij * lum[j][k];
                    }
                }

            } // j

            return toggle;
        } // MatrixDecompose

        public static float[] Helper(float[][] luMatrix, float[] b) // helper
        {
            var n = luMatrix.Length;
            var x = new float[n];
            b.CopyTo(x, 0);

            for (var i = 1; i < n; ++i)
            {
                var sum = x[i];
                for (var j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (var i = n - 2; i >= 0; --i)
            {
                var sum = x[i];
                for (var j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        } // Helper

        public static float MatrixDeterminant(float[][] matrix)
        {
            float[][] lum;
            int[] perm;
            var toggle = MatrixDecompose(matrix, out lum, out perm);
            float result = toggle;
            for (var i = 0; i < lum.Length; ++i)
                result *= lum[i][i];
            return result;
        }

        // ----------------------------------------------------------------

        public static float[][] MatrixCreate(int rows, int cols)
        {
            var result = new float[rows][];
            for (var i = 0; i < rows; ++i)
                result[i] = new float[cols];
            return result;
        }

        public static float[][] MatrixProduct(float[][] matrixA,
            float[][] matrixB)
        {
            var aRows = matrixA.Length;
            var aCols = matrixA[0].Length;
            var bRows = matrixB.Length;
            var bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices");

            var result = MatrixCreate(aRows, bCols);

            for (var i = 0; i < aRows; ++i) // each row of A
            for (var j = 0; j < bCols; ++j) // each col of B
            for (var k = 0; k < aCols; ++k) // could use k < bRows
                result[i][j] += matrixA[i][k] * matrixB[k][j];

            return result;
        }

        public static string MatrixAsString(float[][] matrix)
        {
            var s = "";
            for (var i = 0; i < matrix.Length; ++i)
            {
                for (var j = 0; j < matrix[i].Length; ++j)
                    s += matrix[i][j].ToString("F3").PadLeft(8) + " ";
                s += Environment.NewLine;
            }
            return s;
        }

        public static float[][] ExtractLower(float[][] lum)
        {
            // lower part of an LU Doolittle decomposition (dummy 1.0s on diagonal, 0.0s above)
            var n = lum.Length;
            var result = MatrixCreate(n, n);
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    if (i == j)
                        result[i][j] = 1.0f;
                    else if (i > j)
                        result[i][j] = lum[i][j];
                }
            }
            return result;
        }

        public static float[][] ExtractUpper(float[][] lum)
        {
            // upper part of an LU (lu values on diagional and above, 0.0s below)
            var n = lum.Length;
            var result = MatrixCreate(n, n);
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    if (i <= j)
                        result[i][j] = lum[i][j];
                }
            }
            return result;
        }

        public static Matrix Identity(int rows) {
            var matrix = new Matrix(rows, rows);
            for (var i = 0; i < rows; i++) {
                matrix[i, i] = 1f;
            }
            return matrix;
        }
    }
} 
