using BasicHelloWorld.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicHelloWorld.Services
{
    public class ProductServices
    {
        public decimal GetProductPrice(string productName)
        { 
            return 799m;
        }

        public bool GetProductAvailability(string productName)
        { 
            return (productName.Length % 2) == 0;
        }
    }
}
