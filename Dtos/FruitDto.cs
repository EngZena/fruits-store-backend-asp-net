using fruits_store_backend_asp_net.Enums;

namespace fruits_store_backend_asp_net.Dtos
{
    public partial class FruitDto
    {
        /// <summary>
        /// The name of the fruit
        /// </summary>
        /// <example>Apple</example>
        public required string Name { get; set; }

        /// <summary>
        /// The price of the fruit
        /// </summary>
        /// <example>2</example>
        public decimal Price { get; set; }

        /// <summary>
        /// The type of the fruit
        /// </summary>
        /// <example>0</example>
        public FruitType FruitType { get; set; }

        /// <summary>
        /// The image of the fruit
        /// </summary>
        public required string ImageBase64 { get; set; }
    }
}
