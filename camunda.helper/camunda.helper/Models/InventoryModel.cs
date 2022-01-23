namespace camunda.helper.Models
{
    public class InventoryModel
    {
        public string orderId { get; set; }
        public List<OrderData> products { get; set; }
        public string action { get; set; }
    }
}
