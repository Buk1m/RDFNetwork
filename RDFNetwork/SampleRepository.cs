using System.Collections.Generic;
using System.Text;
using IAD_zadanie02;

namespace RDFNetwork
{
    public static class SampleRepository
    {
        public static List<Sample> TrainSamples = new List<Sample>();
        public static List<Sample> TestSamples = new List<Sample>();

#pragma warning disable 108,114
        public static string ToString()
#pragma warning restore 108,114
        {
            var sb = new StringBuilder();
            TrainSamples.ForEach( sample => sb.Append( sample.ToString() ) );
            return sb.ToString();
        }

        public static List<SamplePoint> GetInputSamplePoints()
        {
            List<SamplePoint> samplePoints = new List<SamplePoint>();
            foreach ( var trainSample in SampleRepository.TrainSamples )
            {
                trainSample.Inputs.ForEach( input => samplePoints.Add( new SamplePoint( input ) ) );
            }

            return samplePoints;
        }
    }
}

