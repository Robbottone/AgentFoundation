namespace DotNetAIAgent.Services
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
