using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Mqtt {
    public class PhyphoxClient : MonoBehaviour {

        public GraphDisplay graphDisplay;

        private List<MqttData> _data;
        
        private void Start() {
            _data = new List<MqttData>();
            CsvToMqttData();
            graphDisplay.points = _data.ToArray();
        }

        private void CsvToMqttData() {
            var rows = System.IO.File.ReadAllLines("Assets/Rechts-Links.csv");

            for (var i = 1; i < rows.Length; i++) {
                var cells = rows[i].Split(",");
                var cellValues = new double[cells.Length];
                for (var j = 0; j < cells.Length; j++) {
                    cellValues[j] = double.Parse(cells[j], CultureInfo.InvariantCulture);
                }

                var data = new MqttData {
                    DateTime = DateTime.Today.AddSeconds(cellValues[0]),
                    Acceleration = new Vector3((float)cellValues[1], (float)cellValues[2], (float)cellValues[3]),
                    Topic = MqttTopic.Phyphox
                };
                _data.Add(data);
                Debug.Log(data.ToString());
            }
        }
        
    }
}
