using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MmsEngine.Support
{
    public class BindableBase
    {
        [JsonIgnore]
        internal MmsPlayer Player { get; set; }
        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event Action<string, string, object> PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, bool notify = true, [CallerMemberName] string propertyName = null, [CallerFilePath] string propertyPath = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            if (notify && !string.IsNullOrEmpty(propertyName))
            {
                if (Player == null) return true;

                var jsonAttribute = GetPropertyAttribute<JsonPropertyAttribute>(this, propertyName);                
                OnPropertyChanged(propertyName, jsonAttribute.PropertyName, value);
            }
            return true;
        }
        protected void OnPropertyChanged(string propertyName, string jsonPropertyName, object value) => PropertyChanged?.Invoke(propertyName, jsonPropertyName, value);

        private TAttribute GetPropertyAttribute<TAttribute>(object obj, string propertyName) where TAttribute : Attribute
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                return null;

            var propertyList = propertyInfo.GetCustomAttributes<TAttribute>().ToList();
            if (propertyList.Count > 0)
                return propertyInfo.GetCustomAttributes<TAttribute>().ToList()[0];
            else
                return null;
        }
    }
}