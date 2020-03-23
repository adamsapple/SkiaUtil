using System;
using System.Collections.Generic;
using System.Linq;


namespace SkiaUtil.Extension
{
    internal static class LinqExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="func"></param>
        public static void ForEach<T>(this IEnumerable<T> src, Action<T, int> func)
        {
            foreach (var item in src.Select((x, i) => new { Value = x, Index = i }))
                func(item.Value, item.Index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="func"></param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> src, Action<T> func)
        {
            try
            {
                if (src != null)
                {
                    foreach (var item in src)
                    {
                        func(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception :  " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("StackTrace : " + ex.StackTrace);
            }

            return src;
        }

        public static IEnumerable<T> For<T>(this IEnumerable<T> src, Action<T> func)
        {
            try
            {
                if (src != null)
                {
                    for (var i = src.Count(); --i >= 0;)
                    {
                        func(src.ElementAt(i));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception :  " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("StackTrace : " + ex.StackTrace);
            }

            return src;
        }

    }
}
