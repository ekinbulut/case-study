namespace Common.Messaging
{
    public static class RabbitMqConstants
    {
        // Default exchange name used for routing messages
        public const string ExchangeName = "main-exchange";

        // Queue Names
        public const string OrderQueue = "order-queue";
        public const string OrderConfirmedQueue = "order-queue-confirmed";

        public const string StockQueue = "stock-queue";
        public const string StockQueueUpdated = "stock-queue-updated";

        public const string NotificationQueue = "notification-queue";
        public const string NotificationSentQueue = "notification-sent";

        
        // Routing Keys
        public const string OrderCreatedRoutingKey = "order.created";
        public const string OrderCancelledRoutingKey = "order.cancelled";
        public const string OrderConfirmedRoutingKey = "order.confirmed";

        public const string StockUpdatedRoutingKey = "stock.updated";
        public const string StockUpdateRoutingKey = "stock.update";
        public const string NotificationRoutingKey = "notification.created";
        public const string NotificationSentRoutingKey = "notification.sent";

        // Dead Letter Configuration
        public const string DeadLetterExchange = "dead-letter-exchange";
        public const string DeadLetterRoutingKey = "dead-letter";
    }
}