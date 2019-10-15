using System;
using System.Collections.Generic;
using System.Text;

namespace Minesolver
{
    public interface IClonable<TOut>
    {
        TOut Clone();
    }
}
