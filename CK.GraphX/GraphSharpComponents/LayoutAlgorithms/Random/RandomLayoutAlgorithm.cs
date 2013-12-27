using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GraphX.GraphSharp.Algorithms.Layout
{
    public class RandomLayoutAlgorithm<TVertex, TEdge, TGraph> : LayoutAlgorithmBase<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        public bool AlsoRandomValidPosition { get; set; }

        protected override Point OnOriginalPosition( TVertex v, Point p )
        {
            if( AlsoRandomValidPosition )
            {
                return new Point( Randomizer.Next( 0, 2000 ), Randomizer.Next( 0, 2000 ) );
            }
            return p.IsValid() ? p : new Point( Randomizer.Next( 0, 2000 ), Randomizer.Next( 0, 2000 ) );
        }

        protected override void InternalCompute()
        {
        }
    }
}
