# 🏛️ Sistema de Gestión para Despachos de Abogados

<div align="center">

**Plataforma para la gestión eficiente de despachos jurídicos**

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.x-4FC08D?logo=vue.js)](https://vuejs.org/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

---

## 📋 Descripción

Sistema web para la gestión integral de despachos de abogados que permite administrar clientes, expedientes legales, citas, actuaciones judiciales y documentación de forma centralizada y eficiente.

La aplicación ofrece una interfaz intuitiva para el seguimiento de casos desde su apertura hasta su cierre, manteniendo un historial completo de todas las actuaciones realizadas y facilitando la generación de reportes profesionales.

---

## ✨ Características Principales

### 👥 Gestión de Clientes
- Registro completo de personas físicas y jurídicas
- Directorio de contactos con datos de acceso rápido
- Búsqueda avanzada y filtros
- Historial de expedientes por cliente

### 📁 Expedientes Jurídicos
- Creación y seguimiento de casos legales
- Clasificación por tipo: Civil, Penal, Laboral, Mercantil, Familia
- Control de estados: Abierto, En Trámite, Suspendido, Archivado, Cerrado
- Vinculación con juzgados y números de procedimiento

### 📅 Agenda de Citas
- Programación de vistas judiciales, reuniones y consultas
- Vista de calendario mensual
- Recordatorios de citas próximas y pendientes
- Gestión de participantes y ubicaciones

### 📝 Registro de Actuaciones
- Historial cronológico completo de cada expediente
- Tipos de actuaciones: Reuniones, Escritos, Comparecencias, Notificaciones
- Registro de responsables y resultados obtenidos
- Seguimiento detallado del progreso de cada caso

### 📊 Gestión de Informes
- Generación automática de informes en PDF
- Reporte de clientes con datos completos
- Análisis de expedientes por estado
- Historial detallado de actuaciones por expediente
- Trazabilidad: todos los reportes se almacenan y pueden descargarse posteriormente

---

## 🖼️ Capturas de Pantalla

### Gestión de Clientes
![Lista de Clientes](/imagenes/clientes.png)
*Directorio completo de clientes con búsqueda y filtros*

### Expedientes de los Clientes
![Expedientes](/imagenes/expedientes.png)
*Vista completa de los expedientes*

### Detalles Actuaciones
![Actuaciones](/imagenes/actuaciones.png)
*Actuaciones registradas en cada expediente*

### Calendario de Citas
![Calendario](/imagenes/citas.png)
*Agenda de eventos, vistas judiciales y reuniones*

### Calendario de Citas
![Documentos](/imagenes/documentos.png)
*Documentos guardados y generación de informes*

### Documentación de API
![Swagger UI](/imagenes/swagger.png)
*Documentación interactiva de la API REST con Swagger*

---

## 🚀 Tecnologías Utilizadas

### Backend
- **ASP.NET Core 9.0** - Framework web moderno y de alto rendimiento
- **Entity Framework Core 9.0** - ORM para acceso a datos
- **MySQL 8.0** - Base de datos relacional
- **QuestPDF** - Generación de documentos PDF
- **Swagger/OpenAPI** - Documentación automática de API

### Frontend
- **Vue.js 3** - Framework JavaScript progresivo
- **TypeScript** - JavaScript con tipado estático
- **Vite** - Build tool ultra-rápido
- **Vue Router** - Enrutamiento SPA

### Infraestructura
- **Docker & Docker Compose** - Containerización y orquestación
- **Nginx** - Proxy inverso y servidor web
- **phpMyAdmin** - Administración de base de datos

### Testing
- **xUnit** - Framework de testing para .NET
- **FluentAssertions** - Aserciones expresivas
- **Moq** - Mocking framework

---

## 📦 Instalación y Uso

### Requisitos Previos

- [Docker](https://www.docker.com/get-started) instalado
- [Docker Compose](https://docs.docker.com/compose/install/) instalado

### Inicio Rápido

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/CainMartinez/ASPNET-Vue-DespachoAbogados.git
   cd ASP-Abogados
   ```

2. **Iniciar la aplicación con Docker**
   ```bash
   docker-compose up -d --build
   ```

3. **Acceder a la aplicación**
   - **Aplicación Web**: http://localhost
   - **Swagger UI**: http://localhost/swagger
   - **phpMyAdmin**: http://localhost:8081

4. **Detener la aplicación**
   ```bash
   docker-compose down
   ```

### Datos Iniciales

La aplicación incluye datos de prueba para facilitar la exploración:
- 4 clientes de ejemplo
- 5 expedientes en diferentes estados
- Citas y actuaciones de demostración

---

### Documentación de API
La documentación interactiva de la API está disponible en:
- **Swagger UI**: http://localhost/swagger (cuando la aplicación esté corriendo)

---

## 🏗️ Arquitectura del Sistema

```
┌─────────────────────────────────────────────────────────┐
│                     NGINX (Puerto 80)                    │
│              Proxy Inverso & Load Balancer              │
└───────────────────┬─────────────────┬───────────────────┘
                    │                 │
        ┌───────────▼──────────┐     ┌▼──────────────────┐
        │   Frontend (Vue.js)  │     │  Backend (API)    │
        │      Puerto 5173     │     │   Puerto 6050     │
        │   ┌──────────────┐   │     │ ┌──────────────┐  │
        │   │ Vue Router   │   │     │ │ Controllers  │  │
        │   │ Components   │   │     │ │  Services    │  │
        │   │  Services    │   │     │ │ EF Core      │  │
        │   └──────────────┘   │     │ └──────┬───────┘  │
        └──────────────────────┘     └────────┼──────────┘
                                              │
                                     ┌────────▼─────────┐
                                     │   MySQL 8.0      │
                                     │  Puerto 3306     │
                                     │ ┌──────────────┐ │
                                     │ │  abogados_db │ │
                                     │ └──────────────┘ │
                                     └──────────────────┘
```

**Flujo de Datos**:
1. Usuario accede a http://localhost
2. Nginx enruta al frontend (Vue.js) o al backend API según la ruta
3. Frontend realiza peticiones HTTP al backend
4. Backend procesa la lógica y accede a MySQL vía Entity Framework Core
5. Respuestas en formato JSON se envían al frontend
6. Vue.js renderiza la información en la interfaz

---

## 🧪 Testing

El proyecto incluye suite de tests:

### Ejecutar Tests del Backend

**Usando Docker (recomendado)**:
```bash
cd backend
./run-tests.sh
```

### Cobertura de Tests
- ✅ Tests Unitarios: Validación de lógica de servicios
- ✅ Tests de Integración: Validación de endpoints completos
- ✅ 3/3 tests pasando correctamente

---

## 📊 Base de Datos

### Esquema Principal

```
Cliente (1) ──────< (N) Expediente
                         │
                         ├──< (N) Actuacion
                         ├──< (N) Cita
                         └──< (N) Documento
```

### Entidades Principales
- **Cliente**: Personas físicas o jurídicas
- **Expediente**: Casos legales
- **Actuación**: Registro de acciones en expedientes
- **Cita**: Eventos programados
- **Documento**: Archivos adjuntos

---

## 🛠️ Desarrollo

### Estructura del Proyecto

```
ASP-Abogados/
├── backend/                    # Backend ASP.NET Core
│   ├── Controllers/           # Controladores de API
│   ├── Services/              # Lógica de negocio
│   ├── Models/                # Entidades de dominio
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Data/                  # Contexto de base de datos
│   └── AbogadosAPI.Tests/     # Tests unitarios e integración
├── frontend/                   # Frontend Vue.js
│   ├── src/
│   │   ├── components/        # Componentes Vue
│   │   ├── services/          # Servicios HTTP
│   │   └── router/            # Configuración de rutas
│   └── public/                # Archivos estáticos
├── nginx/                      # Configuración de proxy
│   └── nginx.conf
└── docker-compose.yml          # Orquestación de servicios
```

---

## 📄 Licencia

Este proyecto está bajo la Licencia Creative Commons. Ver el archivo [LICENSE](LICENSE) para más detalles.
