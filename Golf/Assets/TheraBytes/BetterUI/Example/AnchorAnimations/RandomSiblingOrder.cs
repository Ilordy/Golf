using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSiblingOrder : MonoBehaviour
{
    [SerializeField] Transform parent;

    // called from OnClick Event
    public void RandomizeSiblingOrder()
    {
        if(parent == null)
        {
            Debug.LogWarning("parent was not specified. Using self.");
            parent = this.transform;
        }

        // All children will get a random sibling index.
        // This will trigger the layout group to resort the positions based on the sibling index.
        // It doesn't matter if the indices have gaps in between.
        foreach (Transform child in parent)
        {
            int randomValue = UnityEngine.Random.Range(0, 1000);
            child.SetSiblingIndex(randomValue);
        }
    }

}
