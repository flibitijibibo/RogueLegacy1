using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public interface IDisposableObj
    {
        bool IsDisposed { get; }
        void Dispose();
    }
}
