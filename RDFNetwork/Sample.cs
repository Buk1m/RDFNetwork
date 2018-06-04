using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDFNetwork
{
    public class Sample
    {
        public List<double> Inputs { get; set; }
        public List<double> ExpectedValues { get; set; }
        public List<int> Classification { get; set; } = new List<int>();

        public Sample( List<double> input, List<double> expected )
        {
            Inputs = input;
            ExpectedValues = expected;
        }

        public Sample( List<double> inputList )
        {
            Inputs = inputList;
        }

        public Sample( List<double> inputList, double expected )
        {
            Inputs = inputList;
            ExpectedValues = new List<double> { expected };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append( "\nInput values:" );
            if ( Inputs.Any() )
            {
                foreach ( var el in Inputs )
                {
                    sb.Append( el + " " );
                }
            }

            if ( !ExpectedValues.Any() ) return sb.ToString();
            sb.Append( "\nExpected values:" );
            foreach ( var expectedValue in ExpectedValues )
            {
                sb.Append( expectedValue + " " );
            }

            return sb.ToString();
        }
    }
}