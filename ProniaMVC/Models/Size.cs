namespace ProniaMVC.Models
{
    public class Size:BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //relational
        public List<ProductSize>ProductSizes { get; set; }
    }
}
