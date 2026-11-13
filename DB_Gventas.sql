CREATE DATABASE DB_Gventas;
GO
USE DB_Gventas;
GO

-- 1. TABLA CLIENTES
CREATE TABLE tb_clientes (
    codigo CHAR(5) PRIMARY KEY,
    ruc VARCHAR(11) NOT NULL,
    nombres VARCHAR(120) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    direccion VARCHAR(200) NOT NULL
);
GO

-- 2. TABLA PRODUCTOS
CREATE TABLE tb_productos (
    codigo CHAR(5) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    unidad VARCHAR(30) NOT NULL,
    precio DECIMAL(10, 2) NOT NULL
);
GO

-- 3. TABLA CABECERA DE PEDIDOS
CREATE TABLE tb_pedidos_cab (
    idpedido INT IDENTITY(1,1) PRIMARY KEY,
    fecha DATETIME DEFAULT GETDATE(),
    cod_cliente CHAR(5) REFERENCES tb_clientes(codigo),
    total DECIMAL(10, 2) NOT NULL
);
GO

-- 4. TABLA DETALLE DE PEDIDOS
CREATE TABLE tb_pedidos_det (
    idpedido INT REFERENCES tb_pedidos_cab(idpedido),
    cod_producto CHAR(5) REFERENCES tb_productos(codigo),
    cantidad INT NOT NULL,
    precio DECIMAL(10, 2) NOT NULL,
    subtotal AS (cantidad * precio),
    PRIMARY KEY (idpedido, cod_producto)
);
GO

-- PROCEDIMIENTOS ALMACENADOS: CLIENTES (CRUD)
CREATE OR ALTER PROC usp_clientes_listar AS
    SELECT codigo, ruc, nombres, telefono, direccion FROM tb_clientes;
GO

CREATE OR ALTER PROC usp_clientes_buscar @codigo CHAR(5) AS
    SELECT codigo, ruc, nombres, telefono, direccion FROM tb_clientes WHERE codigo = @codigo;
GO

CREATE OR ALTER PROC usp_clientes_merge
    @codigo CHAR(5), @ruc VARCHAR(11), @nombres VARCHAR(120), @telefono VARCHAR(20), @direccion VARCHAR(200)
AS
    MERGE tb_clientes AS target
    USING (SELECT @codigo, @ruc, @nombres, @telefono, @direccion) AS src (cod, ruc, nom, tel, dir)
    ON target.codigo = src.cod
    WHEN MATCHED THEN
        UPDATE SET ruc = src.ruc, nombres = src.nom, telefono = src.tel, direccion = src.dir
    WHEN NOT MATCHED THEN
        INSERT VALUES (src.cod, src.ruc, src.nom, src.tel, src.dir);
GO

CREATE OR ALTER PROC usp_clientes_eliminar @codigo CHAR(5) AS
    DELETE FROM tb_clientes WHERE codigo = @codigo;
GO

-- PROCEDIMIENTOS ALMACENADOS: PRODUCTOS (CRUD)
CREATE OR ALTER PROC usp_productos_listar AS
    SELECT codigo, nombre, unidad, precio FROM tb_productos;
GO

CREATE OR ALTER PROC usp_productos_merge
    @codigo CHAR(5), @nombre VARCHAR(100), @unidad VARCHAR(30), @precio DECIMAL(10,2)
AS
    MERGE tb_productos AS target
    USING (SELECT @codigo, @nombre, @unidad, @precio) AS src (cod, nom, uni, pre)
    ON target.codigo = src.cod
    WHEN MATCHED THEN
        UPDATE SET nombre = src.nom, unidad = src.uni, precio = src.pre
    WHEN NOT MATCHED THEN
        INSERT VALUES (src.cod, src.nom, src.uni, src.pre);
GO

CREATE OR ALTER PROC usp_productos_eliminar @codigo CHAR(5) AS
    DELETE FROM tb_productos WHERE codigo = @codigo;
GO

-- DATOS DE PRUEBA
INSERT INTO tb_clientes VALUES 
('C0001', '20100123456', 'Comercial Lima SAC', '01-4192900', 'Av. Brasil 651 - Breña'),
('C0002', '20548796321', 'Distribuidora Norte SRL', '01-3254100', 'Jr. Unión 450 - Lima');

INSERT INTO tb_productos VALUES 
('P0001', 'Laptop Core i7', 'Unidad', 3500.00), 
('P0002', 'Mouse Inalámbrico', 'Caja', 45.50),
('P0003', 'Monitor 24 Pulgadas', 'Unidad', 680.00),
('P0004', 'Teclado Mecánico RGB', 'Unidad', 190.00);
GO
