using UnityEngine;

namespace TModell {
    public class TModellSensorController : SensorSphereController
    {
        public Vector3 startPosition;
        public Transform myRoot;
        public float length;
        public float angleLimitation;

        protected new void Start() {
            base.Start();
            transform.position = startPosition;
        }
        
        private void LateUpdate() {
            if ((myRoot.position - transform.position).magnitude >= length) {
                var position = myRoot.position;
                var transform1 = transform;
                transform1.position = position + ((transform1.position - position).normalized * length);
            }
        }
    }
}
