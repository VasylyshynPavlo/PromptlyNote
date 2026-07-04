using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Entities;

namespace PromptlyNote.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ToDoTask> Tasks => Set<ToDoTask>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<TaskList> TaskLists => Set<TaskList>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.ColorHex)
                    .HasMaxLength(9);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TaskList
            modelBuilder.Entity<TaskList>(entity =>
            {
                entity.HasKey(tl => tl.Id);

                entity.Property(tl => tl.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(tl => tl.User)
                    .WithMany(u => u.TaskLists)
                    .HasForeignKey(tl => tl.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ToDoTask
            modelBuilder.Entity<ToDoTask>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(t => t.Note)
                        .HasMaxLength(1000);

                entity.HasOne(t => t.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Category)
                    .WithMany(c => c.Tasks)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.TaskList)
                    .WithMany(tl => tl.Tasks)
                    .HasForeignKey(t => t.TaskListId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.OwnsMany(t => t.SubTasks, sb =>
                {
                    sb.ToTable("SubTasks");
                    sb.WithOwner().HasForeignKey("ToDoTaskId");
                    sb.HasKey(s => s.Id);

                    sb.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                });
            });
        }
    }
}
