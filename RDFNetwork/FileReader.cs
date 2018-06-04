using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace RDFNetwork
{
    public static class FileReader
    {
        public static List<Sample> ReadFromFile( string pathToFile )
        {
            List<Sample> samples = new List<Sample>();
            StreamReader dataStream = new StreamReader( pathToFile );
            string dataLine;
            FileInfo fileInfo = new FileInfo( pathToFile );

            if (!fileInfo.Exists) throw new FileNotFoundException( "File not found." );
            if (fileInfo.Length == 0) throw new FormatException( "File cannot be empty." );

            try
            {
                while (( dataLine = dataStream.ReadLine() ) != null)
                {
                    List<double> doubles = dataLine.Split( ' ' )
                        .Select( str => double.Parse( str, CultureInfo.InvariantCulture ) )
                        .ToList();
                    samples.Add( new Sample( doubles.Take( doubles.Count - 1 ).ToList(), doubles.Last() ) );
                }
            }
            finally
            {
                dataStream.Close();
            }

            return samples;
        }
    }
}