using UnityEngine;
using System;
using Universe;

[CreateAssetMenu(menuName = "FUCKING DEBUG")]
public class ScriptableDebugSizeOfTaskData : ScriptableObject
{
    public SizeOfEverything m_sizeOfEverything;
	
    [Serializable]
    public class SizeOfEverything
    {
        public int SizeOfUpdate;
        public int SizeOfLateUpdate;
        public int SizeOfFixedUpdate;

        public void UpdateSizeOfUpdate(int candidate)
        {

            if(SizeOfUpdate < candidate)
                SizeOfUpdate = candidate;
        }

        public void UpdateOfLateUpdate(int candidate)
        {

            if(SizeOfLateUpdate < candidate)
                SizeOfLateUpdate = candidate;
        }

        public void UpdateOfFixedUpdate(int candidate)
        {
			
            if(SizeOfFixedUpdate < candidate)
                SizeOfFixedUpdate = candidate;
        }
    }
}