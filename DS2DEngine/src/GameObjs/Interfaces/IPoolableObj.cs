using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public interface IPoolableObj
    {
        bool IsCheckedOut { get; set; }
        bool IsActive { get; set; }
    }
}
