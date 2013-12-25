using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GraphX
{
    static public class PointAndSizeExtension
    {

        static public bool IsDefined( this Point @this )
        {
            return !Double.IsNaN( @this.X ) && !Double.IsNaN( @this.Y );
        }
        
        static public bool IsDefined( this Size @this )
        {
            return !Double.IsNaN( @this.Width ) && !Double.IsNaN( @this.Height );
        }
    }
}
