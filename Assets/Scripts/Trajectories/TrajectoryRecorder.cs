using System.Collections.Generic;
using Data;
using Mqtt;
using UnityEngine;

namespace Trajectories {
    public class TrajectoryRecorder : MqttObserver {
        public Transform objectToTrack;

        public bool usePeucker = false;
        public DouglasPeuckerTest douglasPeuckerTest;
        
        [Header("Point Visibility")]
        public bool pointsVisible = true;
        public float pointRadius = 0.05f;

        private Trajectory _trajectory;
        private List<TrajectoryPoint> _points;
        private bool _isRecording;
        private bool _isUpdateAvailable;

        //TEMP
        private Vector3 _acceleration;
        
        public override void OnNext(MqttData mqttData) {
            _isUpdateAvailable = true;
            _acceleration = mqttData.Acceleration;
        }

        public void ToggleRecording() {
            if (!_isRecording)
                StartRecording();
            else
                StopRecording();
        }

        public override string ToString() {
            var toString = "";
            foreach (var point in _trajectory.AsList()) {
                toString += $"{point},\n";
            }
            return toString;
        }

        public void OnDrawGizmos() {
            if (!pointsVisible) {
                return;
            }

            if (_points is not { Count: > 0 }) {
                return;
            }
            
            var pointArray = _points.ToArray();
            Gizmos.DrawSphere(pointArray[0].Position, pointRadius);
            for (var i = 1; i < pointArray.Length; i++) {
                Gizmos.DrawSphere(pointArray[i].Position, pointRadius);
                Gizmos.DrawLine(pointArray[i-1].Position, pointArray[i].Position);
            }
        }

        private void Start() {
            _points = new List<TrajectoryPoint>();
        }

        private void LateUpdate() {
            if (_isUpdateAvailable) {
                if (_isRecording) {
                    _points.Add(new TrajectoryPoint(objectToTrack.transform.position, Time.time, _acceleration));
                    
                }
                if (usePeucker || _points.Count >= 2) {
                    douglasPeuckerTest.SetInputList(_points);
                }
                _isUpdateAvailable = false;
            }
        }

        private void StartRecording() {
            Debug.Log("Starting a recording...");
            _isRecording = true;
            _trajectory = null;
        }

        private void StopRecording() {
            Debug.Log("Stopping a recording...");
            _isRecording = false;
            _trajectory = new Trajectory(_points.ToArray());
            if (usePeucker || _trajectory != null) {
                douglasPeuckerTest.SetInputList(_trajectory);
            }
            DataManager.SaveTrajectory(_trajectory);
        }
    }
}
