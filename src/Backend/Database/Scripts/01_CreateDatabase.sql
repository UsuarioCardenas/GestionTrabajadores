CREATE DATABASE TrabajadoresPrueba;
GO

USE TrabajadoresPrueba;
GO

CREATE TABLE Trabajadores (
    IdTrabajador INT PRIMARY KEY IDENTITY(1,1),
    Nombres NVARCHAR(50) NOT NULL,
    Apellidos NVARCHAR(50) NOT NULL,
    TipoDocumento NVARCHAR(35) NOT NULL,
    NumeroDocumento NVARCHAR(35) NOT NULL UNIQUE,
    Sexo CHAR(1) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Foto NVARCHAR(500) NULL,
    Direccion NVARCHAR(175) NOT NULL
);
GO

CREATE PROCEDURE USP_ObtenerTodosLosTrabajadores
AS
BEGIN
    SELECT * FROM Trabajadores ORDER BY Apellidos, Nombres
END
GO