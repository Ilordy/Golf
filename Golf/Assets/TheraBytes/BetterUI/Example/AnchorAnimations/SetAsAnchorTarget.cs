using System;
using System.Collections;
using System.Collections.Generic;
using TheraBytes.BetterUi;
using UnityEngine;

public class SetAsAnchorTarget : MonoBehaviour
{
    [SerializeField] AnchorOverride affectedObject;

    // called from OnClick Event
    public void SetTarget(RectTransform target)
    {
        Debug.Assert(affectedObject != null,
            "Affected Object is not referenced in " + this.name);

        // We assume that there is at least one anchor reference ...
        Debug.Assert(affectedObject.CurrentAnchors.Elements.Count > 0, 
            "There is no element in the list of anchor references in " + affectedObject.name);

        // ... and we want to change the reference of the first element
        // (there could be more elements but not in this example).
        affectedObject.CurrentAnchors.Elements[0].Reference = target;
    }

}
