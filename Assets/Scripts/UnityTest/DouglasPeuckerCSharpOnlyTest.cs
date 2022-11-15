using CSharp_Only;
using UnityEngine;

public class DouglasPeuckerCSharpOnlyTest : MonoBehaviour {

    public float pointRadius = 1f;
        
    public Vector3[] nativePoints;
    public float tolerance;

    private Vector3[] _douglasPoints;

    private void OnDrawGizmos() {
        if (nativePoints is not { Length: > 0 })
            return;
            
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(nativePoints[0], pointRadius);

        for (var i = 1; i < nativePoints.Length; i++) {
            Gizmos.DrawLine(nativePoints[i-1], nativePoints[i]);
            Gizmos.DrawSphere(nativePoints[i], pointRadius);
        }
            
        if (_douglasPoints is not { Length: > 0 })
            return;
            
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_douglasPoints[0], pointRadius);

        for (var i = 1; i < _douglasPoints.Length; i++) {
            Gizmos.DrawLine(_douglasPoints[i-1], _douglasPoints[i]);
            Gizmos.DrawSphere(_douglasPoints[i], pointRadius);
        }
            
    }

    private void Update() {
        _douglasPoints =
            Utility.SystemToUnity(
                DouglasPeuckerAlgorithm.PointReduction(Utility.UnityToSystem(nativePoints), tolerance));
    }
}