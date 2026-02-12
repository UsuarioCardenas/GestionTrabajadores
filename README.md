# 🧑‍💼 Módulo de Gestión de Trabajadores

**Sistema de Gestión de Recursos Humanos | MYPER Software**

---

## 📋 Descripción

Sistema web para la gestión de trabajadores desarrollado con **.NET 8**, **Blazor WebAssembly** y **Entity Framework Core**. Permite realizar operaciones CRUD completas sobre registros de trabajadores con una interfaz moderna y responsiva.

### ✨ Funcionalidades Implementadas

- ✅ **Listado de trabajadores** - Visualización de todos los registros con procedimiento almacenado
- ✅ **Registro de trabajador** - Creación mediante modal con validaciones
- ✅ **Edición de trabajador** - Actualización de datos mediante modal
- ✅ **Eliminación de trabajador** - Con mensaje de confirmación requerido
- ✅ **Filtro por sexo** - Filtrado por Masculino/Femenino *(Bonus)*
- ✅ **Coloreo de filas** - Azul para masculino, naranja para femenino *(Bonus)*
- ✅ **Subida de fotos** - Integración con Cloudinary para almacenamiento de imágenes
- ✅ **Validaciones robustas** - Campos requeridos, documento único, formatos

---

## 🏗️ Arquitectura del Proyecto

El proyecto sigue una **Arquitectura Limpia (Clean Architecture)** con separación en capas:

```
GestiónTrabajadores/
├── src/
│   ├── GestiónTrabajadores.API/           # Capa de presentación - API REST
│   ├── GestiónTrabajadores.Application/   # Capa de aplicación - Servicios, DTOs, Validaciones
│   ├── GestiónTrabajadores.Domain/        # Capa de dominio - Entidades
│   ├── GestiónTrabajadores.Infrastructure/# Capa de infraestructura - EF Core, Repositorios
│   └── GestiónTrabajadores.Web/           # Frontend - Blazor WebAssembly
├── test/
│   ├── GestiónTrabajadores.UnitTests/     # Pruebas unitarias
│   └── GestiónTrabajadores.IntegrationTests/ # Pruebas de integración
└── docs/
    └── QA/                                 # Documentación de QA y evidencias
```

### 🛠️ Tecnologías Utilizadas

| Capa | Tecnología |
|------|------------|
| Frontend | Blazor WebAssembly, Bootstrap 5 |
| Backend | .NET 8, ASP.NET Core Web API |
| Base de Datos | SQL Server, Entity Framework Core |
| Validaciones | FluentValidation |
| Testing | xUnit, Moq, FluentAssertions |
| Almacenamiento de Imágenes | Cloudinary |

### 📐 Patrones Aplicados

- Repository Pattern
- Dependency Injection
- DTOs (Data Transfer Objects)
- Clean Architecture

---

## 📦 Entregables

### 1. 🎨 Prototipo de Interfaz (Figma)

Diseño visual de las pantallas del módulo incluyendo listado, registro, edición y eliminación.

🔗 **[Ver Prototipo en Figma](https://www.figma.com/design/2g5Tbn9BawEM8LGn1Fyslr/Prueba-T%C3%A9cnica---Myper?node-id=19-113&t=w0X5FntL4wwFd8es-1)**

---

### 2. 💾 Repositorio de Código (GitHub)

Código fuente completo del proyecto con commits descriptivos.

🔗 **[Ver Repositorio en GitHub](https://github.com/UsuarioCardenas/GestionTrabajadores)**

---

### 3. 🗃️ Script de Base de Datos

Script SQL para crear la base de datos `TrabajadoresPrueba` con tablas y procedimientos almacenados.

🔗 **[Ver Script SQL](./database/script.sql)**

---

### 4. 📝 Documentación de QA

Documento completo de validación y pruebas con casos de prueba funcionales, pruebas unitarias, pruebas de integración y evidencias visuales.

🔗 **[Ver Documento de QA](./docs/QA/Documento_QA_ModuloTrabajadores.md)**

📁 **[Ver Evidencias Visuales](./docs/QA/evidencias/)**

---

### 5. 🎥 Video de Presentación (Loom)

Explicación técnica del proyecto, arquitectura, decisiones de desarrollo y demostración de la aplicación funcionando.

🔗 **[Ver Video en Loom](#)** *(Pendiente de agregar enlace)*

---

## 🚀 Cómo Ejecutar el Proyecto

### Prerrequisitos

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 o VS Code

### Pasos

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/UsuarioCardenas/GestionTrabajadores.git
   ```

2. **Ejecutar el script de base de datos**
   - Abrir SQL Server Management Studio
   - Ejecutar el archivo `database/script.sql`

3. **Configurar la cadena de conexión**
   - Actualizar `appsettings.json` en el proyecto API con tu conexión a SQL Server

4. **Ejecutar la aplicación**
   ```bash
   cd src/GestiónTrabajadores.API
   dotnet run
   ```

5. **Ejecutar las pruebas**
   ```bash
   dotnet test
   ```

---

## 👤 Autor

**Diego Alessandro Cardenas Garcia**

📅 Junio 2025
