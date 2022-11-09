
using System;
using UnityEngine;

namespace Mqtt {
    public struct MqttData {
        
        public string RawData { get; set; }
        public MqttTopic Topic { get; set; }
        public DateTime DateTime { get; set; }
        public UnityEngine.Vector3 Acceleration { get; set; }
        public Vector3 Angles { get; set; }
        public Quaternion Quaternion { get; set; }

        public MqttData(string mqttTopic, string message) {
            RawData = message;
            var msgParts = message.Trim().Split(";");
            for (var i = 1; i < msgParts.Length; i++) {
                msgParts[i] = msgParts[i].Replace(".", ",");
            }
        
            if (mqttTopic.ToUpper().Contains("MPU6050"))
                Topic = MqttTopic.MPU6050;
            else if (mqttTopic.Contains("MPU6051"))
                Topic = MqttTopic.MPU6051;
            else
                Topic = MqttTopic.Unknown;
        
            DateTime = DateTime.Parse(msgParts[0]);
            Acceleration = new Vector3(float.Parse(msgParts[1]), float.Parse(msgParts[2]), float.Parse(msgParts[3]));
            Angles = new Vector3(float.Parse(msgParts[4]), float.Parse(msgParts[5]), float.Parse(msgParts[6]));
            Quaternion = new Quaternion();
            //Quaternion = new Quaternion(float.Parse(msgParts[7]), float.Parse(msgParts[8]), float.Parse(msgParts[9]), float.Parse(msgParts[10]));
        }

        public override string ToString() {
            return $"[{Topic}] {DateTime.Second},{DateTime.Millisecond}s : {Acceleration}m/s²";//, {Angles}, {Quaternion}";
        }

        public void Add(MqttData mqttData) {
            throw new NotImplementedException();
        }

        public void Divide(int depth) {
            throw new NotImplementedException();
        }
    }
}