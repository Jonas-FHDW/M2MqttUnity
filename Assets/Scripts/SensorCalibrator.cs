using System;
using System.Collections.Generic;
using Mqtt;
using UnityEngine;

public class SensorCalibrator : MqttObserver, IObservable<MqttData> {

    public List<MqttObserver> mqttObserversToConnect = new();
    
    [Header("Buffer Configuration")]
    [Tooltip("Amount of measements use to determine the acceleration drift")][Range(5,20)] public int bufferSize = 10;
    public float acceptedIdleDifference = 2;

    [Header("Calibration Options")]
    public bool routeBeforeCalibration = true;
    public bool removeDrift = true;
    public float ignoreValuesUnder = 10;

    [Header("Runtime Info")]
    [SerializeField] private Vector3 drift;
    private RingBuffer _dataBuffer;
    private DateTime _last;

    private readonly List<IObserver<MqttData>> _mqttObservers = new();

    public override void OnNext(MqttData mqttData) {
        if (_dataBuffer == null) {
            return;
        }
        _dataBuffer.Push(mqttData);

        if (!routeBeforeCalibration) {
            if (!_dataBuffer.IsFilled)
                return;
        }

        if (_dataBuffer.MinMaxDifference.magnitude <= acceptedIdleDifference) {
            drift = _dataBuffer.AverageAcceleration;
        }

        if (removeDrift) {
            mqttData.Acceleration -= drift;
        }

        if (mqttData.Acceleration.magnitude < ignoreValuesUnder) {
            mqttData.Acceleration = Vector3.zero;
        }
        
        TrackMqttData(mqttData);
    }

    private void Log(MqttData mqttData) {
        var curr = mqttData.DateTime;
        var between = curr - _last;
        Debug.Log($"{mqttData.Topic}:measurementTime[{curr.TimeOfDay:g}], " +
        $"lastTime[{_last.TimeOfDay:g}], " +
        $"between={between.TotalMilliseconds}ms");
        _last = curr;

        Debug.Log($"{mqttData.Topic}:[average={_dataBuffer.AverageAcceleration}, length={_dataBuffer.AverageAcceleration.magnitude}], " +
                  $"[min={_dataBuffer.MinAcceleration}, length={_dataBuffer.MinAcceleration.magnitude}], " +
                  $"[max={_dataBuffer.MaxAcceleration}, length={_dataBuffer.MaxAcceleration.magnitude}], " +
                  $"[difference={_dataBuffer.MinMaxDifference}, length={_dataBuffer.MinMaxDifference.magnitude}]");
    }

    public IDisposable Subscribe(IObserver<MqttData> observer) {
        var mqttObserver = (MqttObserver)observer;
        if (!_mqttObservers.Contains(mqttObserver)) {
            _mqttObservers.Add(mqttObserver);
        }
        return new Unsubscriber(_mqttObservers, mqttObserver);
    }

    private void Start() {
        foreach (var mqttObserver in mqttObserversToConnect) {
            if (mqttObserver != null) {
                mqttObserver.Subscribe(this);
            }
        }

        _last = new DateTime();
        _dataBuffer = new RingBuffer(bufferSize);
    }

    private void TrackMqttData(MqttData? mqttData) {
        if (!mqttData.HasValue) return;
        foreach (var observer in _mqttObservers) {
            observer.OnNext(mqttData.Value);
        }
    }

    private class Unsubscriber : IDisposable {
        private readonly List<IObserver<MqttData>> _observers;
        private readonly IObserver<MqttData> _observer;

        public Unsubscriber(List<IObserver<MqttData>> observers, IObserver<MqttData> observer) {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose() {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
    
}
