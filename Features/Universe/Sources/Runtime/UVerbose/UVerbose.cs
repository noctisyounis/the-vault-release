using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Universe
{
    public static class UVerbose
    {
        #region Public API
        
        [Conditional("DEBUG")]
        public static void ULog(this UBehaviour source, string message)
        {
            if (!source.IsMasterDebug && !source.IsVerbose) return;
            
            var textToShow =  $"[<color=orange>GameObject: {source.name} </color>] " +
                                    $"{message} \n" +
                                    $"[<color=magenta>GameTime: {Time.time:0.00} </color>] " +
                                    $"[<color=red>Thread: {Thread.CurrentThread.ManagedThreadId} </color>]";
            
            Log(textToShow);
        }

        [Conditional("DEBUG")]
        public static void ULog<T>(this UBehaviour source, string message, T val)
        {
            if (!source.IsMasterDebug && !source.IsVerbose) return;
            
            var textToShow = message + val;
            
            ULog( source, textToShow );
        }

        [Conditional("DEBUG")]
        public static void ULog<T>(this UBehaviour source, string message, List<T> values)
        {
            if (!source.IsMasterDebug && !source.IsVerbose) return;
            
            var textToShow = "";
               
            for ( var i = 0; i < values.Count; i++ )
            {
                textToShow += values[i] + "\n";
            }
            
            ULog( source, message + textToShow );
        }
    
        [Conditional("DEBUG")]
        public static void ULog<T>(this UBehaviour source, string message, IEnumerable<T> values )
        {
            if (!source.IsMasterDebug && !source.IsVerbose) return;
            
            var textToShow = "";
            foreach ( var t in values )
            {
                textToShow += t + " \n";
            }
            
            ULog(source,  message + textToShow );
        }
    
        [Conditional("DEBUG")]
        public static void ULog<T>(this UBehaviour source, string message, Dictionary<T, T> values )
        {
            if (!source.IsMasterDebug && !source.IsVerbose) return;
            
            var textToShow = "";
            foreach ( var pair in values )
            {
                textToShow += pair.Key + ", " + pair.Value + "\n";
            }
            
            ULog(source, message + textToShow );
        }

        [Conditional("DEBUG")]
        public static void UWarning(this UBehaviour source, string message)
        {
            if (!source.IsMasterDebug && !source.IsVerbose) return;
            
            Warning(message);
        }

        [Conditional("DEBUG")]
        public static void UError(this UBehaviour source, string message)
        {
            if (!source.IsMasterDebug && !source.IsVerbose) return;
            
            Error(message);
        }

        #endregion
        
        
        #region Constructor & Wrapper
        
        static UVerbose()
        {
        
        }

        [Conditional("DEBUG")]
        private static void Log(string textToShow)
        {
            Debug.Log( textToShow );
        }

        [Conditional("DEBUG")]
        private static void Warning(string textToShow)
        {
            Debug.LogWarning(textToShow);
        }

        [Conditional("DEBUG")]
        private static void Error(string textToShow)
        {
            Debug.LogError(textToShow);
        }
        
        #endregion
    }
}
