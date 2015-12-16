using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histogram.HelperClasses
{
    class Converters
    {
        public static void StringArrayToDoubleList(
            string[] array, 
            out List<double> list,
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol, 
            String cultureInfo = "en-US")
        {
            if (array == null || array.Length == 0)
            {
                list = null;
                return;
            }

            list = new List<double>();
            var culture = CultureInfo.CreateSpecificCulture(cultureInfo);
            foreach (String line in array)
            {
                float tmp;
                if (float.TryParse(line, style, culture, out tmp))
                {
                    list.Add(tmp);
                }
                else
                {
                    throw new System.Exception("Niepoprawne dane");
                }
            }
        }

    }
}
