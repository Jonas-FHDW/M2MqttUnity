using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Trajectories;
using UnityEditor;
using UnityEngine;

namespace Data {
    public static class DataManager {
        public static void SaveTrajectory(Trajectory trajectory) {
            // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
            // Create a new Player_Data.
            var filePath = $"{Application.dataPath}/Data/Trajectory Library/Trajectory_{trajectory.Length}.xml";
            var file = File.Create(filePath);
            Debug.Log($"File saved at {filePath}");
            var data = new TrajectoryData {
                //Save the data.
                Points = trajectory.Points,
                Length = trajectory.Length
            };

            //Serialize to xml
            var bf = new DataContractSerializer(data.GetType());
            var streamer = new MemoryStream();

            //Serialize the file
            bf.WriteObject(streamer, data);
            streamer.Seek(0, SeekOrigin.Begin);

            //Save to disk
            file.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

            // Close the file to prevent any corruptions
            file.Close();

            var result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
            Debug.Log("Serialized Result: " + result);
        }

    }
}