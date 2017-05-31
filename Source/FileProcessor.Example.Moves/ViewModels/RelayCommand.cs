//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Example.Moves
//    File Name: RelayCommand.cs
//  
//    Author:    Andrew - 2017/05/30
//  ---------------------------------------------

namespace FileProcessor.Example.Moves.ViewModels
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        #region Fields/Properties

        private readonly Action _executionHandler;
        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;

                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Constructors

        public RelayCommand(Action handler)
        {
            _executionHandler = handler;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public void Execute(object parameter)
        {
            _executionHandler();
        }

        #endregion
    }


    public class RelayCommand<T> : ICommand
    {
        #region Fields/Properties

        private readonly Action<T> _parameterHandler;

        private bool _isEnabled = true;

        /// <summary>
        ///     Provides the ability to Enable/Disable the button control
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;

                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Constructors

        public RelayCommand(Action<T> handler)
        {
            _parameterHandler = handler;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public void Execute(object parameter)
        {
            Execute((T) parameter);
        }

        public void Execute(T parameter)
        {
            _parameterHandler(parameter);
        }

        #endregion
    }
}