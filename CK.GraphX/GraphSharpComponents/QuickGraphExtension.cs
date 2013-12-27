using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace GraphX
{
    public static class QuickGraphExtension
    {
        public static bool IsSelfEdge<TVertex>( this IEdge<TVertex> @this )
        {
            return @this.IsSelfEdge<TVertex,IEdge<TVertex>>();
        }
    }
}
