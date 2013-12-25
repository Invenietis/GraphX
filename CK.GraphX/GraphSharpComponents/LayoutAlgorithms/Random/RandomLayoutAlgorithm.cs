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
        readonly Random _random = new Random();

        public RandomLayoutAlgorithm(TGraph graph)
            : base( graph )
        {
        }

        public bool RandomOnlyNewVertices { get; set; }

        protected override Point OnOriginalPosition( TVertex v, Point p )
        {
            if( RandomOnlyNewVertices )
            {
                return new Point( _random.Next( 0, 2000 ), _random.Next( 0, 2000 ) );
            }
            return p.IsDefined() ? p : new Point( _random.Next( 0, 2000 ), _random.Next( 0, 2000 ) );
        }

        protected override void InternalCompute()
        {
        }
    }
}
