namespace BarbarianCall
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public static class DebugHelper
    {
        public static bool IsDebugBuild()
        {
            // Get the assembly of the current executing code
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the DebuggableAttribute for the assembly
            object[] attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);

            // Check if DebuggableAttribute exists
            if (attributes.Length > 0)
            {
                // Cast the attribute to DebuggableAttribute
                DebuggableAttribute debuggableAttribute = (DebuggableAttribute)attributes[0];

                // Check if the assembly was built in debug mode
                return debuggableAttribute.IsJITTrackingEnabled;
            }

            // If DebuggableAttribute is not found, assume it's not a debug build
            return false;
        }
    }
}
