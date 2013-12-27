using System.Collections.Generic;
using QuickGraph.Algorithms;
using QuickGraph;
using System.Windows;
using System.Threading;
using System;

namespace GraphX.GraphSharp.Algorithms.Layout
{
    public interface ILayoutAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
        /// <summary>
        /// Run algorithm calculation.
        /// </summary>
        IDictionary<TVertex, Point> Compute( CancellationToken cancel, TGraph graph, Func<TVertex, Point> originalPosition, Func<TVertex, Size> vertexSize = null );

        /// <summary>
        /// If algorithm needs to know visual vertex control sizes, the accessor will be provided to Compute.
        /// </summary>
        bool NeedVertexSizes { get; }

        /// <summary>
        /// If algorithm needs to know the visual vertex control original position, the accessor will be provided to Compute.
        /// </summary>
        bool NeedOriginalVertexPosition { get; }
    }
}