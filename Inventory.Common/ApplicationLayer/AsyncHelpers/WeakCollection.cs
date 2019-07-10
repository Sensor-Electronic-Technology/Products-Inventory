using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {
    internal sealed class WeakCollection<T> where T : class {

        private readonly List<WeakReference<T>> _list = new List<WeakReference<T>>();

        public List<T> GetLiveItems() {
            var ret = new List<T>(_list.Count);

            int writeIndex = 0;
            for(int readIndex = 0; readIndex != _list.Count; ++readIndex) {
                WeakReference<T> weakReference = _list[readIndex];
                T item;
                if(weakReference.TryGetTarget(out item)) {
                    ret.Add(item);

                    if(readIndex != writeIndex)
                        _list[writeIndex] = _list[readIndex];

                    ++writeIndex;
                }
            }

            _list.RemoveRange(writeIndex, _list.Count - writeIndex);

            return ret;
        }

        public void Add(T item) {
            _list.Add(new WeakReference<T>(item));
        }

        public bool Remove(T item) {
            for(int i = 0; i != _list.Count; ++i) {
                var weakReference = _list[i];
                T entry;
                if(weakReference.TryGetTarget(out entry) && entry == item) {
                    _list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
    }
}
