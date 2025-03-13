using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace chicchicProgForHaircuts.Models;

public partial class GoodhaircutContext : DbContext
{
    public GoodhaircutContext()
    {
    }

    public GoodhaircutContext(DbContextOptions<GoodhaircutContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Clientstatus> Clientstatuses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Haircut> Haircuts { get; set; }

    public virtual DbSet<Haircutsgender> Haircutsgenders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=goodhaircut;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("appointment_date");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FinalPrice).HasColumnName("final_price");
            entity.Property(e => e.HaircutId).HasColumnName("haircut_id");

            entity.HasOne(d => d.Client).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("appointments_client_id_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("appointments_employee_id_fkey");

            entity.HasOne(d => d.Haircut).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.HaircutId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("appointments_haircut_id_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.HasIndex(e => e.PhoneNumber, "clients_phone_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.NameClient)
                .HasMaxLength(50)
                .HasColumnName("name_client");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.PatronymicClient)
                .HasMaxLength(50)
                .HasColumnName("patronymic_client");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.SurnameClient)
                .HasMaxLength(50)
                .HasColumnName("surname_client");
            entity.Property(e => e.VisitCount)
                .HasDefaultValueSql("0")
                .HasColumnName("visit_count");

            entity.HasOne(d => d.GenderNavigation).WithMany(p => p.Clients)
                .HasForeignKey(d => d.Gender)
                .HasConstraintName("clients_gender_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Clients)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("clients_status_id_fkey");
        });

        modelBuilder.Entity<Clientstatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clientstatus_pkey");

            entity.ToTable("clientstatus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(100)
                .HasColumnName("contact_info");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.NameEmployee)
                .HasMaxLength(50)
                .HasColumnName("name_employee");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.PatronymicEmployee)
                .HasMaxLength(50)
                .HasColumnName("patronymic_employee");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.SurnameEmployee)
                .HasMaxLength(50)
                .HasColumnName("surname_employee");

            entity.HasOne(d => d.GenderNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Gender)
                .HasConstraintName("employees_gender_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("employees_role_id_fkey");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genders_pkey");

            entity.ToTable("genders");

            entity.HasIndex(e => e.GenderName, "genders_gender_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GenderName)
                .HasMaxLength(20)
                .HasColumnName("gender_name");
        });

        modelBuilder.Entity<Haircut>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("haircuts_pkey");

            entity.ToTable("haircuts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.GenderNavigation).WithMany(p => p.Haircuts)
                .HasForeignKey(d => d.Gender)
                .HasConstraintName("haircuts_gender_fkey");
        });

        modelBuilder.Entity<Haircutsgender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("haircutsgenders_pkey");

            entity.ToTable("haircutsgenders");

            entity.HasIndex(e => e.HairgenderName, "haircutsgenders_hairgender_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HairgenderName)
                .HasMaxLength(20)
                .HasColumnName("hairgender_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "roles_role_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
