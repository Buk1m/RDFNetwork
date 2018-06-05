﻿using System;
using System.Collections.Generic;

namespace RDFNetwork.RBFClassification.Model
{
    public class Neuron
    {
        public double Bias { get; set; }
        public List<double> Inputs { get; set; }
        public List<double> Weights { get; set; } = new List<double>();
        private double _totalNetInput;
        public double Output { get; set; }

        public Neuron( double bias = 1 )
        {
            Bias = bias;
        }

        public double CalculateOutput()
        {
            CalcTotalNetInput();
            Output = _totalNetInput;
            return Output;
        }

        private void CalcTotalNetInput()
        {
            _totalNetInput = Bias;
            for ( int i = 0; i < Inputs.Count; i++ )
            {
                _totalNetInput += Weights[i] * Inputs[i];
            }
        }

        public double DerivativeFu()
        {
            return Output * (1 - Output);
        }

        public double ErrorBackPropagationLinear( double expectedValue )
        {
            return (Output - expectedValue);
        }

        public double CalculateError( double expectedValue )
        {
            return (double)1 / Inputs.Count * Math.Pow( (expectedValue - Output), 2 );
        }
    }
}