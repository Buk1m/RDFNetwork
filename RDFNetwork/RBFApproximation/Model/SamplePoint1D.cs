using System.Windows;

namespace IAD_zadanie02
{
    public class SamplePoint1D 
    {
        public int NearsetPointId { get; internal set; }
        public double X { get; set; }


        #region Constructor

        public SamplePoint1D( double x )
        {
            X = x;
        }

        #endregion
    }
}