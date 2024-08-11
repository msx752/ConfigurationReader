using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigurationLib.Tests.Models
{
    public class MockAsyncCursor<T> : IAsyncCursor<T>
    {
        private readonly IEnumerable<T> _items;
        private bool called = false;

        public MockAsyncCursor(IEnumerable<T> items = null)
        {
            _items = items ?? Enumerable.Empty<T>();
        }

        public IEnumerable<T> Current => _items;

        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            return !called && (called = true);
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MoveNext(cancellationToken));
        }

        public void Dispose()
        {
        }
    }
}