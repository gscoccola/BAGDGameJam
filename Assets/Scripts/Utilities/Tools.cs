using Random = System.Random;

public static class Tools
{
    
}

public static class ArrayExtensions
{
    public static void Shuffle<T>(this T[] items) 
    {
        if (items.Length == 2)
        {
            if (UnityEngine.Random.value >= 0.5f)
            {
                T first= items[0];
                T second = items[1];
                items[0] = second;
                items[1] = first;
            }
        }
        else
        {
            Random rnd = new Random();
            for (var i = 0; i < items.Length; i++)
            {
                int j = rnd.Next(i, items.Length);
                (items[i], items[j]) = (items[j], items[i]);
            }
        }
    }
}