using System;
using System.Collections.Generic;
using System.Globalization;
using M2MqttUnity;
using Packages.M2Mqtt.Messages;
using UnityEngine;

namespace Mqtt {
    public class MyMqttClient : M2MqttUnityClient, IObservable<MqttData> {

        public List<MqttObserver> mqttObserversToConnect = new();

        public bool iSPhyphox = false;

        private readonly List<IObserver<MqttData>> _mpu6050Observers = new();
        private readonly List<IObserver<MqttData>> _mpu6051Observers = new();
        private readonly List<IObserver<MqttData>> _phyphoxObservers = new();

        private string[] _phyphoxRows;
        private int _phyphoxRowCounter;

        public IDisposable Subscribe(IObserver<MqttData> observer) {
            var mqttObserver = (MqttObserver)observer;
            return Subscribe(observer, mqttObserver.topic);
        }

        protected override void SubscribeTopics() {
            Client.Subscribe(new []{"/topic/MPU6050-data/+"}, new []{ MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            Client.Subscribe(new []{"/topic/MPU6051-data/+"}, new []{ MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void DecodeMessage(string topic, byte[] message) {
            var data = new MqttData(topic, System.Text.Encoding.UTF8.GetString(message));
            Debug.Log($"{data}  - {System.Text.Encoding.UTF8.GetString(message)}");
            TrackMqttData(data);
        }

        protected override void Start() {
            base.Start();

            foreach (var mqttObserver in mqttObserversToConnect) {
                if (mqttObserver != null)
                    mqttObserver.Subscribe(this);
            }

            if (iSPhyphox) {
                _phyphoxRows = System.IO.File.ReadAllLines("Assets/Rechts-Links.csv");
                _phyphoxRowCounter = 0;
            }
        }

        protected override void Update() {
            if (!iSPhyphox) {
                ProcessMqttEvents();
            }
            else {
                _phyphoxRowCounter++;
                if (_phyphoxRowCounter >= _phyphoxRows.Length)
                    return;
                
                
                var cells = _phyphoxRows[_phyphoxRowCounter].Split(",");
                var cellValues = new double[cells.Length];
                for (var j = 0; j < cells.Length; j++) {
                    cellValues[j] = double.Parse(cells[j], CultureInfo.InvariantCulture);
                }

                var data = new MqttData {
                    DateTime = DateTime.Today.AddSeconds(cellValues[0]),
                    Acceleration = new Vector3((float)cellValues[1], (float)cellValues[2], (float)cellValues[3]),
                    Topic = MqttTopic.Phyphox
                };
                TrackMqttData(data);
            }
        }

        private IDisposable Subscribe(IObserver<MqttData> observer, MqttTopic topic) {
            switch (topic) {
                case MqttTopic.MPU6050:
                    if (!_mpu6050Observers.Contains(observer)) {
                        _mpu6050Observers.Add(observer);
                    }
                    return new Unsubscriber(_mpu6050Observers, observer);

                case MqttTopic.MPU6051:
                    if (!_mpu6051Observers.Contains(observer)) {
                        _mpu6051Observers.Add(observer);
                    }
                    return new Unsubscriber(_mpu6051Observers, observer);
                
                case MqttTopic.Phyphox:
                    if (!_phyphoxObservers.Contains(observer)) {
                        _phyphoxObservers.Add(observer);
                    }
                    return new Unsubscriber(_phyphoxObservers, observer);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TrackMqttData(MqttData? mqttData) {
            if (mqttData.HasValue) {
                switch (mqttData.Value.Topic) {
                    case MqttTopic.MPU6050:
                        foreach (var observer in _mpu6050Observers)
                            observer.OnNext(mqttData.Value);
                        break;
                    
                    case MqttTopic.MPU6051:
                        foreach (var observer in _mpu6051Observers)
                            observer.OnNext(mqttData.Value);
                        break;
                    
                    case MqttTopic.Phyphox:
                        foreach (var observer in _phyphoxObservers)
                            observer.OnNext(mqttData.Value);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else {
                foreach (var observer in _mpu6050Observers)
                    observer.OnError(new MqttDataUnknownException());
                foreach (var observer in _mpu6051Observers)
                    observer.OnError(new MqttDataUnknownException());
            }
        }
    
        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<MqttData>> _observers;
            private readonly IObserver<MqttData> _observer;

            public Unsubscriber(List<IObserver<MqttData>> observers, IObserver<MqttData> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    
    }

    public class MqttDataUnknownException : Exception {
    }
}