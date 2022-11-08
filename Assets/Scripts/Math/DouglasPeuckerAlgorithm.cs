using Unity.Mathematics;
using UnityEngine;

namespace Math {
    public class DouglasPeuckerAlgorithm {

        public static Vector3[] DouglasPeuckerReduction(Vector3[] points, float tolerance) {
            Vector3[] resultList;
            // Finde den Punkt mit dem größten Abstand
            float distanceMax = 0;
            var index = 0;

            for (var i = 1; i < points.Length - 1; i++) {
                var distance = LotrechterAbstand(points[i], new Line(points[0], points[^1] - points[0]));
                if (distance > distanceMax) {
                    index = i;
                    distanceMax = distance;
                }
            }
            
            // Wenn die maximale Entfernung größer ist als die Toleranz, wird rekurisv verinfacht
            if (distanceMax > tolerance) {
                // Rekursiver Aufruf
                var recResult1 = DouglasPeuckerReduction(SubArray(points, 0, index), tolerance);
                var recResult2 = DouglasPeuckerReduction(SubArray(points, index, points.Length-1), tolerance);
                
                // Ergebnisliste aufbauen
                resultList = BuildResultList(recResult1, recResult2);
            }
            else {
                resultList = new[] { points[0], points[^1] };
            }
            
            // Ergebnis zurückgeben
            return resultList;
        }

        private static float LotrechterAbstand(Vector3 point, Line line) {
            // Lotfußpunkt bestimmen
            var a = line.Position;
            var b = line.Direction;

            var t = (-(a.x - point.x) * b.x - (a.y - point.y) * b.y - (a.z - point.z) * b.z) 
                    / (math.pow(b.x, 2f) + math.pow(b.y, 2f) + math.pow(b.z, 2f));

            var lotPoint = a + t * b;
            return (lotPoint - point).magnitude;
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

        
    }
}