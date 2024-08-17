using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionsMethods
{
    public static void SwapLayer(this GameObject obj, string layerName)
    {
        SwapLayer(obj, layerName, true);
    }

    public static void SwapLayer(this GameObject obj, string layerName, bool children)
    {
        obj.layer = LayerMask.NameToLayer(layerName);

        if (children)
        {
            foreach (Transform t in obj.transform)
            {
                SwapLayer(t.gameObject, layerName, children);
            }
        }
    }

    /// <summary>
    /// Destroy all children's GameObject of this transform
    /// </summary>
    /// <param name="transform"></param>
    public static void Clear(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Destroy all children's GameObject of this transform
    /// </summary>
    /// <param name="transform"></param>
    public static void ClearImmediate(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    /// <summary>
    /// Use this to retrieve all children with specified component
    /// </summary>
    public static List<T> GetDescendantsWithComponent<T>(this Transform root) where T : Component
    {
        List<T> components = new List<T>();

        foreach (Transform child in root)
        {
            components.AddRange(GetDescendantsWithComponent<T>(child));
        }

        T component = root.GetComponent<T>();
        if (component != null)
        {
            components.Add(component);
        }

        return components;
    }

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T GetRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}