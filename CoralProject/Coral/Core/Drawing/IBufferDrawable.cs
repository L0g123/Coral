using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core.Drawing
{
    public interface IBufferDrawable
    {
        public void DrawTo(SymbolBuffer buffer);
    }
}
