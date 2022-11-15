using System;
using System.Collections.Generic;

namespace Math.Trajectories {
    public class Trajectory {
        public readonly TrajectoryPoint[] Points;
        public int Length => Points.Length;

        public TrajectoryPoint this[int index] {
            get => Points[index];
        }

        public Trajectory(TrajectoryPoint[] pArray) {
            Points = new TrajectoryPoint[pArray.Length];

            for (var i = 0; i < pArray.Length; i++) {
                Points[i] = pArray[i];
            }
        }

        public List<TrajectoryPoint> AsList() {
            var pointList = new List<TrajectoryPoint>();
            for (var i = 0; i < Points.Length; i++) {
                pointList.Add(this[i]);
            }

            return pointList;
        }
    }

    [Serializable]
    public class TrajectoryData {
        public TrajectoryPoint[] Points { get; set; }
        public int Length { get; set; }
    }
}