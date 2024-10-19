using FruitsStoreBackendASPNET.Enums;

namespace FruitsStoreBackendASPNET.Dtos
{
    public partial class FruitDto
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }

        public FruitType FruitType { get; set; }

        public required string ImageBase64 { get; set; }

        public Guid AddedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
