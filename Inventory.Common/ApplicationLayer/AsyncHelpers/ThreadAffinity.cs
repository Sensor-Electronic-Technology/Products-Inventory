using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer.AsyncHelpers {
    public struct ThreadAffinity {
        private int? _managedThreadId;

        public static ThreadAffinity BindToCurrentThread() {
            return new ThreadAffinity { _managedThreadId = Environment.CurrentManagedThreadId };
        }

        public void VerifyCurrentThread() {
            if(_managedThreadId != Environment.CurrentManagedThreadId)
                throw new InvalidOperationException("The calling thread cannot access this object because a different thread owns it.");
        }
    }
}
