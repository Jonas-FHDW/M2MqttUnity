using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MyRigidbody : MonoBehaviour {
    [SerializeField] public bool usePlayerInputAsAcceleration = false;
    [field: SerializeField] public Vector3 Position { get; set; }
    [field: SerializeField] public Vector3 Velocity { get; set; }
    [field: SerializeField] public Vector3 Acceleration { get; set; }

        private void Start() {
    }

    private void Update() {
        if (usePlayerInputAsAcceleration) {
            Acceleration = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        }

        Velocity += Acceleration * Time.deltaTime;
        Position += Velocity * Time.deltaTime;
        
        // Debug.Log($"Velocity {Velocity} = {Acceleration} * {Time.deltaTime}");
    }

    private void FixedUpdate() {
        transform.position = Position;
    }
}
