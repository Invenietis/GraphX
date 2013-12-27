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
        readonly Random _random;
        TGraph _graph;
        IDictionary<TVertex, Point> _positions;
        Dictionary<TVertex, Size> _sizes;
        CancellationToken _cancel;

        protected LayoutAlgorithmBase()
        {
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

        public IDictionary<TVertex, Point> Compute( CancellationToken cancel, TGraph graph, Func<TVertex, Point> originalPosition, Func<TVertex, Size> vertexSize = null )
        {
            if( graph == null ) throw new ArgumentNullException( "graph" );
            if( NeedOriginalVertexPosition && originalPosition == null ) throw new InvalidOperationException();
            if( NeedVertexSizes && vertexSize == null ) throw new InvalidOperationException();
            _graph = graph;
            _cancel = cancel;
            InternalPreCompute();
            if( originalPosition != null )
            {
                _positions = _graph.Vertices.ToDictionary( v => v, v => OnOriginalPosition( v, originalPosition( v ) ) );
            }
            if( vertexSize != null )
            {
                _sizes = _graph.Vertices.ToDictionary( v => v, v => OnOriginalSize( v, vertexSize( v ) ) );
            }
            InternalCompute();
            var p = _positions;
            //_positions = null;
            //_sizes = null;
            InternalPostCompute();
            return p;
        }

		protected TGraph VisitedGraph
		{
			get { return _graph; }
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
            return p.IsValid() ? p : new Point( _random.Next( 0, 2000 ), _random.Next( 0, 2000 ) );
        }

        protected virtual Size OnOriginalSize( TVertex v, Size s )
        {
            return s.IsValid() ? s : new Size( 0, 0 );
        }

        protected virtual void InternalPreCompute()
        {
        }

        protected abstract void InternalCompute();

        protected virtual void InternalPostCompute()
        {
        }

        protected IDictionary<TVertex, Point> VertexPositions { get { return _positions; } }

        protected IDictionary<TVertex, Size> VertexSizes { get { return _sizes; } }
    }
}