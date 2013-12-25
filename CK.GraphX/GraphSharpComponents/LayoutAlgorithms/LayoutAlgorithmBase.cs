using System.Collections.Generic;
using QuickGraph;
using System.Windows;
using System.Threading;
using System;
using System.Linq;

namespace GraphX.GraphSharp.Algorithms.Layout
{
	
	public abstract class LayoutAlgorithmBase<TVertex, TEdge, TGraph> : ILayoutAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		readonly TGraph _visitedGraph;
        readonly Random _random;
        IDictionary<TVertex, Point> _positions;
        Dictionary<TVertex, Size> _sizes;
        CancellationToken _cancel;

		public TGraph VisitedGraph
		{
			get { return _visitedGraph; }
		}

		protected LayoutAlgorithmBase( TGraph visitedGraph ) 
		{
            _visitedGraph = visitedGraph;
            _random = new Random();
		}

        public virtual bool NeedVertexSizes
        {
            get { return false; }
        }

        public virtual bool NeedOriginalVertexPosition
        {
            get { return false; }
        }

        public IDictionary<TVertex, Point> Compute( CancellationToken cancel, Func<TVertex, Point> originalPosition, Func<TVertex, Size> vertexSize = null )
        {
            if( NeedOriginalVertexPosition && originalPosition == null ) throw new InvalidOperationException();
            if( NeedVertexSizes && vertexSize == null ) throw new InvalidOperationException();
            _cancel = cancel;
            if( originalPosition != null )
            {
                _positions = _visitedGraph.Vertices.ToDictionary( v => v, v => OnOriginalPosition( v, originalPosition( v ) ) );
            }
            if( vertexSize != null )
            {
                _sizes = _visitedGraph.Vertices.ToDictionary( v => v, v => OnOriginalSize( v, vertexSize( v ) ) );
            }
            InternalCompute();
            return _positions;
        }

        protected CancellationToken CancelToken
        {
            get { return _cancel; }
        }

        protected Random Randomizer
        {
            get { return _random; }
        }

        protected virtual Point OnOriginalPosition( TVertex v, Point p )
        {
            return p.IsDefined() ? p : new Point( _random.Next( 0, 2000 ), _random.Next( 0, 2000 ) );
        }

        protected virtual Size OnOriginalSize( TVertex v, Size s )
        {
            return s.IsDefined() ? s : new Size( 1, 1 );
        }

        protected abstract void InternalCompute();

        protected IDictionary<TVertex, Point> VertexPositions { get { return _positions; } }

        protected IDictionary<TVertex, Size> VertexSizes { get { return _sizes; } }
    }
}