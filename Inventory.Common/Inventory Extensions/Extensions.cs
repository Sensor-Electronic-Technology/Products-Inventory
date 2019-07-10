using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer {
    public static class EnumHelper {

        /// <summary>
        /// Returns description attribute of enum.  If description not foud returns null
        /// </summary>
        /// <typeparam name="T">Enum Type</typeparam>
        /// <param name="e">Enum to get description from</param>
        /// <returns></returns>
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;
            if (e is Enum) {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);
                foreach (int val in values) {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture)) {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAtt = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAtt.Length > 0) {
                            description = ((DescriptionAttribute)descriptionAtt[0]).Description;
                        }
                        break;
                    }
                }
            }
            return description;
        }
    }

    /// <summary>
    /// Gets enum from string.  string much be a description of a known user defined enum
    /// if can't find description returns default parameter
    /// </summary>
    public static class StringExtensions {

        public static T GetEnum<T>(this string value, T defaultIfFail) where T : IConvertible
        {
            if (string.IsNullOrEmpty(value)) {
                return defaultIfFail;
            }

            Type type = typeof(T);
            Array values = Enum.GetValues(type);
            foreach(T val in values) {

                var memInfo = type.GetMember(type.GetEnumName(val));
                var descriptionAtt = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptionAtt.Length > 0) {
                    if(value== ((DescriptionAttribute)descriptionAtt[0]).Description) {
                        return val;
                    }
                }
            }
            return defaultIfFail;
        }
    }
}
