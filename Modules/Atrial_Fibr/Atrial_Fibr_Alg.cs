using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public partial class Atrial_Fibr : IModule
    {
        public class detectedAF
        {
            private bool _detected;
            private int[] _detectedPoints;
            private string _detectedS;
            private string _timeofAF;

            public bool Detected
            {
                get
                {
                    return _detected;
                }
                set
                {
                    _detected = value;
                }
            }

            public int[] DetectedPoint
            {
                get
                {
                    return _detectedPoints;
                }
                set
                {
                    _detectedPoints = value;
                }
            }

            public string DetectedS
            {
                get
                {
                    return _detectedS;
                }
                set
                {
                    _detectedS = value;
                }
            }

            public string TimeofAF
            {
                get
                {
                    return _timeofAF;
                }
                set
                {
                    _timeofAF = value;
                }
            }

            public detectedAF(bool detected, int[] detectedPoints, string detectedS, string timeofAF)
            {
                this.Detected = detected;
                this.DetectedPoint = detectedPoints;
                this.DetectedS = detectedS;
                this.TimeofAF = timeofAF;
            }
            public detectedAF()
            {
                this.Detected = false;
                int[] tmp = { 0 };
                this.DetectedPoint =tmp;
                this.DetectedS = "Nie wykryto migotania przedsionków";
                this.TimeofAF = "";
            }

        }
        private void detectAF (int[] RR, double fs)
        {
            switch (_method.Method)
            {
                case Detect_Method.STATISTIC:
                    detectAFStat(RR,fs);
                    break;
                case Detect_Method.POINCARE:
                    detectAFPoin(RR,fs);
                    break;
                default:
                    break;
            }
        }
       detectedAF detectAFStat(int[] RR, double fs )
        {
            detectedAF R = new detectedAF();
            return R;
            
        }
        detectedAF detectAFPoin(int[] RR, double fs)
        {
            detectedAF R = new detectedAF();
            return R;
        }

    }
}
