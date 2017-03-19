using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xray.Common
{
    public static class IAsyncEnumerableExtensions
    {
        public static async Task<TKey> MinAsync<TKey, TValue>(this IAsyncEnumerable<KeyValuePair<TKey, TValue>> instance, CancellationToken token)
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            IAsyncEnumerator<KeyValuePair<TKey, TValue>> enumerator = instance.GetAsyncEnumerator();
            Comparer<TKey> comparer = Comparer<TKey>.Default;
            TKey value = default(TKey);

            if (value == null)
            {
                while (await enumerator.MoveNextAsync(token))
                {
                    TKey current = enumerator.Current.Key;

                    if (current != null && (value == null || comparer.Compare(current, value) < 0))
                    {
                        value = current;
                    }
                }

                return value;
            }

            bool hasValue = false;

            while (await enumerator.MoveNextAsync(token))
            {
                TKey current = enumerator.Current.Key;

                if (hasValue)
                {
                    if (comparer.Compare(current, value) < 0)
                    {
                        value = current;
                    }
                }
                else
                {
                    value = current;
                    hasValue = true;
                }
            }

            if (hasValue)
            {
                return value;
            }

            throw new ArgumentException("No elements in sequence.");
        }
    }
}
