using System.Collections;
using System.Collections.Generic;

namespace WinDowDev
{
    public static class ExtArrayList
    {
        public static List<T> ArrayListToList<T>(this ArrayList arrList)
        {
            var results = new List<T>();
            foreach (var item in arrList)
            {
                if (item is T t)
                {
                    results.Add(t);
                }
            }
            return results;
        }

        public static ArrayList ListToArrayList<T>(this List<T> list)
        {
            var results = new ArrayList();
            list.ForEach(item =>
            {
                results.Add(item);
            });
            return results;
        }
    }
}
