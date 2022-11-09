using System;
using System.Collections;
using System.Collections.Generic;
using Mqtt;
using TMPro;
using UnityEngine;

public class GraphDisplay : MonoBehaviour {

    public Vector3 root;
    public float xSize;
    public float ySize;
    public float zSize;
    public float pointRadius;

    public MqttData[] points;
    
    public void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(root, root + new Vector3(xSize, 0, 0));
        Gizmos.DrawLine(root, root + new Vector3(0, ySize, 0));
        Gizmos.DrawLine(root, root + new Vector3(0, 0, zSize));

        if (points != null) {
            Gizmos.color = Color.red;
            foreach (var point in points) {
                Gizmos.DrawSphere(point.Acceleration, pointRadius);
            }
        }
    }
}
