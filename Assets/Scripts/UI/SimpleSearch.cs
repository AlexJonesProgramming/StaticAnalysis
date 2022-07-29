using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSearch : MonoBehaviour
{
    public Transform container;
    public void Search(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            int count = container.childCount;

            for (int i = 0; i < count; i++)
            {
                container.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            searchTerm = searchTerm.ToLower();

            int count = container.childCount;

            Transform currentChild;

            for (int i = 0; i < count; i++)
            {
                currentChild = container.GetChild(i);
                if (currentChild.name.ToLower().Contains(searchTerm))
                    currentChild.gameObject.SetActive(true);
                else
                    currentChild.gameObject.SetActive(false);
            }
        }
    }
}
