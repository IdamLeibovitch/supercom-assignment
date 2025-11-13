// using Microsoft.EntityFrameworkCore;

// namespace TaskReminderService
// {
//     public class TaskDbContext : DbContext
//     {
//         public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
//         {
//         }

//         public DbSet<TaskReminderService.User> Users => Set<TaskReminderService.User>();
//         public DbSet<TaskReminderService.Task> Tasks => Set<TaskReminderService.Task>();

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);

//             modelBuilder.Entity<User>()
//                 .Property(u => u.UserName)
//                 .IsRequired()
//                 .HasMaxLength(20);

//             modelBuilder.Entity<User>()
//                 .HasIndex(u => u.UserName)
//                 .IsUnique();

//             modelBuilder.Entity<User>()
//                 .HasMany(u => u.Tasks)
//                 .WithOne(t => t.User)
//                 .HasForeignKey(t => t.UserId);
//         }
//     }
// }
