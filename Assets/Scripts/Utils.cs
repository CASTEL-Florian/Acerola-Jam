using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Utils
{
    public static T[] GetAllObjectsOnlyInScene<T>() where T : Object
    {
        List<T> objectsInScene = new List<T>();

        foreach (T o in Resources.FindObjectsOfTypeAll<T>())
        {
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(o.GameObject().transform.root.gameObject))
            {
                continue;
            }
#endif
            
            if (!(o.GameObject().hideFlags == HideFlags.NotEditable || o.GameObject().hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(o);

        }

        return objectsInScene.ToArray();
    }
}
