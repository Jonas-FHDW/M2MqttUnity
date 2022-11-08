using UnityEngine;

namespace Math {
    public class Point3 {
        private Vector3 _vector3;

        public Point3(Vector3 vector3) {
            _vector3 = vector3;
        }

        public float X {
            get { return _vector3.x; }
            set { _vector3.x = value; }
        }
        
        public float Y {
            get { return _vector3.y; }
            set { _vector3.y = value; }
        }
        
        public float Z {
            get { return _vector3.z; }
            set { _vector3.z = value; }
        }
    }
}