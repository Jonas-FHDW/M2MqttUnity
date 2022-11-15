using System.Collections.Generic;
using Math;
using Math.Trajectories;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DouglasPeuckerTest : MonoBehaviour {

    public float pointRadius = 0.05f;
    
    [Header("UI Configuration")]
    public float toleranceSliderMin = 0.1f;
    public float toleranceSliderMax = 10f;
    public TMP_Text toleranceTextfield;
    public Slider toleranceSlider;

    [Header("Douglas-Peucker-Algorithm Configuration")]
    public List<Vector3> inputPointList;

    //TEMP
    public bool showAcceleration = false;
    
    private List<Vector3> _displayPointList;
    private float _peuckerTolerance = 5f;
    
    //Temp
    private List<Vector3> _accelerationList;

    public void SetInputList(Trajectory trajectory) {
        var pointList = new List<Vector3>();
        for (var i = 0; i < trajectory.Length; i++) {
            pointList.Add(trajectory[i].Position);
        }

        inputPointList = pointList;
    }
    
    public void SetInputList(List<TrajectoryPoint> trajectoryPointList) {
        var pointList = new List<Vector3>();
        foreach (var point in trajectoryPointList) {
            pointList.Add(point.Position);
        }

        foreach (var tPoint in trajectoryPointList) {
            _accelerationList.Add(new Vector3(tPoint.Time*10, tPoint.Acceleration.magnitude, 0));
            Debug.Log(new Vector3(tPoint.Time, tPoint.Acceleration.magnitude, 0));
        }

        inputPointList = pointList;
    }

    public void ToleranceChanged() {
        _peuckerTolerance = toleranceSlider.value;
        toleranceTextfield.text = $"Peucker-Tolerance: {_peuckerTolerance:0.0000}";
    }

    public void OnDrawGizmos() {
        if (_displayPointList is not { Count: > 0 })
            return;

        var pointArray = _displayPointList.ToArray();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointArray[0], pointRadius);
        
        for (var i = 1; i < pointArray.Length; i++) {
            if (!showAcceleration) {
                Gizmos.DrawLine(pointArray[i-1], pointArray[i]);
            }
            Gizmos.DrawSphere(pointArray[i], pointRadius);
        }
    }

    private void Awake() {
        toleranceSlider.minValue = toleranceSliderMin;
        toleranceSlider.maxValue = toleranceSliderMax;
    }

    private void Start() {
        _displayPointList = inputPointList;
        _accelerationList = new List<Vector3>();
    }

    private void Update() {
        if (inputPointList is not { Count: > 0 })
            return;
        if (!showAcceleration) {
            _displayPointList = new List<Vector3>(DouglasPeuckerAlgorithm.PointReduction(inputPointList.ToArray(), _peuckerTolerance));
        }
        else {
            _displayPointList = _accelerationList;
        }
            
    }
}
