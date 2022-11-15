using System.Numerics;

public static class Utility {
    public static Vector3 UnityToSystem(UnityEngine.Vector3 unityVector3) {
        return new Vector3(unityVector3.x, unityVector3.y, unityVector3.z);
    }

    public static UnityEngine.Vector3 SystemToUnity(Vector3 systemVector3) {
        return new UnityEngine.Vector3(systemVector3.X, systemVector3.Y, systemVector3.Z);
    }

    public static Vector3[] UnityToSystem(UnityEngine.Vector3[] unityVector3S) {
        var systemVector3S = new Vector3[unityVector3S.Length];
        for (var i = 0; i < unityVector3S.Length; i++) {
            systemVector3S[i] = Utility.UnityToSystem(unityVector3S[i]);
        }

        return systemVector3S;
    }
    public static UnityEngine.Vector3[] SystemToUnity(Vector3[] systemVector3S) {
        var unityVector3S = new UnityEngine.Vector3[systemVector3S.Length];
        for (var i = 0; i < systemVector3S.Length; i++) {
            unityVector3S[i] = SystemToUnity(systemVector3S[i]);
        }

        return unityVector3S;
    }
}