using System;
using UnityEngine;

namespace Universe.SceneTask.Runtime
{
	[Flags]
    public enum Environment
	{
		INVALID = -1,
		NONE = 0,
		[InspectorName("Block mesh")]
		BLOCK_MESH = 1,
		[InspectorName("Art")]
		ART = 2,
		[InspectorName("Both")]
		BOTH = 3
	}
}