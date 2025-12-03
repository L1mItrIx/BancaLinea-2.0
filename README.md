# Banca en Línea - Sistema Bancario Integral

Sistema completo de banca en línea desarrollado con **Angular + Ionic (Frontend)** y **.NET 6 Web API (Backend)**, que permite gestión de usuarios, cuentas bancarias, transferencias, pagos de servicios, reportes y auditoría con roles diferenciados.

---

## 📋 Tabla de Contenidos
- [Características](#-características)
- [Tecnologías](#-tecnologías)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Ejecución](#-ejecución)
- [Arquitectura](#-arquitectura)
- [Roles y Permisos](#-roles-y-permisos)
- [Funcionalidades por Rol](#-funcionalidades-por-rol)
- [Endpoints API](#-endpoints-api)
- [Despliegue](#-despliegue)
- [Contribuir](#-contribuir)

---

## ✨ Características

### Generales
- ✅ Sistema multi-rol (Administrador, Gestor, Cliente)
- ✅ Autenticación con control de intentos
- ✅ Tema oscuro / claro
- ✅ Diseño responsivo (móvil, tablet, escritorio)
- ✅ Auditoría completa de acciones
- ✅ Generación de PDFs y CSVs
- ✅ Soporte para múltiples monedas (CRC/USD)
- ✅ Conversión automática de divisas

### Funcionalidades Bancarias
- 💳 Gestión de cuentas bancarias (Ahorros, Corriente, Inversión, Plazo Fijo)
- 👥 Sistema de beneficiarios con aprobación
- 💸 Transferencias entre cuentas (inmediatas y programadas)
- 🧾 Pagos de servicios (inmediatos y programados)
- 📊 Reportes y estadísticas en tiempo real
- 📄 Extractos mensuales y comprobantes
- 🔍 Historial de transacciones con filtros avanzados

---

## 🛠 Tecnologías

### Frontend
- **Framework:** Angular 17 + Ionic 8
- **Estilos:** TailwindCSS 3
- **Lenguaje:** TypeScript
- **HTTP Client:** Angular HttpClient
- **PDF Generation:** jsPDF + autoTable
- **Routing:** Angular Router (SPA)

### Backend
- **Framework:** .NET 6 Web API
- **Base de datos:** SQL Server
- **ORM:** Entity Framework Core
- **Arquitectura:** Capas (API, Business Logic, Data Access)
- **Autenticación:** Custom validation

### Despliegue
- **Frontend:** Netlify
- **Backend:** Azure / Render / Railway

---

## 📦 Requisitos Previos

### Para el Frontend
- Node.js 18+ y npm 9+
- Angular CLI 17+
- Ionic CLI 8+

### Para el Backend
- .NET SDK 6.0+
- SQL Server 2019+ o SQL Server Express
- Visual Studio 2022 o VS Code con extensiones C#

---

## 🚀 Instalación

### 1. Clonar el repositorio
```bash
git clone <URL_DEL_REPOSITORIO>
cd Proyecto
```

### 2. Instalar Frontend
```bash
npm install
```

### 3. Configurar Backend
```bash
cd API/BancaEnLinea.API
dotnet restore
```

### 4. Configurar Base de Datos

Ejecuta el script SQL ubicado en `SQLQuery2.sql` en SQL Server Management Studio o usa Entity Framework Migrations:

```bash
cd API/BancaEnLinea.API
dotnet ef database update
```

---

## ⚙️ Configuración

### Frontend (`src/environments/environment.ts`)
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5246' // Cambiar para producción
};
```

### Backend (`API/BancaEnLinea.API/appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BancaEnLinea;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

---

## ▶️ Ejecución

### Frontend (Desarrollo)
```bash
# Modo desarrollo con live reload
ionic serve

# Build de producción
ionic build

# Previsualizar build
npx http-server www
```
El frontend estará disponible en `http://localhost:8100`

### Backend
```bash
cd API/BancaEnLinea.API
dotnet run
```
La API estará disponible en `http://localhost:5246`

**Swagger UI:** `http://localhost:5246/swagger`

---

## 🏗 Arquitectura

### Frontend
```
src/
├── app/
│   ├── admin/              # Módulo de administrador
│   ├── gestor/             # Módulo de gestor
│   ├── user/               # Módulo de cliente
│   ├── login/              # Autenticación
│   ├── register/           # Registro
│   ├── home/               # Landing page
│   ├── components/         # Componentes reutilizables
│   │   ├── header/
│   │   ├── footer/
│   │   ├── tabla-*/        # Tablas especializadas
│   │   └── modal-*/        # Modales de formularios
│   ├── services/           # Servicios
│   │   ├── auth.service.ts
│   │   ├── auditoria.service.ts
│   │   └── extracto.service.ts
│   └── config/             # Configuración
│       ├── api.config.ts
│       └── crud.service.ts
└── environments/           # Variables de entorno


### Backend
```
API/
├── BancaEnLinea.API/       # Capa de presentación (Controllers)
│   ├── Controllers/
│   │   ├── CuentasController.cs
│   │   ├── CuentaBancariaController.cs
│   │   ├── BeneficiariosController.cs
│   │   ├── TransferenciasController.cs
│   │   ├── ServiciosController.cs
│   │   ├── ContratosServiciosController.cs
│   │   ├── PagosServiciosController.cs
│   │   └── HistorialController.cs
│   └── Program.cs
├── BancaEnLinea.BW/        # Lógica de negocio (Business Logic)
│   ├── CU/                 # Casos de uso
│   └── Interfaces/
├── BancaEnLinea.BC/        # Entidades y modelos
│   ├── Modelos/
│   ├── Enums/
│   └── ReglasDeNegocio/
└── BancaEnLinea.DA/        # Acceso a datos (Data Access)
    ├── Acciones/
    ├── Entidades/
    └── Config/
```

---

## 👥 Roles y Permisos

| Rol | Código | Descripción |
|-----|--------|-------------|
| **Administrador** | 0 | Control total del sistema |
| **Gestor** | 1 | Supervisión de clientes y aprobaciones |
| **Cliente** | 2 | Usuario final con acceso limitado |

---

## 🔐 Funcionalidades por Rol

### 🔴 Administrador
- ✅ Gestión completa de usuarios (CRUD)
- ✅ Gestión de cuentas bancarias de todos los clientes
- ✅ Aprobación/rechazo de beneficiarios
- ✅ Aprobación/rechazo de transferencias que superan umbral
- ✅ Gestión de servicios públicos (ICE, AyA, etc.)
- ✅ Gestión de contratos de servicios
- ✅ Visualización de historial completo
- ✅ Generación de reportes:
  - Totales por período
  - Top 10 clientes por volumen
  - Volumen diario de operaciones
- ✅ Auditoría completa del sistema

### 🟡 Gestor
- ✅ Visualización de clientes asignados
- ✅ Confirmación/rechazo de beneficiarios
- ✅ Aprobación/rechazo de transferencias mayores
- ✅ Creación/edición de cuentas bancarias de clientes
- ✅ Edición de datos básicos de clientes
- ✅ Auditoría de sus acciones

### 🟢 Cliente
- ✅ Visualización de sus cuentas y saldos
- ✅ Gestión de beneficiarios (crear, editar, eliminar)*
- ✅ Transferencias:
  - Crear transferencia inmediata o programada
  - Cancelar transferencias programadas
  - Ver historial de enviadas/recibidas
- ✅ Pagos de servicios:
  - Pagar servicios inmediatamente o programar
  - Cancelar pagos programados (24h antes)
- ✅ Historial personal con filtros
- ✅ Generación de extractos mensuales (PDF/CSV)
- ✅ Comprobantes de transacciones

*\* Requieren aprobación de Gestor/Admin*

---

## 🌐 Endpoints API

### Autenticación
- `POST /Cuentas/ValidarCuenta` - Login
- `POST /Cuentas/RegistrarCuenta` - Registro

### Cuentas
- `GET /Cuentas/ObtenerCuentas` - Listar todas
- `GET /Cuentas/ObtenerCuentasClientes` - Solo clientes
- `PUT /Cuentas/ActualizarCuenta/{id}` - Editar
- `DELETE /Cuentas/EliminarCuenta/{id}` - Eliminar

### Cuentas Bancarias
- `POST /CuentasBancarias/RegistrarCuentaBancaria?idCuenta={id}`
- `GET /CuentasBancarias/ObtenerCuentasBancarias/{idCuenta}`
- `GET /CuentasBancarias/ObtenerTodasLasCuentasBancarias`
- `PUT /CuentasBancarias/ActualizarCuentaBancaria/{id}`
- `DELETE /CuentasBancarias/{id}`

### Beneficiarios
- `POST /Beneficiarios/RegistrarBeneficiario`
- `GET /Beneficiarios/ObtenerBeneficiariosPorCliente/{id}`
- `GET /Beneficiarios/ObtenerTodosLosBeneficiarios`
- `PUT /Beneficiarios/ConfirmarBeneficiario/{id}`
- `PUT /Beneficiarios/RechazarBeneficiario/{id}`
- `DELETE /Beneficiarios/EliminarBeneficiario/{id}`

### Transferencias
- `POST /Transferencias/RegistrarTransferencia`
- `GET /Transferencias/ObtenerTransferenciasEnviadas/{id}`
- `GET /Transferencias/ObtenerTransferenciasRecibidas/{id}`
- `GET /Transferencias/ObtenerTodasLasTransferencias`
- `PUT /Transferencias/CancelarTransferencia/{ref}?idCliente={id}`
- `PUT /Transferencias/AprobarTransferencia/{ref}?idGestor={id}`
- `PUT /Transferencias/RechazarTransferencia/{ref}?idGestor={id}`

### Servicios y Contratos
- `GET /Servicios/ObtenerTodosLosServicios`
- `POST /Servicios/RegistrarServicio`
- `PUT /Servicios/ActualizarServicio/{id}`
- `DELETE /Servicios/EliminarServicio/{id}`
- `POST /ContratosServicios/AgregarContrato`
- `GET /ContratosServicios/ObtenerTodosLosContratos`

### Pagos de Servicios
- `POST /PagosServicios/RealizarPago`
- `GET /PagosServicios/ObtenerPagosPorCliente/{id}`
- `PUT /PagosServicios/CancelarPago/{id}?idCliente={id}`
- `POST /PagosServicios/ProcesarPagosProgramados` *(Automático)*

### Historial y Reportes
- `GET /Historial/ObtenerHistorialTransacciones`
- `GET /Historial/ObtenerHistorialPorCliente/{id}`
- `GET /Historial/ObtenerHistorialPorCuenta/{id}`

### Auditoría
- `GET /Acciones/ObtenerAcciones`
- `POST /Acciones/RegistrarAccion`

---

## 🔄 Flujos Principales

### 1. Login
```
Usuario → Login → ValidarCuenta (API) → 
  ✅ Éxito → Guardar en localStorage → Redirigir según rol
  ❌ Error → Incrementar intentos → Bloqueo temporal (3 intentos)
```

### 2. Crear Transferencia
```
Cliente → Modal Transferencia → Validaciones →
  POST /Transferencias/RegistrarTransferencia →
    Si monto > umbral → Estado: Pendiente (requiere aprobación)
    Si monto ≤ umbral → Estado: Programada/Exitosa
  → Auditoría → Actualizar historial
```

### 3. Pago de Servicio
```
Cliente → Seleccionar Contrato → Modal Pago →
  Monto autocompleta desde servicio.costo (CRC) →
  Seleccionar cuenta (CRC/USD) →
  POST /PagosServicios/RealizarPago →
    Backend convierte si cuenta es USD →
    Debita de la cuenta →
    Guarda pago en CRC
  → Auditoría → Actualizar historial
```

### 4. Aprobación de Beneficiario
```
Cliente → Crear Beneficiario → Estado: Pendiente →
Gestor/Admin → Ver beneficiarios pendientes →
  Confirmar → PUT /Beneficiarios/ConfirmarBeneficiario →
    Estado: Activo → Cliente puede usarlo
  O Rechazar → PUT /Beneficiarios/RechazarBeneficiario →
    Eliminado del sistema
```

---

## 💰 Sistema de Monedas

### Conversión Automática
- **Tasa fija:** ₡520 por $1 USD
- **Transferencias:**
  - Si origen y destino misma moneda → Sin conversión
  - Si diferente moneda → Conversión automática
- **Pagos de servicios:**
  - Servicios SIEMPRE en CRC (₡)
  - Si cuenta USD → Backend convierte para debitar

### Comisiones
- **Transferencias:** ₡500 CRC (fija)
- **Pagos de servicios:** ₡300 CRC (fija)

---

## 📱 Despliegue

### Frontend en Netlify

```bash
# 1. Build
ionic build

# 2. Subir carpeta www/ a Netlify (drag & drop)
# O conectar repositorio GitHub

# 3. Configurar en Netlify:
Build command: ionic build
Publish directory: www

# 4. Archivo _redirects ya incluido para SPA routing
```

### Backend en Azure/Render/Railway

```bash
# 1. Publicar
cd API/BancaEnLinea.API
dotnet publish -c Release -o ./publish

# 2. Configurar variables de entorno:
ConnectionStrings__DefaultConnection="<CONNECTION_STRING>"

# 3. Habilitar CORS para dominio Netlify
```

### Ajuste de producción
**Frontend** (`src/environments/environment.prod.ts`):
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://tu-backend.azurewebsites.net'
};
```

---

## 🧪 Testing

### Usuarios de prueba
```
Admin:
- Email: admin@banca.com
- Password: Admin123!

Gestor:
- Email: gestor@banca.com
- Password: Gestor123!

Cliente:
- Email: cliente@banca.com
- Password: Cliente123!
```

### Datos de prueba
- **Servicios:** ICE (₡30,000), AyA (₡15,000)
- **Cuentas bancarias:** Ahorros CRC, Corriente USD
- **Transferencias:** Varias programadas y ejecutadas

---

## 🐛 Solución de Problemas

### Frontend no conecta con backend
```bash
# Verificar que backend esté corriendo
curl http://localhost:5246/swagger

# Verificar CORS en backend (Program.cs)
```

### Error de base de datos
```bash
# Verificar connection string
# Ejecutar migrations
dotnet ef database update
```

### Build falla
```bash
# Limpiar cache
rm -rf node_modules package-lock.json
npm install
```

---

## 📚 Documentación Adicional

- [Informe completo del proyecto](./Informe_Proyecto_BancaEnLinea.md)
- [API Swagger](http://localhost:5246/swagger) (en desarrollo)

---

## 👨‍💻 Contribuir

1. Fork el proyecto
2. Crea una rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

---

## 📄 Licencia

Este proyecto es un trabajo académico para la Universidad.

---

## ✉️ Contacto

Proyecto desarrollado para el curso de Programación IV.

**Fecha:** Diciembre 2025
