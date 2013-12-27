using GraphX.GraphSharp;
using GraphX.GraphSharp.Algorithms.EdgeRouting;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphX.GraphSharpComponents.EdgeRouting;

namespace GraphX.Models
{
    public sealed class AlgorithmFactory<TVertex, TEdge, TGraph>
        where TVertex : VertexBase
        where TEdge : EdgeBase<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        #region OverlapRemoval factory

        public IOverlapRemovalAlgorithm<TVertex> CreateOverlapRemovalAlgorithm(OverlapRemovalAlgorithmTypeEnum newAlgorithmType, IDictionary<TVertex, Rect> Rectangles, IOverlapRemovalParameters parameters = null)
        {
            //if (Rectangles == null) return null;
            if (parameters == null) parameters = CreateOverlapRemovalParameters(newAlgorithmType);

            switch (newAlgorithmType)
            {
                case OverlapRemovalAlgorithmTypeEnum.FSA:
                    return new FSAAlgorithm<TVertex>(Rectangles, parameters);
                case OverlapRemovalAlgorithmTypeEnum.OneWayFSA:
                    return new OneWayFSAAlgorithm<TVertex>(Rectangles, parameters as OneWayFSAParameters);
                default:
                    return null;
            }
        }

        public IOverlapRemovalParameters CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum algorithmType)
        {
            switch (algorithmType)
            {
                case OverlapRemovalAlgorithmTypeEnum.FSA:
                    return new OverlapRemovalParameters();
                case OverlapRemovalAlgorithmTypeEnum.OneWayFSA:
                    return new OneWayFSAParameters();
                default:
                    return null;
            }
        }


        #endregion

        #region Edge Routing factory
        public IExternalEdgeRouting<TVertex, TEdge> CreateEdgeRoutingAlgorithm(EdgeRoutingAlgorithmTypeEnum newAlgorithmType, Rect graphArea, TGraph Graph, IDictionary<TVertex, Point> Positions, IDictionary<TVertex, Rect> Rectangles, IEdgeRoutingParameters parameters = null)
        {
            //if (Rectangles == null) return null;
            if (parameters == null) parameters = CreateEdgeRoutingParameters(newAlgorithmType);

            switch (newAlgorithmType)
            {
                case EdgeRoutingAlgorithmTypeEnum.SimpleER:
                    return new SimpleEdgeRouting<TVertex, TEdge,  TGraph>(Graph, Positions, Rectangles, parameters);
                case EdgeRoutingAlgorithmTypeEnum.Bundling:
                    return new BundleEdgeRouting<TVertex, TEdge, TGraph>(graphArea, Graph, Positions, Rectangles, parameters);
                case EdgeRoutingAlgorithmTypeEnum.PathFinder:
                    return new PathFinderEdgeRouting<TVertex, TEdge, TGraph>(Graph, Positions, Rectangles, parameters);
                default:
                    return null;
            }
        }

        public IEdgeRoutingParameters CreateEdgeRoutingParameters(EdgeRoutingAlgorithmTypeEnum algorithmType)
        {
            switch (algorithmType)
            {
                case EdgeRoutingAlgorithmTypeEnum.SimpleER:
                    return new SimpleERParameters();
                case EdgeRoutingAlgorithmTypeEnum.Bundling:
                    return new BundleEdgeRoutingParameters();
                case EdgeRoutingAlgorithmTypeEnum.PathFinder:
                    return new PathFinderEdgeRoutingParameters();
                default:
                    return null;
            }
        }
        #endregion
    }
}
