using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.ViewModels
{ 
    public class OrdersInfoVM
    {
        public int? OrderNo { get; set; }
        public string? UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public string Date {  get; set; }
        public string? Status { get; set; }
       
    }
}
