using GraphX.GraphSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using QuickGraph;

namespace GraphX
{
    /// <summary>
    /// Base class for graph edge
    /// </summary>
    /// <typeparam name="TVertex">Vertex class</typeparam>
    public abstract class EdgeBase<TVertex>: Edge<TVertex>, IIdentifiableGraphDataObject, IRoutingInfo
    {
        public EdgeBase(TVertex source, TVertex target)
            : base(source, target)
        {
            ID = -1;
        }

        /// <summary>
        /// Unique edge ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Returns true if Source vertex equals Target vertex
        /// </summary>
        public bool IsSelfLoop
        {
            get { return Source.Equals(Target); }
        }

        /// <summary>
        /// Routing points collection used to make Path visual object
        /// </summary>
        public Point[] RoutingPoints { get; set; }
    }
}
