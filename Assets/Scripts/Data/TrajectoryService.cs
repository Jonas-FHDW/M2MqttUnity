using System.Data;
using Math.Trajectories;

namespace Data {
    public class TrajectoryService {
        public IDbConnection Conn;
        
        private const string TABLE_NAME = "trajectory";

        public bool Save(Trajectory entity) {
            // IDbCommand cmd = conn.CreateCommand();
            // string sql = $"INSERT INTO {TABLE_NAME} () VALUES ({});";
            // cmd.CommandText = sql;
            return true;
        }
    }
}