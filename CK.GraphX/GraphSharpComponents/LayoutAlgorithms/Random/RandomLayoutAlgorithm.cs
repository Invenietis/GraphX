﻿using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GraphX.GraphSharp.Algorithms.Layout
{
    public class RandomLayoutAlgorithm<TVertex, TEdge, TGraph> : ILayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private TGraph Graph;
        private Random Rnd = new Random((int)DateTime.Now.Ticks);

        public RandomLayoutAlgorithm(TGraph graph)
        {
            Graph = graph;
        }        
    
        public void Compute()
        {
            foreach (var item in Graph.Vertices)
                vertexPositions.Add(item, new Point(Rnd.Next(0, 2000), Rnd.Next(0, 2000)));
        }

        IDictionary<TVertex, Point> vertexPositions = new Dictionary<TVertex, Point>();

        public IDictionary<TVertex, Point> VertexPositions { get { return vertexPositions; } }

        public IDictionary<TVertex, Size> VertexSizes { get; set; }

        public bool NeedVertexSizes
        {
            get { return false; }
        }

        public TGraph VisitedGraph
        {
            get { return Graph; }
        }
    }
}
