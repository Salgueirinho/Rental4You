using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Data.Migrations
{
    public partial class fixrelations_etc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_AspNetUsers_ApplicationUserId",
                table: "Cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Cliente_ClienteId",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Funcionarios_FuncionarioEntregaId",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Funcionarios_FuncionarioRecebeId",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Veiculos_VeiculoId",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Categoria_CategoriaId",
                table: "Veiculos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reserva",
                table: "Reserva");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria");

            migrationBuilder.RenameTable(
                name: "Reserva",
                newName: "Reservas");

            migrationBuilder.RenameTable(
                name: "Cliente",
                newName: "Clientes");

            migrationBuilder.RenameTable(
                name: "Categoria",
                newName: "Categorias");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_VeiculoId",
                table: "Reservas",
                newName: "IX_Reservas_VeiculoId");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_FuncionarioRecebeId",
                table: "Reservas",
                newName: "IX_Reservas_FuncionarioRecebeId");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_FuncionarioEntregaId",
                table: "Reservas",
                newName: "IX_Reservas_FuncionarioEntregaId");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_ClienteId",
                table: "Reservas",
                newName: "IX_Reservas_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Cliente_ApplicationUserId",
                table: "Clientes",
                newName: "IX_Clientes_ApplicationUserId");

            migrationBuilder.AddColumn<bool>(
                name: "Confirmado",
                table: "Reservas",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Clientes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservas",
                table: "Reservas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_AspNetUsers_ApplicationUserId",
                table: "Clientes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Funcionarios_FuncionarioEntregaId",
                table: "Reservas",
                column: "FuncionarioEntregaId",
                principalTable: "Funcionarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Funcionarios_FuncionarioRecebeId",
                table: "Reservas",
                column: "FuncionarioRecebeId",
                principalTable: "Funcionarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Veiculos_VeiculoId",
                table: "Reservas",
                column: "VeiculoId",
                principalTable: "Veiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Categorias_CategoriaId",
                table: "Veiculos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_AspNetUsers_ApplicationUserId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Funcionarios_FuncionarioEntregaId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Funcionarios_FuncionarioRecebeId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Veiculos_VeiculoId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Categorias_CategoriaId",
                table: "Veiculos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservas",
                table: "Reservas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Confirmado",
                table: "Reservas");

            migrationBuilder.RenameTable(
                name: "Reservas",
                newName: "Reserva");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "Cliente");

            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "Categoria");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_VeiculoId",
                table: "Reserva",
                newName: "IX_Reserva_VeiculoId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_FuncionarioRecebeId",
                table: "Reserva",
                newName: "IX_Reserva_FuncionarioRecebeId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_FuncionarioEntregaId",
                table: "Reserva",
                newName: "IX_Reserva_FuncionarioEntregaId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_ClienteId",
                table: "Reserva",
                newName: "IX_Reserva_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_ApplicationUserId",
                table: "Cliente",
                newName: "IX_Cliente_ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Cliente",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reserva",
                table: "Reserva",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_AspNetUsers_ApplicationUserId",
                table: "Cliente",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Cliente_ClienteId",
                table: "Reserva",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Funcionarios_FuncionarioEntregaId",
                table: "Reserva",
                column: "FuncionarioEntregaId",
                principalTable: "Funcionarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Funcionarios_FuncionarioRecebeId",
                table: "Reserva",
                column: "FuncionarioRecebeId",
                principalTable: "Funcionarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Veiculos_VeiculoId",
                table: "Reserva",
                column: "VeiculoId",
                principalTable: "Veiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Categoria_CategoriaId",
                table: "Veiculos",
                column: "CategoriaId",
                principalTable: "Categoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
