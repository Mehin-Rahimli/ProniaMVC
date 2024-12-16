namespace ProniaMVC.ViewModels
{
    public class OrderVM
    {
        public String Address { get; set; }
        public List<BasketInOrderVM>? BasketInOrderVMs { get; set; }
    }
}
