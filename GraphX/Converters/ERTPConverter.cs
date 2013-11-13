using System;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace GraphX.Converters
{
	/// <summary>
	/// Converts the position and sizes of the source and target points, and the route informations
	/// of an edge to a path.
	/// The edge can bend, or it can be straight line.
	/// </summary>
	public class ERTPConverter : IMultiValueConverter
	{
        protected const Int32 CurveThroughIntermediatePointsTolerance = 8;

		public object Convert( object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			Debug.Assert( values != null && values.Length == 10, "EdgeRouteToPathConverter should have 9 parameters: pos (1,2), size (3,4) of source; pos (5,6), size (7,8) of target; routeInformation (9)." );

			#region Get the inputs
			//get the position of the source
			Point sourcePos = new Point()
			                  	{
			                  		X = ( values[0] != DependencyProperty.UnsetValue ? (double)values[0] : 0.0 ),
			                  		Y = ( values[1] != DependencyProperty.UnsetValue ? (double)values[1] : 0.0 )
			                  	};
			//get the size of the source
			Size sourceSize = new Size()
			                  	{
			                  		Width = ( values[2] != DependencyProperty.UnsetValue ? (double)values[2] : 0.0 ),
			                  		Height = ( values[3] != DependencyProperty.UnsetValue ? (double)values[3] : 0.0 )
			                  	};
			//get the position of the target
			Point targetPos = new Point()
			                  	{
			                  		X = ( values[4] != DependencyProperty.UnsetValue ? (double)values[4] : 0.0 ),
			                  		Y = ( values[5] != DependencyProperty.UnsetValue ? (double)values[5] : 0.0 )
			                  	};
			//get the size of the target
			Size targetSize = new Size()
			                  	{
			                  		Width = ( values[6] != DependencyProperty.UnsetValue ? (double)values[6] : 0.0 ),
			                  		Height = ( values[7] != DependencyProperty.UnsetValue ? (double)values[7] : 0.0 )
			                  	};
            if (sourcePos == targetPos) return null;

			//get the route informations
			Point[] routeInformation = ( values[8] != DependencyProperty.UnsetValue ? (Point[])values[8] : null );
			#endregion

			bool hasRouteInfo = routeInformation != null && routeInformation.Length > 1;

            //calculate source and target edge attach points
            Point p1 = GraphHelper.CalculateAttachPoint(sourcePos, sourceSize, (hasRouteInfo ? routeInformation[0] : targetPos));
            Point p2 = GraphHelper.CalculateAttachPoint(targetPos, targetSize, (hasRouteInfo ? ((routeInformation[routeInformation.Length - 1] == targetPos) ? routeInformation[routeInformation.Length - 2] : routeInformation[routeInformation.Length - 1]) : sourcePos));

            //if we have route and route consist of 2 or more points
            if (hasRouteInfo)
            {
                //var ga = values[9] as GraphAreaBase;
                Vector v = routeInformation[routeInformation.Length - 2] - p2; v = v / v.Length * 5;
                Vector n = new Vector(-v.Y, v.X) * 0.7;
                
                var list = new PathSegment[1] { new PolyLineSegment(routeInformation, true) };
                var l = routeInformation.ToList(); l.Insert(0, p1); l.Add(p2);
                var pfc = GeometryHelper.GetCurveThroughPoints(l.ToArray(), 0.5, CurveThroughIntermediatePointsTolerance).Figures;
                pfc.Add(new PathFigure(p2, new PathSegment[] {
			            new LineSegment(p2 + v - n, true),
			            new LineSegment(p2 + v + n, true)}, true));
                GeometryHelper.TryFreeze(pfc);

                /*PathFigureCollection pfc = new PathFigureCollection(2)
                {
                    new PathFigure(p1, list, false),
                    new PathFigure(p2, new PathSegment[] {
			            new LineSegment(p2 + v - n, true),
			            new LineSegment(p2 + v + n, true)}, true)
                };*/

                return pfc;
            }
            else // no route defined
            {

                PathSegment[] segments = new PathSegment[1];           
                Vector v = p1 - p2; v = v / v.Length * 5;
                Vector n = new Vector(-v.Y, v.X) * 0.7;
                segments[0] = new LineSegment(p2 + v, true);

                PathFigureCollection pfc = new PathFigureCollection(2);
                pfc.Add(new PathFigure(p1, segments, false));
                pfc.Add(new PathFigure(p2,
                                         new PathSegment[] {
			                                           	new LineSegment(p2 + v - n, true),
			                                           	new LineSegment(p2 + v + n, true)}, true));

                return pfc;
            }
		}

		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}