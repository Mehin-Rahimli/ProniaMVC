namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class PaginatedVM<T>
    {
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public List<T> items { get; set; }
    }
}
