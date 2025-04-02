using fruits_store_backend_asp_net.Enums;

namespace fruits_store_backend_asp_net.Models
{
    public partial class Fruit
    {
        public Guid FruitId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public FruitType FruitType { get; set; }

        public string ImageBase64 { get; set; }

        public Guid AddedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Fruit()
        {
            Name ??= "";
            ImageBase64 ??= "";
        }
    }
}
