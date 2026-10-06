using DotNetAIAgent.Attribute;
using DotNetAIAgent.Model.Interfaces;
using DotNetAIAgent.Services;
using System.ComponentModel;

namespace DotNetAIAgent.Tools
{
    public class ProductTools: IAIToolProvider
    {
        private readonly ProductServices _productService;

        public ProductTools(ProductServices productService)
        {
            _productService = productService;
        }

        [AITool]
        [Description("Recupera il prezzo di un prodotto dato il suo nome. Usa questa funzione quando l'utente chiede il prezzo di un prodotto.")]
        public decimal GetProductPrice(string productName) => _productService.GetProductPrice(productName);

        [AITool]
        [Description("Recupera la disponibilita' del prodotto dato il suo nome. Usa questa funzione quando l'utente chiede la disponibilita' di un prodotto.")]
        public bool GetProductAvailability(string productName) => _productService.GetProductAvailability(productName);
        
    }
}
