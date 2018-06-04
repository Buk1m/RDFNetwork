using System.Windows;

namespace IAD_zadanie02
{
    public class SamplePoint 
    {
        public int NearsetPointId { get; internal set; }
        public double X { get; set; }


        #region Constructor

        public SamplePoint( double x )
        {
            X = x;
        }

        #endregion
    }
}