using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicHelloWorld.Model
{
    public class Product
    {
        public string ProductName           { get; set;  }
        public int    Storage               { get; set; }
        public string Description           { get; set; }
        public decimal Price                { get; set; }
        public bool Available               { get; set; }
        public int? WarrantyMonths          { get; set; }
        public ProductCondition? ProductCondition { get; set; }
    }

    public enum ProductCondition { Used, New, Refurbished }
}
