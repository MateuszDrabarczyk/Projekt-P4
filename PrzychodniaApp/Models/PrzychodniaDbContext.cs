using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PrzychodniaApp.Models;

public partial class PrzychodniaDbContext : DbContext
{
    public PrzychodniaDbContext()
    {
    }

    public PrzychodniaDbContext(DbContextOptions<PrzychodniaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Audyt> Audyts { get; set; }

    public virtual DbSet<Lekarze> Lekarzes { get; set; }

    public virtual DbSet<Pacjenci> Pacjencis { get; set; }

    public virtual DbSet<SlIcd10> SlIcd10s { get; set; }

    public virtual DbSet<SlRole> SlRoles { get; set; }

    public virtual DbSet<SlStatusy> SlStatusies { get; set; }

    public virtual DbSet<SlUslugi> SlUslugis { get; set; }

    public virtual DbSet<Uzytkownicy> Uzytkownicies { get; set; }

    public virtual DbSet<Wizyty> Wizyties { get; set; }

    public virtual DbSet<WizytyUslugi> WizytyUslugis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-G58O04B\\SQLEXPRESS;Database=PrzychodniaDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Konfiguracja tabeli AUDYT
        modelBuilder.Entity<Audyt>(entity =>
        {
            entity.HasKey(e => e.IdLogu).HasName("PK__AUDYT__38DA3380E0028CF3");

            entity.ToTable("AUDYT");

            entity.Property(e => e.Akcja).HasMaxLength(100);
            entity.Property(e => e.DataLogu)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUzytkownikaNavigation).WithMany(p => p.Audyts)
                .HasForeignKey(d => d.IdUzytkownika)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Audyt_Uzytkownik");
        });

        // 2. Konfiguracja tabeli LEKARZE
        modelBuilder.Entity<Lekarze>(entity =>
        {
            entity.HasKey(e => e.IdUzytkownika).HasName("PK__LEKARZE__614CA422593DCC77");

            entity.ToTable("LEKARZE");

            entity.HasIndex(e => e.NumerPwz, "UQ__LEKARZE__1C61ED2449CB3AD5").IsUnique();

            entity.Property(e => e.IdUzytkownika).ValueGeneratedNever();
            entity.Property(e => e.NumerPwz)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NumerPWZ");

            entity.HasOne(d => d.IdUzytkownikaNavigation).WithOne(p => p.Lekarze)
                .HasForeignKey<Lekarze>(d => d.IdUzytkownika)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lekarz_Uzytkownik");
        });

