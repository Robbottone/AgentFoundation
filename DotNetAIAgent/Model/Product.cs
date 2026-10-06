namespace DotNetAIAgent.Model
{
    public class Product
    {
        public string ProductName           { get; set;  } = string.Empty;
        public int    Storage               { get; set; }
        public string Description           { get; set; } = string.Empty;
        public decimal Price                { get; set; }
        public bool Available               { get; set; }
        public int? WarrantyMonths          { get; set; }
        public ProductCondition? ProductCondition { get; set; }
    }

    public enum ProductCondition { Used, New, Refurbished }
}
