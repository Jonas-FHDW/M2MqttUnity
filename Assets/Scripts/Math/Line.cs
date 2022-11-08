using UnityEngine;

namespace Math {
    public class Line {

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }

        public Line(Vector3 position, Vector3 direction) {
            this.Position = position;
            this.Direction = direction;
        }
    }
}