namespace ProniaMVC.Models
{
    public class OrdersInfo:BaseEntity
    {
        public int Id { get; set; } 
        public int OrderNo { get; set; }
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
