namespace sta.Models
{
    public class Orders
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string payment_method { get; set; }
        public decimal total_price { get; set; }
        public DateTime created_at { get; set; }
        public string products { get; set; } // Novo campo products
        public int type { get; set; }

    }
}