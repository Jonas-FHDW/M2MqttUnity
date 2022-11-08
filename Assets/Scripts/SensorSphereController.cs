using System;
using Mqtt;
using UnityEngine;

[RequireComponent(typeof(MyRigidbody))]
public class SensorSphereController : MqttObserver
{
    public MyRigidbody rb;

    public override void OnNext(MqttData mqttData) {
        rb.Acceleration = mqttData.Acceleration;
        // Debug.Log(mqttData.ToString());
    }

    protected void Start() {
        rb = GetComponent<MyRigidbody>();
    }
}