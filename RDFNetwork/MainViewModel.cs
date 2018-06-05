using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using OxyPlot;

using RDFNetwork.Commands;

namespace RDFNetwork
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            _taskName = "Choose Task";
        }

        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                OnPropertyChanged();
            }
        }

        public ICommand ApproximationClick { get; }

        private string _taskName;
    }
}