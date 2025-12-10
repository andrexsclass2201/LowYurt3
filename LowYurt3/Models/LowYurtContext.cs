using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LowYurt3.Models;

public partial class LowYurtContext : DbContext
{
    public LowYurtContext()
    {
    }

    public LowYurtContext(DbContextOptions<LowYurtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<EnvioPedido> EnvioPedidos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MAFAS; database=LowYurt; integrated security=true;TrustServerCertificate=True;");
    */

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.IdAdmin).HasName("PK__Admin__3F48CD797C849503");

            entity.ToTable("Admin");

            entity.Property(e => e.IdAdmin).HasColumnName("Id_Admin");
            entity.Property(e => e.ContraseñaAdmin)
                .HasMaxLength(50)
                .HasColumnName("Contraseña_admin");
            entity.Property(e => e.CorreoAdmin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Correo_Admin");
            entity.Property(e => e.NombreAdmin)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("Nombre_Admin");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__CB9033498D28ED89");

            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Categoria");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__3DD0A8CB3BDD35E1");

            entity.ToTable("Cliente");

            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Contraseña).HasMaxLength(45);
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Segundo_Apellido");
            entity.Property(e => e.SegundoNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Segundo_Nombre");
        });

        modelBuilder.Entity<EnvioPedido>(entity =>
        {
            entity.HasKey(e => e.IdEnvio).HasName("PK__Envio_Pe__3A8388337EACD537");

            entity.ToTable("Envio_Pedido");

            entity.Property(e => e.IdEnvio).HasColumnName("Id_Envio");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.IdPago).HasColumnName("Id_Pago");
            entity.Property(e => e.IdPedido).HasColumnName("Id_Pedido");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago);

            entity.ToTable("Pago");

            entity.Property(e => e.IdPago).HasColumnName("Id_Pago");
            entity.Property(e => e.MetodoPago).HasColumnName("Metodo_Pago");
            entity.Property(e => e.ValorEnvio).HasColumnName("Valor_Envio");
            entity.Property(e => e.PagoTotal).HasColumnName("Pago_Total");
            entity.Property(e => e.IdPedido).HasColumnName("Id_Pedido");

            // RELACIÓN CORRECTA CON PEDIDO
            entity.HasOne(p => p.Pedido)
                  .WithMany(p => p.Pagos)
                  .HasForeignKey(p => p.IdPedido)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Pago_Pedido");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido);
            entity.Property(e => e.IdPedido).HasColumnName("Id_Pedido");
            entity.Property(e => e.FechaCompra).HasColumnName("Fecha_Compra").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CantidadProductos).HasColumnName("Cantidad_Productos");
            entity.Property(e => e.ValorTotal).HasColumnName("Valor_Total");
            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");

            entity.HasOne(d => d.Cliente)
                  .WithMany(p => p.Pedidos)
                  .HasForeignKey(d => d.IdCliente)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Pedido_Cliente");
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.HasKey(e => new { e.IdPedido, e.IdProducto });

            entity.ToTable("Pedido_Detalle");

            entity.Property(e => e.IdPedido).HasColumnName("Id_Pedido");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.Cantidad).HasColumnName("Cantidad");

            entity.HasOne(d => d.Pedido)
                  .WithMany(p => p.PedidoDetalles)
                  .HasForeignKey(d => d.IdPedido)
                  .HasConstraintName("FK_PedidoDetalle_Pedido");

            entity.HasOne(d => d.Producto)
                  .WithMany(p => p.PedidoDetalles)
                  .HasForeignKey(d => d.IdProducto)
                  .HasConstraintName("FK_PedidoDetalle_Producto");
        });


        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__2085A9CF645FAFBA");

            entity.ToTable("Producto");

            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.ImagenProducto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Imagen_Producto");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Producto");
            entity.Property(e => e.ValorUnitario).HasColumnName("Valor_Unitario");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producto__Id_Cat__2334397B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
