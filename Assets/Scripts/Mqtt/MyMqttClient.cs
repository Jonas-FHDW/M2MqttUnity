using System;
using System.Collections.Generic;
using M2MqttUnity;
using Packages.M2Mqtt.Messages;

namespace Mqtt {
    public class MyMqttClient : M2MqttUnityClient, IObservable<MqttData> {

        public List<MqttObserver> mqttObserversToConnect = new();

        private readonly List<IObserver<MqttData>> _mpu6050Observers = new();
        private readonly List<IObserver<MqttData>> _mpu6051Observers = new();

        public IDisposable Subscribe(IObserver<MqttData> observer) {
            var mqttObserver = (MqttObserver)observer;
            return Subscribe(observer, mqttObserver.topic);
        }

        protected override void SubscribeTopics() {
            Client.Subscribe(new []{"/topic/mpu6050-data/+"}, new []{ MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            Client.Subscribe(new []{"/topic/mpu6051-data/+"}, new []{ MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void DecodeMessage(string topic, byte[] message) {
            TrackMqttData(new MqttData(topic, System.Text.Encoding.UTF8.GetString(message)));
        }

        protected override void Start() {
            base.Start();

            foreach (var mqttObserver in mqttObserversToConnect) {
                if (mqttObserver != null)
                    mqttObserver.Subscribe(this);
            }
        }

        protected override void Update() {
            ProcessMqttEvents();
        }

        private IDisposable Subscribe(IObserver<MqttData> observer, MqttTopic topic) {
            switch (topic) {
                case MqttTopic.Mpu6050:
                    if (!_mpu6050Observers.Contains(observer)) {
                        _mpu6050Observers.Add(observer);
                    }
                    return new Unsubscriber(_mpu6050Observers, observer);

                case MqttTopic.Mpu6051:
                    if (!_mpu6051Observers.Contains(observer)) {
                        _mpu6051Observers.Add(observer);
                    }
                    return new Unsubscriber(_mpu6051Observers, observer);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TrackMqttData(MqttData? mqttData) {
            if (mqttData.HasValue) {
                switch (mqttData.Value.Topic) {
                    case MqttTopic.Mpu6050:
                        foreach (var observer in _mpu6050Observers)
                            observer.OnNext(mqttData.Value);
                        break;
                    
                    case MqttTopic.Mpu6051:
                        foreach (var observer in _mpu6051Observers)
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