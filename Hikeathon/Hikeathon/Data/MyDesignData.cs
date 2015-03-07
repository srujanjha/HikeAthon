﻿using Lumia.Sense;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;


namespace Hikeathon.Data
{
    public class MyDesignData : INotifyPropertyChanged
    {
        private List<MyQuantifiedData> _ListData = null;
        private static MyDesignData _self;

        // time window index, 0 = today, -1 = yesterday 
        private double timeWindowIndex = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MyDesignData()
        {
            _ListData = new List<MyQuantifiedData>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _ListData.Add(new MyQuantifiedData("Idle", new TimeSpan(13, 0, 0)));
                _ListData.Add(new MyQuantifiedData("Moving", new TimeSpan(4, 0, 0)));
                _ListData.Add(new MyQuantifiedData("Stationary", new TimeSpan(1, 0, 0)));
                _ListData.Add(new MyQuantifiedData("Walking", new TimeSpan(2, 0, 0)));
                _ListData.Add(new MyQuantifiedData("Running", new TimeSpan(3, 0, 0)));
            }
        }

        static public MyDesignData Instance()
        {
            if (_self == null)
                _self = new MyDesignData();
            return _self;
        }

        public string CurrentActivity
        {
            get
            {
                return "walking";
            }
        }

        public List<MyQuantifiedData> ListData
        {
            get
            {
                return _ListData;
            }
        }

        public double TimeWindow
        {
            get
            {
                return timeWindowIndex;
            }
        }

    }

}
