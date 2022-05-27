using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Universe
{
    public class UniverseManager : MonoBehaviour
    {
        #region Public
        
        public static HashSet<FactBase> Facts = new HashSet<FactBase>();
        public static HashSet<SignalBase> Signals = new HashSet<SignalBase>();
        
        #endregion
        
        
        #region Unity API

        private void OnGUI()
        {
            var factsDebug = $"Count => {Facts.Count} \n";
            
            for (var i = 0; i < Facts.Count; i++)
            {
                factsDebug +=  $" {Facts.ElementAt(i).name} => {Facts.ElementAt(i).ToString()} \n";
            }
            
            GUILayout.Button($"Facts: {factsDebug}" );
        }
        
        #endregion
    }
}