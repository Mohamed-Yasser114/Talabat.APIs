namespace Talabat.APIs.Helpers
{
    public class Pagination<T>
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }

        public Pagination(int pageSize, int pageIndex, int count, IEnumerable<T> data) { 
        
            PageSize = pageSize;
            PageIndex = pageIndex;
            Count = count;
            Data = data;
        }
    }
}
