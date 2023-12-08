/******************************************************************************
* Filename    = ViewModel.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the definition of the ViewModel class, which
*               implements the INotifyPropertyChanged interface. It is part of
*               the MessengerDashboard project and is used to handle property
*               changes and notifications in the UI.
*****************************************************************************/

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MessengerDashboard.UI.ViewModels
{
    /// <summary>
    /// Represents the base ViewModel class that implements the INotifyPropertyChanged interface.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of the specified property and notifies listeners of the change.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">The reference to the backing field of the property.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property (automatically provided).</param>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Invokes the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property (automatically provided).</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
