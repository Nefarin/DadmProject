using System;
using System.Reflection;

namespace EKG_Project.Modules
{
    #region Documentation
    /// <summary>
    /// Basic parameters will be provided to modules in successors to this class (for example if R_Peaks should be calculated by Hilbert Transform or something else)
    /// </summary>
    /// 
    #endregion
    public class ModuleParams
    {
        public string AnalysisName { get; set; }
        public bool GUIParametersAvailable { get; set; }

        public ModuleParams()
        {
            this.AnalysisName = "undefined";
            this.GUIParametersAvailable = false;
        }

        /// <summary>
        /// Kopiuje wszystkie propercje z innego ModuleParams (musi byc tego samego typu).
        /// </summary>
        /// <param name="source">Zrodlo z ktorego kopiujemy</param>
        /// <returns>null jesli typy sie nie zgadzaja, this jesli wszystko poszlo dobrze.</returns>
        public ModuleParams CopyFrom(ModuleParams source)
        {
            Type sourceType = source.GetType();
            if (this.GetType() != sourceType)
                return null;

            PropertyInfo[] properties = sourceType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    property.SetValue(this, property.GetValue(source, null), null);
                }
            }

            return this;
        }
    }
}
