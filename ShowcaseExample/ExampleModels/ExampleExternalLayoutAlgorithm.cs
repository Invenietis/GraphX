using GraphX.GraphSharp.Algorithms.Layout;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphX.GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using System.Windows;
using System.Threading;

/*
 External layout algorithm implementation example
 
 Also shows how to use internal algorithms inside the external one.
 
 */
namespace ShowcaseExample
{
    public class ExampleExternalLayoutAlgorithm: IExternalLayout<DataVertex>
    {
        readonly IVertexAndEdgeListGraph<DataVertex, DataEdge> _graph;

        public ExampleExternalLayoutAlgorithm(IVertexAndEdgeListGraph<DataVertex, DataEdge> graph)
        {
            _graph = graph;
        }

        public IDictionary<DataVertex, Point> Compute( CancellationToken cancel, Func<DataVertex, Point> originalPosition, Func<DataVertex, Size> vertexSize = null )
        {
            var pars = new EfficientSugiyamaLayoutParameters { LayerDistance = 200 };
            var algo = new EfficientSugiyamaLayoutAlgorithm<DataVertex, DataEdge, IVertexAndEdgeListGraph<DataVertex, DataEdge>>( _graph, pars );
            return algo.Compute( cancel, originalPosition, vertexSize );
        }

        public bool NeedVertexSizes
        {
            get { return true; }
        }

        public bool NeedOriginalVertexPosition
        {
            get { return true; }
        }
    }
}
