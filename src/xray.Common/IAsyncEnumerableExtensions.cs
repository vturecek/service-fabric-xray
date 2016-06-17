// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data.Collections;
    using Fabric = Microsoft.ServiceFabric.Data;

    public static class AsyncEnumerableEx
    {
        #region Creation

        private static IAsyncEnumerable<T> Create<T>(Func<IAsyncEnumerator<T>> getEnumerator)
        {
            return new AnonymousAsyncEnumerable<T>(getEnumerator);
        }

        private class AnonymousAsyncEnumerable<T> : IAsyncEnumerable<T>
        {
            private readonly Func<IAsyncEnumerator<T>> _getEnumerator;

            public AnonymousAsyncEnumerable(Func<IAsyncEnumerator<T>> getEnumerator)
            {
                _getEnumerator = getEnumerator;
            }

            public IAsyncEnumerator<T> GetEnumerator()
            {
                return _getEnumerator();
            }
        }

        private static IAsyncEnumerator<T> Create<T>(Func<CancellationToken, Task<bool>> moveNext, Func<T> current,
            Action dispose, IDisposable enumerator)
        {
            return Create(async ct =>
            {
                using (ct.Register(dispose))
                {
                    try
                    {
                        var result = await moveNext(ct).ConfigureAwait(false);
                        if (!result)
                        {
                            enumerator?.Dispose();
                        }
                        return result;
                    }
                    catch
                    {
                        enumerator?.Dispose();
                        throw;
                    }
                }
            }, current, dispose);
        }

        private static IAsyncEnumerator<T> Create<T>(Func<CancellationToken, Task<bool>> moveNext, Func<T> current, Action dispose)
        {
            return new AnonymousAsyncEnumerator<T>(moveNext, current, dispose);
        }

        private class AnonymousAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly Func<CancellationToken, Task<bool>> _moveNext;
            private readonly Func<T> _current;
            private readonly Action _dispose;
            private bool _disposed;

            public AnonymousAsyncEnumerator(Func<CancellationToken, Task<bool>> moveNext, Func<T> current, Action dispose)
            {
                _moveNext = moveNext;
                _current = current;
                _dispose = dispose;
            }

            public Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                if (_disposed)
                    return SpecialTasks.False;

                return _moveNext(cancellationToken);
            }

            public T Current => _current();

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _dispose();
                }
            }
        }

        #endregion

        #region Async Operators

        public static IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Create(() =>
            {
                var e = source.GetEnumerator();
                var current = default(TResult);

                var cts = new CancellationTokenDisposable();
                var d = Disposable.Create(cts, e);

                return Create(async ct =>
                {
                    if (await e.MoveNext(cts.Token).ConfigureAwait(false))
                    {
                        current = await selector(e.Current).ConfigureAwait(false);
                        return true;
                    }
                    return false;
                },
                    () => current,
                    d.Dispose,
                    e
                );
            });
        }

        public static IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, Task<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Create(() =>
            {
                var e = source.GetEnumerator();
                var current = default(TResult);
                var index = 0;

                var cts = new CancellationTokenDisposable();
                var d = Disposable.Create(cts, e);

                return Create(async ct =>
                {
                    if (await e.MoveNext(cts.Token).ConfigureAwait(false))
                    {
                        current = await selector(e.Current, checked(index++)).ConfigureAwait(false);
                        return true;
                    }
                    return false;
                },
                    () => current,
                    d.Dispose,
                    e
                );
            });
        }

        public static IAsyncEnumerable<TSource> WhereAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return Create(() =>
            {
                var e = source.GetEnumerator();
                var current = default(TSource);
                var cts = new CancellationTokenDisposable();
                var d = Disposable.Create(cts, e);

                return Create(async ct =>
                {
                    var b = false;
                    bool moveNext;
                    do
                    {
                        moveNext = await e.MoveNext(cts.Token).ConfigureAwait(false);
                        if (moveNext)
                        {
                            b = await predicate(e.Current).ConfigureAwait(false);
                        }
                    } while (!b && moveNext);
                    if (b)
                    {
                        current = e.Current;
                        return true;
                    }
                    return false;
                },
                    () => current,
                    d.Dispose,
                    e
                    );
            });
        }

        public static IAsyncEnumerable<TSource> WhereAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, Task<bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return Create(() =>
            {
                var e = source.GetEnumerator();
                var index = 0;
                var current = default(TSource);
                var cts = new CancellationTokenDisposable();
                var d = Disposable.Create(cts, e);

                return Create(async ct =>
                {
                    var b = false;
                    bool moveNext;
                    do
                    {
                        moveNext = await e.MoveNext(cts.Token).ConfigureAwait(false);
                        if (moveNext)
                        {
                            b = await predicate(e.Current, checked(index++)).ConfigureAwait(false);
                        }
                    } while (!b && moveNext);
                    if (b)
                    {
                        current = e.Current;
                        return true;
                    }
                    return false;
                },
                    () => current,
                    d.Dispose,
                    e
                    );
            });
        }

        public static IAsyncEnumerable<TResult> SelectManyAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Task<IAsyncEnumerable<TResult>>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Create(() =>
            {
                var e = source.GetEnumerator();
                var ie = default(IAsyncEnumerator<TResult>);

                var innerDisposable = new AssignableDisposable();

                var cts = new CancellationTokenDisposable();
                var d = Disposable.Create(cts, innerDisposable, e);

                // ReSharper disable once RedundantAssignment
                var inner = default(Func<CancellationToken, Task<bool>>);
                var outer = default(Func<CancellationToken, Task<bool>>);

                inner = async ct =>
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (await ie.MoveNext(ct).ConfigureAwait(false))
                    {
                        return true;
                    }
                    innerDisposable.Disposable = null;
                    // ReSharper disable once AccessToModifiedClosure
                    // ReSharper disable once PossibleNullReferenceException
                    return await outer(ct).ConfigureAwait(false);
                };

                outer = async ct =>
                {
                    if (await e.MoveNext(ct).ConfigureAwait(false))
                    {
                        var enumerable = await selector(e.Current).ConfigureAwait(false);
                        ie = enumerable.GetEnumerator();
                        innerDisposable.Disposable = ie;

                        return await inner(ct).ConfigureAwait(false);
                    }
                    return false;
                };

                return Create(ct => ie == null ? outer(ct) : inner(ct),
                    () => ie.Current,
                    d.Dispose,
                    e
                );
            });
        }

        public static IAsyncEnumerable<TResult> SelectManyAsync<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, Task<IAsyncEnumerable<TResult>>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Create(() =>
            {
                var e = source.GetEnumerator();
                var ie = default(IAsyncEnumerator<TResult>);

                var innerDisposable = new AssignableDisposable();

                var index = 0;

                var cts = new CancellationTokenDisposable();
                var d = Disposable.Create(cts, innerDisposable, e);

                // ReSharper disable once RedundantAssignment
                var inner = default(Func<CancellationToken, Task<bool>>);
                var outer = default(Func<CancellationToken, Task<bool>>);

                inner = async ct =>
                {
                    if (await e.MoveNext(ct).ConfigureAwait(false))
                    {
                        return true;
                    }
                    innerDisposable.Disposable = null;
                    // ReSharper disable once AccessToModifiedClosure
                    // ReSharper disable once PossibleNullReferenceException
                    return await outer(ct).ConfigureAwait(false);
                };

                outer = async ct =>
                {
                    if (await e.MoveNext(ct).ConfigureAwait(false))
                    {
                        var enumerable = await selector(e.Current, checked(index++)).ConfigureAwait(false);
                        ie = enumerable.GetEnumerator();
                        innerDisposable.Disposable = ie;

                        return await inner(ct).ConfigureAwait(false);
                    }
                    return false;
                };

                return Create(ct => ie == null ? outer(ct) : inner(ct),
                    () => ie.Current,
                    d.Dispose,
                    e
                );
            });
        }

        public static async Task ForEachAsyncAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, Task> action, CancellationToken cancellationToken)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            using (var e = source.GetEnumerator())
            {
                while (await e.MoveNext(cancellationToken).ConfigureAwait(false))
                {
                    await action(e.Current).ConfigureAwait(false);
                }
            }
        }

        public static async Task ForEachAsyncAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, Task> action, CancellationToken cancellationToken)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var i = 0;

            using (var e = source.GetEnumerator())
            {
                while (await e.MoveNext(cancellationToken).ConfigureAwait(false))
                {
                    await action(e.Current, checked(i++)).ConfigureAwait(false);
                }
            }
        }

        #endregion

        #region Wrappers

        public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(this Fabric.IAsyncEnumerable<T> enumerable)
        {
            return new AsyncEnumerableFabricWrapper<T>(enumerable);
        }

        private sealed class AsyncEnumerableFabricWrapper<T> : IAsyncEnumerable<T>
        {
            private readonly Fabric.IAsyncEnumerable<T> _inner;

            public AsyncEnumerableFabricWrapper(Fabric.IAsyncEnumerable<T> inner)
            {
                _inner = inner;
            }

            public IAsyncEnumerator<T> GetEnumerator()
            {
                return new AsyncEnumeratorFabricWrapper(_inner.GetAsyncEnumerator());
            }

            private class AsyncEnumeratorFabricWrapper : IAsyncEnumerator<T>
            {
                private readonly Fabric.IAsyncEnumerator<T> _inner;

                public AsyncEnumeratorFabricWrapper(Fabric.IAsyncEnumerator<T> inner)
                {
                    _inner = inner;
                }

                public void Dispose() => _inner.Dispose();

                public Task<bool> MoveNext(CancellationToken cancellationToken)
                    => _inner.MoveNextAsync(cancellationToken);

                public T Current => _inner.Current;
            }
        }

        #endregion
    }

    public static class ReliableCollectionExtensions
    {
        public static async Task<IAsyncEnumerable<KeyValuePair<TKey, TValue>>> CreateLinqAsyncEnumerable<TKey, TValue>(
            this IReliableDictionary<TKey, TValue> dictionary, Microsoft.ServiceFabric.Data.ITransaction txn)
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var enumerable = await dictionary.CreateEnumerableAsync(txn).ConfigureAwait(false);
            return enumerable.AsAsyncEnumerable();
        }

        public static async Task<IAsyncEnumerable<KeyValuePair<TKey, TValue>>> CreateLinqAsyncEnumerable<TKey, TValue>(
            this IReliableDictionary<TKey, TValue> dictionary, Microsoft.ServiceFabric.Data.ITransaction txn, EnumerationMode enumerationMode)
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var enumerable = await dictionary.CreateEnumerableAsync(txn, enumerationMode).ConfigureAwait(false);
            return enumerable.AsAsyncEnumerable();
        }

        public static async Task<IAsyncEnumerable<KeyValuePair<TKey, TValue>>> CreateLinqAsyncEnumerable<TKey, TValue>(
            this IReliableDictionary<TKey, TValue> dictionary, Microsoft.ServiceFabric.Data.ITransaction txn, Func<TKey, bool> filter, EnumerationMode enumerationMode)
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var enumerable = await dictionary.CreateEnumerableAsync(txn, filter, enumerationMode).ConfigureAwait(false);
            return enumerable.AsAsyncEnumerable();
        }

        public static async Task<IAsyncEnumerable<T>> CreateLinqAsyncEnumerable<T>(
            this IReliableQueue<T> dictionary, Microsoft.ServiceFabric.Data.ITransaction txn)
        {
            var enumerable = await dictionary.CreateEnumerableAsync(txn).ConfigureAwait(false);
            return enumerable.AsAsyncEnumerable();
        }
    }

    internal static class SpecialTasks
    {
        public static Task<bool> False { get; } = Task.FromResult(false);
        public static Task<bool> True { get; } = Task.FromResult(true);
    }

    internal class CancellationTokenDisposable : IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public CancellationToken Token => _cts.Token;

        public void Dispose()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }
    }

    internal class BinaryDisposable : IDisposable
    {
        private IDisposable _d1;
        private IDisposable _d2;

        public BinaryDisposable(IDisposable d1, IDisposable d2)
        {
            _d1 = d1;
            _d2 = d2;
        }

        public void Dispose()
        {
            var d1 = Interlocked.Exchange(ref _d1, null);
            if (d1 != null)
            {
                d1.Dispose();

                var d2 = Interlocked.Exchange(ref _d2, null);
                d2?.Dispose();
            }
        }
    }

    internal class Disposable : IDisposable
    {
        private static readonly Action _nop = () => { };
        private Action _dispose;

        public Disposable(Action dispose)
        {
            _dispose = dispose;
        }

        public static IDisposable Create(IDisposable d1, IDisposable d2)
        {
            return new BinaryDisposable(d1, d2);
        }

        public static IDisposable Create(params IDisposable[] disposables)
        {
            return new CompositeDisposable(disposables);
        }

        public void Dispose()
        {
            var dispose = Interlocked.Exchange(ref _dispose, _nop);
            dispose();
        }
    }

    internal class CompositeDisposable : IDisposable
    {
        private static readonly IDisposable[] _empty = new IDisposable[0];
        private IDisposable[] _dispose;

        public CompositeDisposable(params IDisposable[] dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            var dispose = Interlocked.Exchange(ref _dispose, _empty);

            foreach (var d in dispose)
            {
                d.Dispose();
            }
        }
    }

    internal class AssignableDisposable : IDisposable
    {
        private readonly object _gate = new object();
        private IDisposable _disposable;
        private bool _disposed;

        public IDisposable Disposable
        {
            set
            {
                lock (_gate)
                {
                    DisposeInner();

                    _disposable = value;

                    if (_disposed)
                    {
                        DisposeInner();
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    DisposeInner();
                }
            }
        }

        private void DisposeInner()
        {
            if (_disposable != null)
            {
                _disposable.Dispose();
                _disposable = null;
            }
        }
    }

}