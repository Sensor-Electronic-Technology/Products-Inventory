using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {
    public sealed class PropertyChangedEventArgsCache {
        private readonly Dictionary<string, PropertyChangedEventArgs> _cache = new Dictionary<string, PropertyChangedEventArgs>();

        private PropertyChangedEventArgsCache() {

        }

        public static PropertyChangedEventArgsCache Instance { get; } = new PropertyChangedEventArgsCache();

        public PropertyChangedEventArgs Get(string propertyName) {
            lock(_cache) {
                PropertyChangedEventArgs result;
                if(this._cache.TryGetValue(propertyName,out result)) {
                    return result;
                }
                result = new PropertyChangedEventArgs(propertyName);
                this._cache.Add(propertyName, result);
                return result;
            }
        }
    }
}
