using Mqtt;
using UnityEngine;

public class RingBuffer {

    public bool IsFilled { get; private set; }

    public int Length => _data.Length;

    public Vector3 AverageAcceleration { get; private set; }
    public Vector3 MinAcceleration { get; private set; }
    public Vector3 MaxAcceleration { get; private set; }
    public Vector3 MinMaxDifference { get; private set; }

    private MqttData[] _data;
    private int _filledIndex;
    
    public RingBuffer(int size) {
        _data = new MqttData[size];
        _filledIndex = 0;
    }

    public void Push(MqttData data) {
        if (_filledIndex < _data.Length) {
            _filledIndex++;
        }
        else {
            IsFilled = true;
        }
        
        for (var i = _data.Length-1; i > 0; i--) {
            _data[i] = _data[i - 1];
        }
        _data[0] = data;
        
        var accelerationSum = Vector3.zero;
        MinAcceleration = _data[0].Acceleration;
        MaxAcceleration = _data[0].Acceleration;
        
        foreach (var mqttData in _data) {
            accelerationSum += mqttData.Acceleration;
            if (mqttData.Acceleration.magnitude > MaxAcceleration.magnitude) {
                MaxAcceleration = mqttData.Acceleration;
            }
            else if (mqttData.Acceleration.magnitude < MinAcceleration.magnitude) {
                MinAcceleration = mqttData.Acceleration;
            }
        }

        MinMaxDifference = MaxAcceleration - MinAcceleration;
        AverageAcceleration = accelerationSum / _data.Length;
    }

    public override string ToString() {
        var str = "";
        for (int i = 0; i < _data.Length - 1; i++) {
            str += $"{_data[i].Acceleration}, ";
        }
        str += $"{_data[^1].Acceleration}";
        return $"[{str}]";
    }
}