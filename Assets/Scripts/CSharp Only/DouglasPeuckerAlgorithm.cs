using System.Numerics;

namespace CSharp_Only {
    public static class DouglasPeuckerAlgorithm {
        
        /// <summary>
        /// Implementation of the Douglas-Peucker algorithm.
        /// It decimates a curve composed of line segments to a similar curve with fewer points.
        /// </summary>
        /// <param name="points">Points of the original curve.</param>
        /// <param name="tolerance">Used tolerance in the algorithm.</param>
        /// <returns>A similar curve with fewer points</returns>
        public static Vector3[] PointReduction(Vector3[] points, float tolerance) {
            Vector3[] resultList;
            // Finde den Punkt mit dem größten Abstand
            float distanceMax = 0;
            var index = 0;

            for (var i = 1; i < points.Length - 1; i++) {
                var distance = PerpendicularDistance(points[i], new Line(points[0], points[^1] - points[0]));
                if (distance > distanceMax) {
                    index = i;
                    distanceMax = distance;
                }
            }

            // Wenn die maximale Entfernung größer ist als die Toleranz, wird rekurisv verinfacht
            if (distanceMax > tolerance) {
                // Rekursiver Aufruf
                var recResult1 = PointReduction(SubArray(points, 0, index), tolerance);
                var recResult2 = PointReduction(SubArray(points, index, points.Length - 1), tolerance);

                // Ergebnisliste aufbauen
                resultList = BuildResultList(recResult1, recResult2);
            }
            else {
                resultList = new[] { points[0], points[^1] };
            }

            // Ergebnis zurückgeben
            return resultList;
        }

        private static float PerpendicularDistance(Vector3 point, Line line) {
            // Lotfußpunkt bestimmen
            var a = line.Position;
            var b = line.Direction;

            var t = (-(a.X - point.X) * b.X - (a.Y - point.Y) * b.Y - (a.Z - point.Z) * b.Z)
                    / (System.Math.Pow(b.X, 2f) + System.Math.Pow(b.Y, 2f) + System.Math.Pow(b.Z, 2f));

            var lotPoint = Vector3.Add(a, Vector3.Multiply((float)t, b));
            return (lotPoint - point).Length();
        }

        private static Vector3[] SubArray(Vector3[] array, int startIndex, int endIndex) {
            if (array == null) {
                return null;
            }

            var subArray = new Vector3[endIndex - startIndex + 1];
            for (var i = startIndex; i <= endIndex; i++) {
                subArray[i - startIndex] = array[i];
            }

            return subArray;
        }

        private static Vector3[] BuildResultList(Vector3[] recResult1, Vector3[] recResult2) {
            var resultArray = new Vector3[recResult1.Length + recResult2.Length - 1];
            for (var i = 0; i < resultArray.Length; i++) {
                if (i < recResult1.Length)
                    resultArray[i] = recResult1[i];
                else
                    resultArray[i] = recResult2[i - (recResult1.Length - 1)];
            }

            return resultArray;
        }

        private class Line {
            public Vector3 Position { get; }
            public Vector3 Direction { get; }

            public Line(Vector3 position, Vector3 direction) {
                Position = position;
                Direction = direction;
            }
        }
    }
}