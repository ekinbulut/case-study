using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Messaging
{
    public class EventBus : IDisposable
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
        
        public async Task DeclareExchange()
        {
            // Declare the main exchange
            await _channel.ExchangeDeclareAsync(
                exchange: RabbitMqConstants.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Optionally, declare a dead letter exchange for handling failed messages
            await _channel.ExchangeDeclareAsync(
                exchange: RabbitMqConstants.DeadLetterExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);
        }
        

        public async Task Publish<T>(T @event, string routingKey)
        {
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);


            await _channel.BasicPublishAsync(
                exchange: RabbitMqConstants.ExchangeName,
                routingKey: routingKey,
                body: body);
        }

        public async void Dispose()
        {
            await _channel?.CloseAsync();
            await _connection?.CloseAsync();
        }
    }
}