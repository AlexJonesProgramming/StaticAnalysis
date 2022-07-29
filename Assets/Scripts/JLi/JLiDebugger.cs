#if !UNITY_WEBGL
using System.Diagnostics;
using System.Reflection;
#endif

namespace JLi_Utility
{
    /// <summary>
    /// My debugger static class.  Use like Unity's Debug.Log/LogWarning/LogError
    /// Basically displays the class and method where these functions were called from and appends the message.
    /// </summary>
    public static class Debugger
    {
        private static string myFormat        = "<color={0}>{1}</color>";
        private static string logColor        = "green";
        private static string logWarningColor = "yellow";
        private static string logErrorColor   = "red";

        public static void Log(object msg, UnityEngine.Object context = null, int stackFrame = 2)
        {
            msg = string.Format(myFormat, logColor, msg);
#if UNITY_WEBGL
            UnityEngine.Debug.Log(msg, context);
#else
            UnityEngine.Debug.Log(Format(msg, stackFrame), context);
#endif
        }

        public static void LogWarning(object msg, UnityEngine.Object context = null)
        {
            msg = string.Format(myFormat, logWarningColor, msg);
#if UNITY_WEBGL
            UnityEngine.Debug.LogWarning(msg, context);
#else
            UnityEngine.Debug.LogWarning(Format(msg), context);
#endif
        }

        public static void LogError(object msg, UnityEngine.Object context = null)
        {
            msg = string.Format(myFormat, logErrorColor, msg);
#if UNITY_WEBGL
            UnityEngine.Debug.LogError(msg, context);
#else
            UnityEngine.Debug.LogError(Format(msg), context);
#endif
        }

        public static void ShowLog(bool show, object msg, UnityEngine.Object context = null)
        {
            if (show)
                Log(msg, context, 3);
        }

#if !UNITY_WEBGL
        private static string Format(object msg, int stackFrame = 2)
        {
            try
            {
                MethodBase method = new StackTrace().GetFrame(stackFrame).GetMethod();
                string methodName = method.Name;
                string className = method.DeclaringType.Name;
                return className + "." + methodName + ": " + msg;
            }
            catch (System.Exception e)
            {
                LogError("JLiDebugger.Debugger.Format failed, known to not work in Windows Standalone architecture 'x86_64': " + e);
                return "";
            }
        }
#endif
    }
}