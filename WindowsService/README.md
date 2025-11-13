# Task Reminder Windows Service

## Overview
Monitors overdue tasks and sends reminders via RabbitMQ queue.

**Architecture:**
- **TaskPublisherWorker**: Checks DB for overdue tasks → publishes to RabbitMQ
- **TaskConsumerWorker**: Subscribes to queue → logs reminders

## Quick Start

### 1. Start RabbitMQ (If not installed or not on windows)
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

**If you get `PRECONDITION_FAILED` error:**
```bash
docker restart rabbitmq
```

### 2. Run the Service
```bash
cd WindowsService/TaskReminderService
dotnet restore
dotnet build
dotnet run
```

## Configuration

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "sqlserver": "Server=localhost,1433;Database=TasksManagerDB;..."
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672
  },
  "TaskReminder": {
    "CheckIntervalMinutes": 5
  }
}
```

## Verify It's Working

1. **Console logs:**
   - "Task Publisher Worker started"
   - "Task Consumer Worker started"
   - "RabbitMQ connection established"

2. **RabbitMQ UI:** http://localhost:15672 (guest/guest)
   - Check "Queues" tab for `task-reminders` and `task-reminders-dlq`

3. **Test:** Create task with past due date → wait 5 min → check logs

## Concurrency Features

- **Optimistic locking** with row versioning
- **Dead Letter Queue** for failed messages (3 retries max)
- **Idempotency** tracking to prevent duplicate processing
- **Transactions** with automatic rollback
- **Prefetch limit** of 10 messages for controlled concurrency

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Port conflict | `docker ps` then `docker stop rabbitmq` |
| Queue error | `docker restart rabbitmq` |
| No messages | Check tasks have `DueDate < NOW` and `IsReminderSent = false` |
| Connection failed | Verify SQL Server and RabbitMQ are running |

## Production Deployment (Windows)

```bash
dotnet publish -c Release -o ./publish
sc create TaskReminderService binPath="C:\path\to\publish\TaskReminderService.exe"
sc start TaskReminderService
```
