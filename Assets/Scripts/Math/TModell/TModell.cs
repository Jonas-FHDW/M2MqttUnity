using UnityEngine;

namespace TModell {
    public class TModell : MonoBehaviour {
        public Vector3 startPosition;
        public float spineLength = .6f;
        public Transform sphere;
        

        private void Start() {
            transform.position = startPosition;
        }

        private void Update() {
        }
    }
}
