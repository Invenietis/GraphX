﻿namespace GraphX.GraphSharp.Algorithms.Layout.Compound.FDP
{
    public class CompoundFDPLayoutParameters : LayoutParametersBase
    {
        private double _idealEdgeLength = 25;
        private double _elasticConstant = 0.005;
        private double _repulsionConstant = 150;
        private double _nestingFactor = 0.2;
        private double _gravitationFactor = 8;

        private int _phase1Iterations = 50;
        private int _phase2Iterations = 70;
        private int _phase3Iterations = 30;

        private double _phase2TemperatureInitialMultiplier = 0.5;
        private double _phase3TemperatureInitialMultiplier = 0.2;

        private double _temperatureDecreasing = 0.5;
        private double _temperatureFactor = 0.95;
        private double _displacementLimitMultiplier = 0.5;
        private double _separationMultiplier = 15;

        /// <summary>
        /// Gets or sets the ideal edge length.
        /// </summary>
        public double IdealEdgeLength
        {
            get { return _idealEdgeLength; }
            set
            {
                if (value == _idealEdgeLength)
                    return;

                _idealEdgeLength = value;
                RaisePropertyChanged("IdealEdgeLength");
            }
        }

        /// <summary>
        /// Gets or sets the elastic constant for the edges.
        /// </summary>
        public double ElasticConstant
        {
            get { return _elasticConstant; }
            set
            {
                if (value == _elasticConstant)
                    return;

                _elasticConstant = value;
                RaisePropertyChanged("ElasticConstant");
            }
        }

        /// <summary>
        /// Gets or sets the repulsion constant for the node-node 
        /// repulsion.
        /// </summary>
        public double RepulsionConstant
        {
            get { return _repulsionConstant; }
            set
            {
                if (value == _repulsionConstant)
                    return;

                _repulsionConstant = value;
                RaisePropertyChanged("RepulsionConstant");
            }
        }

        /// <summary>
        /// Gets or sets the factor of the ideal edge length for the 
        /// inter-graph edges.
        /// </summary>
        public double NestingFactor
        {
            get { return _nestingFactor; }
            set
            {
                if (value == _nestingFactor)
                    return;

                _nestingFactor = value;
                RaisePropertyChanged("NestingFactor");
            }
        }

        /// <summary>
        /// Gets or sets the factor of the gravitation.
        /// </summary>
        public double GravitationFactor
        {
            get { return _gravitationFactor; }
            set
            {
                if (value == _gravitationFactor)
                    return;

                _gravitationFactor = value;
                RaisePropertyChanged("GravitationFactor");
            }
        }

        public int Phase1Iterations
        {
            get { return _phase1Iterations; }
            set
            {
                if (value == _phase1Iterations)
                    return;

                _phase1Iterations = value;
                RaisePropertyChanged("Phase1Iterations");
            }
        }

        public int Phase2Iterations
        {
            get { return _phase2Iterations; }
            set
            {
                if (value == _phase2Iterations)
                    return;

                _phase2Iterations = value;
                RaisePropertyChanged("Phase2Iterations");
            }
        }

        public int Phase3Iterations
        {
            get { return _phase3Iterations; }
            set
            {
                if (value == _phase3Iterations)
                    return;

                _phase3Iterations = value;
                RaisePropertyChanged("Phase3Iterations");
            }
        }

        public double Phase2TemperatureInitialMultiplier
        {
            get { return _phase2TemperatureInitialMultiplier; }
            set
            {
                if (value == _phase2TemperatureInitialMultiplier)
                    return;

                _phase2TemperatureInitialMultiplier = value;
                RaisePropertyChanged("Phase2TemperatureInitialMultiplier");
            }
        }

        public double Phase3TemperatureInitialMultiplier
        {
            get { return _phase3TemperatureInitialMultiplier; }
            set
            {
                if (value == _phase3TemperatureInitialMultiplier)
                    return;

                _phase3TemperatureInitialMultiplier = value;
                RaisePropertyChanged("Phase3TemperatureInitialMultiplier");
            }
        }

        public double TemperatureDecreasing
        {
            get { return _temperatureDecreasing; }
            set
            {
                if (value == _temperatureDecreasing)
                    return;

                _temperatureDecreasing = value;
                RaisePropertyChanged("TemperatureDecreasing");
            }
        }

        public double TemperatureFactor
        {
            get { return _temperatureFactor; }
            set
            {
                if (value == _temperatureFactor)
                    return;

                _temperatureFactor = value;
                RaisePropertyChanged("TemperatureFactor");
            }
        }

        public double DisplacementLimitMultiplier
        {
            get { return _displacementLimitMultiplier; }
            set
            {
                if (value == _displacementLimitMultiplier)
                    return;

                _displacementLimitMultiplier = value;
                RaisePropertyChanged("DisplacementLimitMultiplier");
            }
        }

        public double SeparationMultiplier
        {
            get { return _separationMultiplier; }
            set
            {
                if (value == _separationMultiplier)
                    return;

                _separationMultiplier = value;
                RaisePropertyChanged("SeparationMultiplier");
            }
        }
    }
}
