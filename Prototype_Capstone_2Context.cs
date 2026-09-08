using Microsoft.EntityFrameworkCore;
using Prototype_Capstone_2.Models;

public class Prototype_Capstone_2Context(DbContextOptions<Prototype_Capstone_2Context> options) : DbContext(options)
{
    public DbSet<Prototype_Capstone_2.Models.Equipment> Equipment { get; set; } = default!;
    public DbSet<Prototype_Capstone_2.Models.EquipmentItem> EquipmentItem { get; set; } = default!;
    public DbSet<Prototype_Capstone_2.Models.Borrowing> Borrowing { get; set; } = default!;
    public DbSet<Prototype_Capstone_2.Models.BorrowingRequests> BorrowingRequests { get; set; } = default!;
    public DbSet<Prototype_Capstone_2.Models.BorrowingEquipmentItem> BorrowingEquipmentItem { get; set; } = default!;
    public DbSet<Prototype_Capstone_2.Models.Ticketing> Ticketing { get; set; } = default!;
    public DbSet<Prototype_Capstone_2.Models.Users> Users { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed default admin user
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Users>();
        var adminUser = new Users
        {
            UserId = 1,
            Username = "admin",
            Email = "admin@envicomm.com",
            FirstName = "Admin",
            LastName = "User",
            UserRole = "Admin",
            UserDivision = "Administration",
            PhoneNumber = "1234567890",
            ApprovalStatus = "Approved",
            UserCreated = DateTime.UtcNow
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

        modelBuilder.Entity<Users>().HasData(adminUser);

        modelBuilder.Entity<Users>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Users>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configure DateTime columns to use UTC
        modelBuilder.Entity<Users>()
            .Property(u => u.UserCreated)
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<BorrowingRequests>()
            .Property(b => b.BorrowDate)
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<BorrowingRequests>()
            .Property(b => b.ReturnDate)
            .HasConversion(v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

        modelBuilder.Entity<Borrowing>()
            .Property(b => b.BorrowDate)
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Borrowing>()
            .Property(b => b.ReturnDate)
            .HasConversion(v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

        modelBuilder.Entity<Equipment>()
            .Property(e => e.EquipmentDateCreated)
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<BorrowingEquipmentItem>()
            .Property(b => b.ReturnDate)
            .HasConversion(v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

        modelBuilder.Entity<Ticketing>()
            .Property(t => t.TicketDateCreated)
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Ticketing>()
            .Property(t => t.TicketCreated)
            .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Ticketing>()
            .Property(t => t.TicketTimeFixed)
            .HasConversion(v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

        modelBuilder.Entity<Borrowing>()
            .HasOne(b => b.BorrowingRequest)
            .WithMany(r => r.Borrowings)
            .HasForeignKey(b => b.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BorrowingEquipmentItem>()
            .HasOne(bei => bei.Borrowing)
            .WithMany(b => b.AssignedItems)
            .HasForeignKey(bei => bei.BorrowingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BorrowingEquipmentItem>()
            .HasOne(bei => bei.EquipmentItem)
            .WithMany()
            .HasForeignKey(bei => bei.EquipmentItemID)
            .OnDelete(DeleteBehavior.Restrict);

        // Requester on a borrowing request. Restrict delete: a user account
        // should not be deletable while it still has borrowing history attached.
        modelBuilder.Entity<BorrowingRequests>()
            .HasOne(r => r.RequesterUser)
            .WithMany(u => u.BorrowingRequests)
            .HasForeignKey(r => r.RequesterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Submitter of a ticket. Restrict delete for the same reason as above.
        modelBuilder.Entity<Ticketing>()
            .HasOne(t => t.SubmittedByUser)
            .WithMany(u => u.SubmittedTickets)
            .HasForeignKey(t => t.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Technician assigned to a ticket.
        modelBuilder.Entity<Ticketing>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
