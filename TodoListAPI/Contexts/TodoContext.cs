using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListAPI.Entities;

namespace TodoListAPI.Contexts
{
    public class TodoContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public TodoContext(DbContextOptions<TodoContext> options): base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .HasMany(u => u.TodoList)
                .WithOne(td => td.User);

            modelBuilder.Entity<TodoItem>().HasKey(td => td.TodoId);
            modelBuilder.Entity<TodoItem>().Property(td => td.TodoId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TodoItem>()
                .HasOne(td => td.User)
                .WithMany(u => u.TodoList)
                .HasForeignKey(td => td.UserId);
        }
    }
}
