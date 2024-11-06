namespace Web.Support
{
    using System;

    public struct Paging
    {
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public readonly int FirstPage { get => 1; }
        public readonly int LastPage { get => Total; }
        public readonly int NoOfPages { get => (int)Math.Ceiling(Decimal.Divide(Total, PageSize)); }
        public readonly int PreviousPage { get => Math.Max(CurrentPage - 1, FirstPage); }
        public readonly int NextPage { get => Math.Min(CurrentPage + 1, LastPage); }

        public readonly int Skip { get => (CurrentPage - 1) * PageSize; }

        public static Paging Create(int count, int? currentPage, int? pageSize)
        {
            currentPage ??= 1;
            pageSize ??= 10;
            return new Paging
            {
                Total = count,
                PageSize = (int)pageSize,
                CurrentPage = (int)currentPage
            };
        }
    }
}
