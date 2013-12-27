using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GraphX
{
    static public class PointAndSizeExtension
    {

        /// <summary>
        /// True if the point has at least one <see cref="Double.IsNaN"/> coordinates.
        /// </summary>
        /// <param name="this">This point.</param>
        /// <returns>True when X or Y are not numbers.</returns>
        static public bool IsNaN( this Point @this )
        {
            return Double.IsNaN( @this.X ) || Double.IsNaN( @this.Y );
        }

        /// <summary>
        /// True if the size has <see cref="Double.IsNaN"/> width or height.
        /// </summary>
        /// <param name="this">This size.</param>
        /// <returns>True when Width or Height are not numbers.</returns>
        static public bool IsNaN( this Size @this )
        {
            return Double.IsNaN( @this.Width ) || Double.IsNaN( @this.Height );
        }

        /// <summary>
        /// True if the point has <see cref="Double.IsInfinity"/> coordinates.
        /// </summary>
        /// <param name="this">This point.</param>
        /// <returns>True when X or Y are infinite.</returns>
        static public bool IsInfinity( this Point @this )
        {
            return Double.IsInfinity( @this.X ) || Double.IsInfinity( @this.Y );
        }

        /// <summary>
        /// True if the size has <see cref="Double.IsInfinity"/> width or height.
        /// </summary>
        /// <param name="this">This size.</param>
        /// <returns>True when Width or Height are infinite.</returns>
        static public bool IsInfinity( this Size @this )
        {
            return Double.IsInfinity( @this.Width ) || Double.IsInfinity( @this.Height );
        }

        /// <summary>
        /// True if the point is not <see cref="PointAndSizeExtension.IsInfinity"/> nor <see cref="PointAndSizeExtension.IsNaN"/>.
        /// </summary>
        /// <param name="this">This point.</param>
        /// <returns>True when X and Y are numbers not infinite valid numbers .</returns>
        static public bool IsValid( this Point @this )
        {
            return !IsNaN( @this ) && !IsInfinity( @this );
        }

        /// <summary>
        /// True if the size is not <see cref="PointAndSizeExtension.IsInfinity"/> nor <see cref="PointAndSizeExtension.IsNaN"/>.
        /// </summary>
        /// <param name="this">This size.</param>
        /// <returns>True when Width and Height are numbers not infinite valid numbers .</returns>
        static public bool IsValid( this Size @this )
        {
            return !IsNaN( @this ) && !IsInfinity( @this );
        }
    }
}
