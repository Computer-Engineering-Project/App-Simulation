﻿using Environment.Model.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.State_Management
{
    public class MainStateManagement
    {
        public event Action LoadHistory;
        public event Action IsRunningNow;
        public event Action IsIdleNow;
        public event Action IsPauseNow;
        public event Action<string> UpdateHistoryOut;
        public event Action<string> UpdateHitoryIn;
        public event Action<string> UpdateHistoryError;
        public event Action ResetAll;
        public event Action<object> ChangeMode;

        public event Action OpenChart;
        public void loadHistoryFromDB()
        {
            LoadHistory?.Invoke();
        }
        public void isRunningNow()
        {
            IsRunningNow?.Invoke();
        }
        public void isPauseNow()
        {
            IsPauseNow?.Invoke();
        }
        public void isIdleNow()
        {
            IsIdleNow?.Invoke();
        }
        public void changeMode(object data)
        {
            ChangeMode?.Invoke(data);
        }
        public void updateHistoryOut(string portName)
        {
            UpdateHistoryOut?.Invoke(portName);
        }
        public void updateHistoryIn(string portName)
        {
            UpdateHitoryIn?.Invoke(portName);
        }
        public void updateHistoryError(string portName)
        {
            UpdateHistoryError?.Invoke(portName);
        }
        public void resetAll()
        {
            ResetAll?.Invoke();
        }
        public void openChart()
        {
            OpenChart?.Invoke();
        }
    }
}
