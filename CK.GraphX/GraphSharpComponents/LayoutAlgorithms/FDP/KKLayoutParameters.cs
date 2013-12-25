﻿namespace GraphX.GraphSharp.Algorithms.Layout.Simple.FDP
{
	public class KKLayoutParameters : LayoutParametersBase
	{
		private double width = 300;
		/// <summary>
		/// Width of the bounding box.
		/// </summary>
		public double Width
		{
			get { return width; }
			set
			{
				width = value;
				RaisePropertyChanged("Width");
			}
		}

		private double height = 300;
		/// <summary>
		/// Height of the bounding box.
		/// </summary>
		public double Height
		{
			get { return height; }
			set
			{
				height = value;
				RaisePropertyChanged("Height");
			}
		}

		private int maxIterations = 200;
		/// <summary>
		/// Maximum number of the iterations.
		/// </summary>
		public int MaxIterations
		{
			get { return maxIterations; }
			set
			{
				maxIterations = value;
				RaisePropertyChanged("MaxIterations");
			}
		}

		private double _k = 1;
		public double K
		{
			get { return _k; }
			set
			{
				_k = value;
				RaisePropertyChanged("K");
			}
		}


		private bool adjustForGravity;
		/// <summary>
		/// If true, then after the layout process, the vertices will be moved, so the barycenter will be
		/// in the center point of the bounding box.
		/// </summary>
		public bool AdjustForGravity
		{
			get { return adjustForGravity; }
			set
			{
				adjustForGravity = value;
				RaisePropertyChanged("AdjustForGravity");
			}
		}

		private bool exchangeVertices;
		public bool ExchangeVertices
		{
			get { return exchangeVertices; }
			set
			{
				exchangeVertices = value;
				RaisePropertyChanged("ExchangeVertices");
			}
		}

		private double lengthFactor = 1;
		/// <summary>
		/// Multiplier of the ideal edge length. (With this parameter the user can modify the ideal edge length).
		/// </summary>
		public double LengthFactor
		{
			get { return lengthFactor; }
			set
			{
				lengthFactor = value;
				RaisePropertyChanged("LengthFactor");
			}
		}

		private double disconnectedMultiplier = 0.5;
		/// <summary>
		/// Ideal distance between the disconnected points (1 is equal the ideal edge length).
		/// </summary>
		public double DisconnectedMultiplier
		{
			get { return disconnectedMultiplier; }
			set
			{
				disconnectedMultiplier = value;
				RaisePropertyChanged("DisconnectedMultiplier");
			}
		}
	}
}