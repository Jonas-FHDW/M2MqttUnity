using System.Data;
using Mono.Data.Sqlite;
using Trajectories;
using UnityEngine;

namespace Data {
    public partial class SqliteDataManager : MonoBehaviour {

        public IDbConnection Conn;
        
        private void Start() {
            Conn = new SqliteConnection("URI=file:" + Application.dataPath + "/BA_IoT.db");
            Conn.Open();

            var trajectoryPoint = new TrajectoryPoint(new Vector3(Random.Range(0f,3f), Random.Range(0f,3f), Random.Range(0f,3f)), Random.Range(0f,10f), Vector3.zero);
            var cmd = Conn.CreateCommand();
            var sql = $"INSERT INTO trajectory_point(position_x, position_y, position_z, time) VALUES ('{trajectoryPoint.Position.x}', '{trajectoryPoint.Position.y}', '{trajectoryPoint.Position.y}', '{trajectoryPoint.Time}');";
            Debug.Log(sql);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            var dbcmd = Conn.CreateCommand();
            var sqlQuery = "SELECT * FROM trajectory_point";
            dbcmd.CommandText = sqlQuery;
            
            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                var posX = reader.GetFloat(0);
                var posY = reader.GetFloat(1);
                var posZ = reader.GetFloat(2);
                var time = reader.GetFloat(3);

                Debug.Log( $"t = {time}, ({posX}, {posY}, {posZ}))");
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            Conn.Close();
            Conn = null;
        }
    }
}