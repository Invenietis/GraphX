using GraphX.GraphSharp.Algorithms.EdgeRouting;
using GraphX.GraphSharp.Algorithms.Layout;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphX
{
    public sealed class AlgorithmStorage<TVertex, TEdge>
    {
        public IExternalOverlapRemoval<TVertex> OverlapRemoval { get; private set; }
        public IExternalEdgeRouting<TVertex, TEdge> EdgeRouting { get; private set; }

        public AlgorithmStorage( IExternalOverlapRemoval<TVertex> or, IExternalEdgeRouting<TVertex, TEdge> er)
        {
            OverlapRemoval = or; EdgeRouting = er;
        }
    }
}
