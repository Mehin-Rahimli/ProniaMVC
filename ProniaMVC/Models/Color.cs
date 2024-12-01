namespace ProniaMVC.Models
{
    public class Color:BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //relational
        public List<ProductColor>ProductColors { get; set; }
    }
}
