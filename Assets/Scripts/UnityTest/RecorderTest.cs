using System;
using Mqtt;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecorderTest : MonoBehaviour {
    
    [Header("UI Elements")]
    public Button recordButton;
    public TMP_Text recordButtonText;

    [Header("Configuration")]
    public RecorderTpes type = RecorderTpes.MqttData;

    public bool recordFromStart;

    private bool _isRecording;
    private IRecorder _recorder;

    public void ToggleRecordState() {
        if (!_isRecording) {
            StartRecording();
        }
        else {
            StopRecording();
        }
    }

    private void Start() {
        if (recordFromStart) {
            StartRecording();
        }
    }

    private void StartRecording() {
        _isRecording = true;
        Debug.Log($"Starting a recording...");
        recordButtonText.text = "Stop Recording";

        switch (type) {
            case RecorderTpes.MqttData:
                _recorder = new MqttDataRecorder();
                _recorder.StartRecording();
                break;
                
            case RecorderTpes.Trajectory:
                // _recorder = new TrajectoryRecorder();
                // _recorder.StartRecording();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void StopRecording() {
        _isRecording = false;
        Debug.Log($"Stopping a recording...");
        recordButtonText.text = "Start Recording";
    }
}

public enum RecorderTpes {
    MqttData,
    Trajectory
}