using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Exceptions;

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

        public async Task PublishAsync<T>(T @event, string routingKey)
        {
            try
            {
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

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}