using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolUX.LogViewer
{
    public static class LogModelExtensions
    {
        public static void RefreshProperties(this Models.LogEntry logEntry)
        {
            if (logEntry == null) return;

            // Force a refresh on the UI helper properties by explicitly calling OnPropertyChanged
            // This provides an escape hatch if binding isn't working automatically
            var propertyChanged = logEntry.GetType().GetEvent("PropertyChanged");
            var propertyChangedFieldInfo = logEntry.GetType().GetField("PropertyChanged",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (propertyChangedFieldInfo != null)
            {
                var propertyChangedDelegate = propertyChangedFieldInfo.GetValue(logEntry) as PropertyChangedEventHandler;

                if (propertyChangedDelegate != null)
                {
                    // Notify about all the UI helper properties
                    propertyChangedDelegate.Invoke(logEntry, new PropertyChangedEventArgs("HasVariables"));
                    propertyChangedDelegate.Invoke(logEntry, new PropertyChangedEventArgs("HasException"));
                    propertyChangedDelegate.Invoke(logEntry, new PropertyChangedEventArgs("HasExceptionData"));
                    propertyChangedDelegate.Invoke(logEntry, new PropertyChangedEventArgs("HasInnerExceptions"));
                    propertyChangedDelegate.Invoke(logEntry, new PropertyChangedEventArgs("HasStackTrace"));
                }
            }
        }
    }
}
