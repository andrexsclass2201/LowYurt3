using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowYurt3.Migrations
{
    /// <inheritdoc />
    public partial class FixPedidoRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id_Admin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Admin = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    Correo_Admin = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Contraseña_admin = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__3F48CD797C849503", x => x.Id_Admin);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id_Categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Categoria = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__CB9033498D28ED89", x => x.Id_Categoria);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id_Cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Segundo_Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Segundo_Apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Telefono = table.Column<long>(type: "bigint", nullable: false),
                    Direccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Contraseña = table.Column<byte[]>(type: "varbinary(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cliente__3DD0A8CB3BDD35E1", x => x.Id_Cliente);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Id_Producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Producto = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Valor_Unitario = table.Column<int>(type: "int", nullable: false),
                    Imagen_Producto = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Id_Categoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__2085A9CF645FAFBA", x => x.Id_Producto);
                    table.ForeignKey(
                        name: "FK__Producto__Id_Cat__2334397B",
                        column: x => x.Id_Categoria,
                        principalTable: "Categoria",
                        principalColumn: "Id_Categoria");
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id_Pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha_Compra = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Cantidad_Productos = table.Column<int>(type: "int", nullable: false),
                    Valor_Total = table.Column<int>(type: "int", nullable: false),
                    Id_Cliente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pedido__A5D00139DB9C6294", x => x.Id_Pedido);
                    table.ForeignKey(
                        name: "FK__Pedido__Id_Clien__22401542",
                        column: x => x.Id_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "Id_Cliente");
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    Id_Pago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metodo_Pago = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    Valor_Envio = table.Column<int>(type: "int", nullable: true),
                    Pago_Total = table.Column<int>(type: "int", nullable: false),
                    Id_Pedido = table.Column<int>(type: "int", nullable: false),
                    PedidoIdPedido = table.Column<int>(type: "int", nullable: false),
                    IdClienteNavigationIdCliente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pago__3E79AD9A1F589008", x => x.Id_Pago);
                    table.ForeignKey(
                        name: "FK_Pago_Cliente_IdClienteNavigationIdCliente",
                        column: x => x.IdClienteNavigationIdCliente,
                        principalTable: "Cliente",
                        principalColumn: "Id_Cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pago_Pedido_PedidoIdPedido",
                        column: x => x.PedidoIdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id_Pedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Pago__Id_Pedido__24285DB4",
                        column: x => x.Id_Pedido,
                        principalTable: "Pedido",
                        principalColumn: "Id_Pedido");
                });

            migrationBuilder.CreateTable(
                name: "Pedido_Detalle",
                columns: table => new
                {
                    Id_Pedido = table.Column<int>(type: "int", nullable: false),
                    Id_Producto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    PedidoIdPedido = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pedido_D__47D85BA59CF9F2AC", x => new { x.Id_Pedido, x.Id_Producto });
                    table.ForeignKey(
                        name: "FK_Pedido_Detalle_Pedido_PedidoIdPedido",
                        column: x => x.PedidoIdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id_Pedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Pedido_De__Id_Pe__1C873BEC",
                        column: x => x.Id_Pedido,
                        principalTable: "Pedido",
                        principalColumn: "Id_Pedido");
                    table.ForeignKey(
                        name: "FK__Pedido_De__Id_Pr__1D7B6025",
                        column: x => x.Id_Producto,
                        principalTable: "Producto",
                        principalColumn: "Id_Producto");
                });

            migrationBuilder.CreateTable(
                name: "Envio_Pedido",
                columns: table => new
                {
                    Id_Envio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Direccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Id_Pedido = table.Column<int>(type: "int", nullable: false),
                    Id_Cliente = table.Column<int>(type: "int", nullable: false),
                    Id_Pago = table.Column<int>(type: "int", nullable: false),
                    IdPedidoNavigationIdPedido = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Envio_Pe__3A8388337EACD537", x => x.Id_Envio);
                    table.ForeignKey(
                        name: "FK_Envio_Pedido_Pedido_IdPedidoNavigationIdPedido",
                        column: x => x.IdPedidoNavigationIdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id_Pedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Envio_Ped__Id_Cl__27F8EE98",
                        column: x => x.Id_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "Id_Cliente");
                    table.ForeignKey(
                        name: "FK__Envio_Ped__Id_Pa__2610A626",
                        column: x => x.Id_Pago,
                        principalTable: "Pago",
                        principalColumn: "Id_Pago");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Envio_Pedido_Id_Cliente",
                table: "Envio_Pedido",
                column: "Id_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Envio_Pedido_Id_Pago",
                table: "Envio_Pedido",
                column: "Id_Pago");

            migrationBuilder.CreateIndex(
                name: "IX_Envio_Pedido_IdPedidoNavigationIdPedido",
                table: "Envio_Pedido",
                column: "IdPedidoNavigationIdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_Id_Pedido",
                table: "Pago",
                column: "Id_Pedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_IdClienteNavigationIdCliente",
                table: "Pago",
                column: "IdClienteNavigationIdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_PedidoIdPedido",
                table: "Pago",
                column: "PedidoIdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Id_Cliente",
                table: "Pedido",
                column: "Id_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Detalle_Id_Producto",
                table: "Pedido_Detalle",
                column: "Id_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Detalle_PedidoIdPedido",
                table: "Pedido_Detalle",
                column: "PedidoIdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Id_Categoria",
                table: "Producto",
                column: "Id_Categoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Envio_Pedido");

            migrationBuilder.DropTable(
                name: "Pedido_Detalle");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Cliente");
        }
    }
}
