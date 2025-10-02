using System;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using System.Globalization;

public static class Extensions
{
    public static float ParseFloat(this string str)
    {
        str = str.Replace(",", ".");
        return float.Parse(str, CultureInfo.InvariantCulture);
    }

    public static void ToLocalScale(this Transform transform, Vector2 scale)
    {
        var parent = transform.parent;
        transform.parent = null;
        transform.localScale = scale;
        transform.parent = parent;
    }

    public static void ForEach<T>(this T[] ts, Action<T> action)
    {
        foreach (var t in ts)
            action.Invoke(t);
    }

    public static string GetPathInHierarchy(this Transform transform)
    {
        StringBuilder path = new StringBuilder();
        Transform current = transform;
        while (current != null)
        {
            path.Insert(0, current.name);
            if (current.parent != null)
                path.Insert(0, "/");

            current = current.parent;
        }

        return path.ToString();
    }

    public static TimeSpan GetHourToAdd(DateTime now, int toHour)
    {
        int hour = now.Hour >= toHour
            ? 24 - now.Hour + toHour
            : toHour - now.Hour;

        return new TimeSpan(hour, 0, 0) - new TimeSpan(0, now.Minute, now.Second);
    }

    public static void CompleteIfActive(this Tween tween, bool withCallbacks = false)
    {
        if (tween != null && tween.IsActive())
            tween.Complete(withCallbacks);
    }

    public static T Element<T>(this System.Random rand, T[] elements, float[] chances)
    {
        if (elements.Length != chances.Length)
            throw new InvalidOperationException("Arrays must be the same size");

        float totalWeight = 0;
        foreach (var weight in chances)
        {
            totalWeight += weight;
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < chances.Length; i++)
        {
            if (randomValue < chances[i])
                return elements[i];
            randomValue -= chances[i];
        }

        return elements.FirstOrDefault();
    }

    public static T[] Shuffled<T>(this T[] array)
    {
        if (array == null)
            throw new NullReferenceException();

        System.Random random = new System.Random();
        T[] shuffled = array.OrderBy(x => random.Next()).ToArray();

        return shuffled;
    }

    public static Color Fade(this Color color, float fade = 0f) => new Color(color.r, color.g, color.b, fade);
}

