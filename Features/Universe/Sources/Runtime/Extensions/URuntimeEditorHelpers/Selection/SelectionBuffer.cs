using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Universe
{
	public class SelectionBuffer : UBehaviour
	{
	    #region Exposed

	    public List<GameObject> Selected => _selected ??= new();

	    #endregion
	    
	    
	    #region Unity API

	    public void Start()
	    {
#if UNITY_EDITOR
		    var selections = Selection.instanceIDs;
		    var newSelections = MergeSelected(selections);
		    
		    Selection.instanceIDs = newSelections;
		    Destroy(gameObject);
#endif
	    }

	    public void Add(GameObject go)
	    {
		    if (Selected.Contains(go)) return;
		    
		    Selected.Add(go);
	    }

	    #endregion
	    
	    
	    #region Utils

	    private int[] MergeSelected(int[] to)
	    {
		    var initialAmount = to.Length;
		    var selectedAmount = Selected.Count;
		    var mergedAmount = initialAmount + selectedAmount;
		    var result = new int[mergedAmount];
		    
		    for (var i = 0; i < initialAmount; i++) result[i] = to[i];

		    for (var i = 0; i < selectedAmount; i++)
		    {
			    var selected = Selected[i];
			    var instanceID = selected.GetInstanceID();
			    var resultIndex = i + initialAmount;

			    result[resultIndex] = instanceID;
		    }

		    return result;
	    }

	    #endregion
	    
	    
	    #region Private

	    [SerializeField]
	    private List<GameObject> _selected;

	    #endregion
	}
}