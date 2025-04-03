using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Exceptions;
using RabbitMQ.Client.Events;

namespace Common.Messaging
{
    public class EventBus : IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        private EventBus(IConnection connection, IChannel channel)
        {
            _connection = connection;
            _channel = channel;
        }

        public static async Task<EventBus> CreateAsync(string hostname, string username, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostname,
                UserName = username,
                Password = password,
                
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            return new EventBus(connection, channel);
        }
        
        public async Task DeclareExchangeAsync()
        {
            // Declare the main exchange asynchronously.
            await _channel.ExchangeDeclareAsync(
                exchange: RabbitMqConstants.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Declare the dead letter exchange asynchronously.
            await _channel.ExchangeDeclareAsync(
                exchange: RabbitMqConstants.DeadLetterExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);
        }

        public async Task PublishAsync<T>(T @event, string routingKey, string queueName)
        {
            try
            {
                // Optionally, declare a queue with a name based on the routing key.
                // You can also use a predefined queue name if you prefer.
                await _channel.QueueDeclareAsync(
                    queue: queueName,           // Queue name
                    durable: true,               // Queue durability
                    exclusive: false,            // Not exclusive
                    autoDelete: false,           // Do not auto-delete
                    arguments: null);
                
                // Bind the declared queue to the exchange using the routing key.
                await _channel.QueueBindAsync(
                    queue: queueName,
                    exchange: RabbitMqConstants.ExchangeName,
                    routingKey: routingKey);
                
                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                // Publish the message asynchronously.
                await _channel.BasicPublishAsync(
                    exchange: RabbitMqConstants.ExchangeName,
                    routingKey: routingKey,
                    body: body);
            }
            catch (MessaagingException e)
            {
                throw new MessaagingException("Failed to publish message to RabbitMQ.", e);
            }
        }

        public async Task SubscribeAsync<T>(string queueName, string routingKey, Func<T, Task> onMessage)
        {
            // Declare the queue and bind it to the exchange with the provided routing key.
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: RabbitMqConstants.ExchangeName,
                routingKey: routingKey);

            // Create an asynchronous consumer.
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));

                // Process the received message.
                await onMessage(message);

                // Acknowledge the message.
                //await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Start consuming the queue.
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }
        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}