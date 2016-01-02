using Histogram.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV2
{

    #region Documentation
    /// <summary>
    /// Coś tu pomieszalam :(
    /// </summary>
    /// 
    #endregion
    public class Tinn
    {
        var Lista = new DataSource();
        ObservableCollection<Sample> Samples = from ObservableCollection < Sample > Count in Samples
                                                orderby Count
                                                select Count;
        ObesvableCollection valueMax = Sample.Last(); //maksymalna wartość

        public double _maxValue = valueMax;
        public double _allValue = Sample.Length;
        
        public double TINN ( _maxValue, _allValue)

        {
            double TINN = _allValue / _maxValue;
            return TINN;
        }

    }
}
