using System;
using UnityEngine;

namespace Mqtt {
    public abstract class MqttObserver : MonoBehaviour, IObserver<MqttData> {

        [Header("Observer Configuration")]
        [SerializeField] public MqttTopic topic;

        public MqttTopic Topic {
            get { return topic; }
            set { topic = value; }
        }
        
        private IDisposable _unsubscriber;

        public virtual void Subscribe(IObservable<MqttData> provider) {
            if (provider != null) {
                _unsubscriber = provider.Subscribe(this);
            }
        }
    
        public virtual void OnCompleted() {
        }

        public virtual void OnError(Exception error) {
        }

        public virtual void OnNext(MqttData mqttData) {
        }

        public virtual void Unsubscribe() {
            _unsubscriber.Dispose();
        } 
    }
    public enum MqttTopic {
        MPU6050,
        MPU6051,
        Phyphox,
        Unknown
    }
}
