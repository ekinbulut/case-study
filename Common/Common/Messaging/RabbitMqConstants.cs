namespace Common.Messaging
{
    public static class RabbitMqConstants
    {
        // Default exchange name used for routing messages
        public const string ExchangeName = "main-exchange";

        // Queue Names
        public const string OrderQueue = "order-queue";
        public const string StockQueue = "stock-queue";
        public const string NotificationQueue = "notification-queue";

        // Routing Keys
        public const string OrderCreatedRoutingKey = "order.created";
        public const string OrderCancelledRoutingKey = "order.cancelled";
        public const string StockUpdatedRoutingKey = "stock.updated";
        public const string NotificationRoutingKey = "notification.*";

        // Dead Letter Configuration
        public const string DeadLetterExchange = "dead-letter-exchange";
        public const string DeadLetterRoutingKey = "dead-letter";
    }
}