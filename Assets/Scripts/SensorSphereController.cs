using System;
using Mqtt;
using UnityEngine;

[RequireComponent(typeof(MyRigidbody))]
public class SensorSphereController : MqttObserver
{
    public MyRigidbody rb;

    public override void OnNext(MqttData mqttData) {
        Debug.Log(mqttData.ToString());
        rb.Acceleration = mqttData.Acceleration;
    }

    protected void Start() {
        rb = GetComponent<MyRigidbody>();
    }
}