using UnityEngine;

namespace Trajectories {
    public class TrajectoryPoint {
        
        public TrajectoryPoint() {
        }

        public TrajectoryPoint(Vector3 position, float time, Vector3 acceleration) {
            Position = position;
            Time = time;
            Acceleration = acceleration;
        }

        public Vector3 Position { get; set; }
        public float Time { get; set; }
        
        //TODO: in Sensor-Daten Attribut auslagern
        public Vector3 Acceleration { get; set; }
        
        public override string ToString() {
            return $"[t={Time}, {Position}";
        }
    }
}