        // 3. Połączona i pełna konfiguracja tabeli PACJENCI 
        modelBuilder.Entity<Pacjenci>(entity =>
        {
            entity.HasKey(e => e.IdPacjenta).HasName("PK__PACJENCI__81982F5AC33A4B8B");


            entity.ToTable("PACJENCI", tb => tb.HasTrigger("TRG_Pacjenci_WalidacjaFormaty"));

            entity.HasIndex(e => e.Pesel, "UQ__PACJENCI__48A5F7178DC47A38").IsUnique();

            entity.Property(e => e.Adres).HasMaxLength(255);
            entity.Property(e => e.Imie).HasMaxLength(50);
            entity.Property(e => e.Nazwisko).HasMaxLength(50);
            entity.Property(e => e.Pesel)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Telefon).HasMaxLength(15);
        });

        // 4. Konfiguracja tabeli SL_ICD10
        modelBuilder.Entity<SlIcd10>(entity =>
        {
            entity.HasKey(e => e.Kod).HasName("PK__SL_ICD10__C41FEDBD2A9A6089");

            entity.ToTable("SL_ICD10");

            entity.Property(e => e.Kod).HasMaxLength(10);
            entity.Property(e => e.Opis).HasMaxLength(255);
        });

        // 5. Konfiguracja tabeli SL_ROLE
        modelBuilder.Entity<SlRole>(entity =>
        {
            entity.HasKey(e => e.IdRoli).HasName("PK__SL_ROLE__B4369050E3EC0116");

            entity.ToTable("SL_ROLE");

            entity.HasIndex(e => e.Nazwa, "UQ__SL_ROLE__602223FF0D97D3EC").IsUnique();

            entity.Property(e => e.Nazwa).HasMaxLength(50);
        });

        // 6. Konfiguracja tabeli SL_STATUSY
        modelBuilder.Entity<SlStatusy>(entity =>
        {
            entity.HasKey(e => e.IdStatusu).HasName("PK__SL_STATU__8E121CD9DDD75533");

            entity.ToTable("SL_STATUSY");

            entity.HasIndex(e => e.Nazwa, "UQ__SL_STATU__602223FF6C428EE9").IsUnique();

            entity.Property(e => e.Nazwa).HasMaxLength(50);
        });

        // 7. Konfiguracja tabeli SL_USLUGI
        modelBuilder.Entity<SlUslugi>(entity =>
        {
            entity.HasKey(e => e.IdUslugi).HasName("PK__SL_USLUG__E504B1B550DE9D52");

            entity.ToTable("SL_USLUGI");

            entity.Property(e => e.CenaAktualna).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Nazwa).HasMaxLength(100);
        });

        // 8. Konfiguracja tabeli UZYTKOWNICY
        modelBuilder.Entity<Uzytkownicy>(entity =>
        {
            entity.HasKey(e => e.IdUzytkownika).HasName("PK__UZYTKOWN__614CA4226C6605F6");

            entity.ToTable("UZYTKOWNICY");

            entity.HasIndex(e => e.Login, "UQ__UZYTKOWN__5E55825B9AB4A14E").IsUnique();

            entity.Property(e => e.CzyAktywny).HasDefaultValue(true);
            entity.Property(e => e.HasloHash).HasMaxLength(255);
            entity.Property(e => e.Login).HasMaxLength(50);

            entity.HasOne(d => d.IdRoliNavigation).WithMany(p => p.Uzytkownicies)
                .HasForeignKey(d => d.IdRoli)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Uzytkownik_Rola");
        });

        // 9. Konfiguracja tabeli WIZYTY
        modelBuilder.Entity<Wizyty>(entity =>
        {
            entity.HasKey(e => e.IdWizyty).HasName("PK__WIZYTY__4E043266A550AD8E");

            entity.ToTable("WIZYTY", tb =>
            {
                tb.HasTrigger("TRG_BlokadaTerminow");
                tb.HasTrigger("TRG_Wizyty_Audit");
            });

            entity.Property(e => e.DataKoniec).HasColumnType("datetime");
            entity.Property(e => e.DataStart).HasColumnType("datetime");
            entity.Property(e => e.KodIcd10)
                .HasMaxLength(10)
                .HasColumnName("KodICD10");

            entity.HasOne(d => d.IdLekarzaNavigation).WithMany(p => p.Wizyties)
                .HasForeignKey(d => d.IdLekarza)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wizyta_Lekarz");

            entity.HasOne(d => d.IdPacjentaNavigation).WithMany(p => p.Wizyties)
                .HasForeignKey(d => d.IdPacjenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wizyta_Pacjent");

            entity.HasOne(d => d.IdStatusuNavigation).WithMany(p => p.Wizyties)
                .HasForeignKey(d => d.IdStatusu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wizyta_Status");

            entity.HasOne(d => d.KodIcd10Navigation).WithMany(p => p.Wizyties)
                .HasForeignKey(d => d.KodIcd10)
                .HasConstraintName("FK_Wizyta_ICD10");
        });

        // 10. Konfiguracja tabeli WIZYTY_USLUGI
        modelBuilder.Entity<WizytyUslugi>(entity =>
        {
            entity.HasKey(e => e.IdPozycji).HasName("PK__WIZYTY_U__5331C5EC1F363515");

            entity.ToTable("WIZYTY_USLUGI");

            entity.Property(e => e.CenaHistoryczna).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdUslugiNavigation).WithMany(p => p.WizytyUslugis)
                .HasForeignKey(d => d.IdUslugi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pozycja_Usluga");

            entity.HasOne(d => d.IdWizytyNavigation).WithMany(p => p.WizytyUslugis)
                .HasForeignKey(d => d.IdWizyty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pozycja_Wizyta");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}