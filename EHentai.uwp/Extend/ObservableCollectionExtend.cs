using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EHentai.uwp.Extend
{
    public static class ObservableCollectionExtend
    {
        public static void AddRange<T>(this ObservableCollection<T> observable, ObservableCollection<T> addCollection)
        {
            try
            {
                if (addCollection != null && addCollection.Any())
                {
                    foreach (T item in addCollection)
                        observable.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
