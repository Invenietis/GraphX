using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace GraphX.GraphSharp.Algorithms.Layout
{
    public interface IExternalLayout<TVertex>
    {
        /// <summary>
        /// Run algorithm calculation.
        /// </summary>
        IDictionary<TVertex, Point> Compute( CancellationToken cancel, Func<TVertex, Point> originalPosition, Func<TVertex, Size> vertexSize = null );

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
