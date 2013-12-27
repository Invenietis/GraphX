using System;
using System.Collections.Generic;
using QuickGraph;
using System.Linq;
using System.Diagnostics;
using System.Windows;

namespace GraphX.GraphSharp.Algorithms.Layout
{
    /// <summary>
    /// Use this class as a base class for your layout algorithm 
    /// if it's parameter class has a default constructor.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices.</typeparam>
    /// <typeparam name="TEdge">The type of the edges.</typeparam>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TParam">The type of the parameters. Must be based on the LayoutParametersBase.</typeparam>
    public abstract class DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParam> : ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParam>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
        where TParam : class, ILayoutParameters, new()
    {
        protected DefaultParameterizedLayoutAlgorithmBase() 
            : base( null )
        {
        }

        protected DefaultParameterizedLayoutAlgorithmBase( TParam oldParameters ) 
            : base( oldParameters)
        {
        }

        protected override TParam DefaultParameters
        {
            get { return new TParam(); }
        }
    }

	/// <typeparam name="TVertex">Type of the vertices.</typeparam>
	/// <typeparam name="TEdge">Type of the edges.</typeparam>
	/// <typeparam name="TGraph">Type of the graph.</typeparam>
	/// <typeparam name="TParam">Type of the parameters. Must be based on the LayoutParametersBase.</typeparam>
	public abstract class ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParam> : LayoutAlgorithmBase<TVertex, TEdge, TGraph>, IParameterizedLayoutAlgorithm<TParam>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
		where TParam : class, ILayoutParameters
	{
		#region Properties
		/// <summary>
		/// Parameters of the algorithm. For more information see <see cref="LayoutParametersBase"/>.
		/// </summary>
		public TParam Parameters { get; protected set; }
		public ILayoutParameters GetParameters()
		{
			return Parameters;
		}
		public TraceSource TraceSource { get; protected set; }
		#endregion

		protected ParameterizedLayoutAlgorithmBase( TParam oldParameters )
		{
			InitParameters( oldParameters );
			TraceSource = new TraceSource( "LayoutAlgorithm", SourceLevels.All );
		}

        protected abstract TParam DefaultParameters { get; } 

		/// <summary>
		/// Initializes the parameters (cloning or creating new parameter object with default values).
		/// </summary>
		/// <param name="oldParameters">Parameters from a previous layout. If it is null, 
		/// the parameters will be set to the default ones.</param>
		protected void InitParameters( TParam oldParameters )
		{
            if (oldParameters == null)
                Parameters = DefaultParameters;
            else
            {
                Parameters = (TParam)oldParameters.Clone();
            }
		}

		/// <summary>
		/// Initializes the positions of the vertices. Assign a random position inside the 'bounding box' to the vertices without positions.
		/// It does NOT modify the position of the other vertices.
		/// 
		/// It generates an <code>IterationEnded</code> event.
		/// 
		/// Bounding box:
		/// x coordinates: double.Epsilon - <code>width</code>
		/// y coordinates: double.Epsilon - <code>height</code>
		/// </summary>
		/// <param name="width">Width of the bounding box.</param>
		/// <param name="height">Height of the bounding box.</param>
		protected virtual void InitializeWithRandomPositions( double width, double height )
		{
			InitializeWithRandomPositions( width, height, 0, 0 );
		}

		/// <summary>
		/// Initializes the positions of the vertices. Assign a random position inside the 'bounding box' to the vertices without positions.
		/// It does NOT modify the position of the other vertices.
		/// 
		/// It generates an <code>IterationEnded</code> event.
		/// 
		/// Bounding box:
		/// x coordinates: double.Epsilon - <code>width</code>
		/// y coordinates: double.Epsilon - <code>height</code>
		/// </summary>
		/// <param name="width">Width of the bounding box.</param>
		/// <param name="height">Height of the bounding box.</param>
		/// <param name="xOffset">Translates the generated x coordinate.</param>
		/// <param name="yOffset">Translates the generated y coordinate.</param>
		protected virtual void InitializeWithRandomPositions( double width, double height, double xOffset, double yOffset )
		{
			var rnd = new Random( DateTime.Now.Millisecond );

			//initialize with random position
			foreach ( TVertex v in VisitedGraph.Vertices )
			{
				VertexPositions[v] =
					new Point(
						Math.Max( double.Epsilon, rnd.NextDouble() * width + xOffset ),
						Math.Max( double.Epsilon, rnd.NextDouble() * height + yOffset ) );
			}
		}

		protected virtual void NormalizePositions()
		{
			NormalizePositions( VertexPositions );
		}

		protected static void NormalizePositions( IDictionary<TVertex, Point> vertexPositions )
		{
			if ( vertexPositions == null || vertexPositions.Count == 0 )
				return;

			//get the topLeft position
			var topLeft = new Point( float.PositiveInfinity, float.PositiveInfinity );
			foreach ( var pos in vertexPositions.Values.ToArray() )
			{
				topLeft.X = Math.Min( topLeft.X, pos.X );
				topLeft.Y = Math.Min( topLeft.Y, pos.Y );
			}

			//translate with the topLeft position
			foreach ( var v in vertexPositions)
			{
				var pos = v.Value;
				pos.X -= topLeft.X;
				pos.Y -= topLeft.Y;
				vertexPositions[v.Key] = pos;
			}
		}
	}
}