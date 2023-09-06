namespace sta.Models
{
    public class TodosProdutos
    {
        public long Id { get; set; }
        public string name { get; set; }

        public string desc { get; set; }

        public decimal price { get; set; } // Propriedade para o preço

        public decimal rrp { get; set; } // Propriedade para o preço

        public int quantity { get; set; } // Propriedade para a quantidade
        public string img { get; set; } // Propriedade para a imagem

        public string type { get; set; }
    }
}
