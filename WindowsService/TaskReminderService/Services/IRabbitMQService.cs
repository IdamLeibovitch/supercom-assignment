namespace TaskReminderService.Services
{
    public interface IRabbitMQService
    {
        void PublishMessage(string message);
        void StartConsuming(Action<string> onMessageReceived);
        void Dispose();
    }
}
