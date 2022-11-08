using UnityEngine;

namespace Kalman_filter {
    public class KalmanFilter : MonoBehaviour {
        private const int ZUSTANDS_VARIABLEN_ANZAHL = 9;

        [Header("Outputs")]
        // Newest estimate of the current "true" state.
        public Matrix NeuerZustand = new(ZUSTANDS_VARIABLEN_ANZAHL, 1);
        // Newest estimate of the average error for each part of the state.
        public Matrix NeueCovarianz = new(ZUSTANDS_VARIABLEN_ANZAHL, ZUSTANDS_VARIABLEN_ANZAHL); 

        [Header("Constants")]
        // State transition matrix. Basically, multiply state by this and add control factors, and you get a prediction of the state for the next time step.
        public Matrix ZustandsMatrix = new(ZUSTANDS_VARIABLEN_ANZAHL, ZUSTANDS_VARIABLEN_ANZAHL); 
        // Control matrix. This is used to define linear equations for any control factor.
        public Matrix EingangsMatrix = new(ZUSTANDS_VARIABLEN_ANZAHL, ZUSTANDS_VARIABLEN_ANZAHL); 
        // Observation matrix. Multiply a state vector byH to translate it to a measurement vector.
        public Matrix AusgangsMatrix = new(ZUSTANDS_VARIABLEN_ANZAHL, ZUSTANDS_VARIABLEN_ANZAHL); 
        // Estimated process error covariance. Finding precise values for Q and R aew beyond the scope of this guide.
        public float prozessrauschen = 0.005f;
        // Estimated measurement error covariance. Finding precise values for Q and R aew beyond the scope of this guide.
        public float messrauschen = 0.3f; 
        
        [Header("Intermediary Variables")]
        public Matrix ZustandsVorhersage;
        public Matrix AktuellerZustand = new(ZUSTANDS_VARIABLEN_ANZAHL, 1);
        public Matrix PPredicted;
        public Matrix AktuelleCovarianz = new(ZUSTANDS_VARIABLEN_ANZAHL, ZUSTANDS_VARIABLEN_ANZAHL);
        public Matrix Innovation;
        public Matrix InnovationCovarianz;
        public Matrix KalmanGain;
        public Matrix EinheitsMatrix = Matrix.Identity(ZUSTANDS_VARIABLEN_ANZAHL);

        private void Awake() {
        }

        private void Update() {
            TimeUpdate(new Matrix(9, 1));
            MeasurementUpdate(new Matrix(9, 1));
        }

        // Prediction
        // Control Vector. This indicates the magnitude of any control system's or user's control on the situation.
        public void TimeUpdate(Matrix systemeingang ) { 
            // Formeln stimmen nicht vollstäding - Transponierung fehlt
            // 1 Project the state ahead
            ZustandsVorhersage = ZustandsMatrix * AktuellerZustand + EingangsMatrix * systemeingang;
            
            // Project the error ahead
            PPredicted = ZustandsMatrix * AktuelleCovarianz * ZustandsMatrix.T + prozessrauschen;
        }

        // Correction
        // Measurement vector. This contains the real-world measurement we recieved in this time step.
        public void MeasurementUpdate(Matrix tatsaelicheMessung) { 
            // Compute the Kalman Gain
            InnovationCovarianz = AusgangsMatrix * PPredicted * AusgangsMatrix.T + messrauschen;
            //TODO: Matrix.Inverse() liefert in der letzen Zeile ein NaN zurück
            var inverse = InnovationCovarianz.Inverse();
            KalmanGain =  PPredicted * AusgangsMatrix.T * inverse;

            // Update the estimate via tatsaelicheMessung
            Innovation = tatsaelicheMessung - AusgangsMatrix * ZustandsVorhersage;
            NeuerZustand = ZustandsVorhersage + (KalmanGain * Innovation);
            
            // Update the error covariance;
            NeueCovarianz = (EinheitsMatrix - (KalmanGain * AusgangsMatrix)) * PPredicted;

            if (!float.IsNaN(AktuelleCovarianz[0,0]) && !float.IsNaN(AktuellerZustand[0,0]) ) {
                AktuellerZustand = NeuerZustand;
                AktuelleCovarianz = NeueCovarianz;
            }
            else {
                AktuellerZustand = new Matrix(ZUSTANDS_VARIABLEN_ANZAHL, 1);
                AktuelleCovarianz = new Matrix(ZUSTANDS_VARIABLEN_ANZAHL, ZUSTANDS_VARIABLEN_ANZAHL);
            }
            
            // Debug.Log($"Zustand: {AktuellerZustand} \n Covarianz: {AktuelleCovarianz}");
        }

    }
}
