namespace CodeDesignPlus.Net.Core.Abstractions.Models.Pager
{
    /// <summary>
    /// Pagination class for paginated data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="data">The data items.</param>
    /// <param name="totalCount">The total count of items.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="skip">The number of items to skip.</param>
    public class Pagination<T>(IEnumerable<T> data, long totalCount, int limit, int skip)
    {
        /// <summary>
        /// Gets the data items.
        /// </summary>
        public IEnumerable<T> Data { get; } = data;
        /// <summary>
        /// Gets the total count of items.
        /// </summary>
        public long TotalCount { get; } = totalCount;
        /// <summary>
        /// Gets the maximum number of items to return.
        /// </summary>
        public int Limit { get; } = limit;
        /// <summary>
        /// Gets the number of items to skip.
        /// </summary>
        public int Skip { get; } = skip;

        /// <summary>
        /// Creates a new instance of the Pagination class.
        /// </summary>
        /// <param name="items">The data items.</param>
        /// <param name="totalCount">The total count of items.</param>
        /// <param name="limit">The maximum number of items to return. If is null, defaults to 10.</param>
        /// <param name="skip">The number of items to skip. If is null, defaults to 0.</param>
        /// <returns>A new instance of the Pagination class.</returns>
        public static Pagination<T> Create(IEnumerable<T> items, long totalCount, int? limit, int? skip)
        {
            return new Pagination<T>(items, totalCount, limit ?? 10, skip ?? 0);
        }
    }
}