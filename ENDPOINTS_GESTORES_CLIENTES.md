# ?? Endpoints para Gestores - Obtener Cuentas de Clientes

## ?? Propósito

Estos endpoints permiten a los **Gestores** obtener información de cuentas **SOLO de clientes**, sin poder ver o acceder a datos de otros Gestores o Administradores.

---

## ?? Control de Acceso

### ? Qué PUEDEN ver los gestores:
- Cuentas con `Rol = 2` (Cliente)
- Información básica de clientes
- Lista completa de clientes

### ? Qué NO PUEDEN ver los gestores:
- Cuentas con `Rol = 0` (Administrador)
- Cuentas con `Rol = 1` (Gestor)
- Contraseñas de clientes

---

## ?? Endpoints Disponibles

### 1. Obtener Todas las Cuentas de Clientes

**Endpoint:**
```
GET /Cuentas/ObtenerCuentasClientes
```

**Descripción:**  
Devuelve una lista de TODAS las cuentas que tienen rol de Cliente.

**Request:**  
No requiere parámetros ni body.

**Response (Éxito 200):**
```json
{
  "success": true,
  "data": [
    {
      "id": 5,
      "telefono": 88338833,
    "nombre": "Melissa",
      "primerApellido": "Quijano",
      "segundoApellido": "Munoz",
      "correo": "melqui@gmail.com",
      "rol": 2,
      "nombreCompleto": "Melissa Quijano Munoz"
    },
    {
      "id": 7,
      "telefono": 87654321,
      "nombre": "Juan",
      "primerApellido": "Pérez",
      "segundoApellido": "González",
   "correo": "juan.perez@email.com",
      "rol": 2,
      "nombreCompleto": "Juan Pérez González"
    }
  ],
  "total": 2
}
```

**Response (Error 500):**
```json
{
  "success": false,
  "message": "Error interno en el servidor: [detalle del error]"
}
```

---

### 2. Obtener Cuenta de Cliente por ID

**Endpoint:**
```
GET /Cuentas/ObtenerCuentaCliente/{id}
```

**Descripción:**  
Devuelve la información de un cliente específico por su ID. Valida que sea un cliente antes de devolver datos.

**Parámetros:**
- `id` (int, requerido) - ID de la cuenta del cliente

**Request:**  
```
GET /Cuentas/ObtenerCuentaCliente/5
```

**Response (Éxito 200):**
```json
{
  "success": true,
  "data": {
    "id": 5,
    "telefono": 88338833,
"nombre": "Melissa",
    "primerApellido": "Quijano",
    "segundoApellido": "Munoz",
    "correo": "melqui@gmail.com",
    "rol": 2,
    "nombreCompleto": "Melissa Quijano Munoz"
  }
}
```

**Response (No Encontrado 404):**
```json
{
  "success": false,
  "message": "Cliente no encontrado o no es un cliente válido"
}
```

**Response (Error 500):**
```json
{
  "success": false,
  "message": "Error interno en el servidor: [detalle del error]"
}
```

---

## ?? Seguridad

### Validaciones Implementadas

1. **Filtrado por Rol**
   - Solo devuelve cuentas con `Rol = RolCuenta.Cliente (2)`
   - Oculta automáticamente Administradores y Gestores

2. **Sin Contraseñas**
 - La contraseña NUNCA se devuelve en la respuesta
   - Solo información básica del cliente

3. **Validación de Existencia**
   - Si el ID no existe, devuelve 404
   - Si el ID es de un Gestor/Admin, devuelve 404

---

## ?? Casos de Uso

### Caso 1: Gestor ve lista de clientes
```javascript
// Frontend - Obtener lista de clientes para mostrar en tabla
fetch('http://localhost:5246/Cuentas/ObtenerCuentasClientes')
  .then(response => response.json())
  .then(data => {
    console.log(`Se encontraron ${data.total} clientes`);
    // Mostrar en tabla/grid
    mostrarClientes(data.data);
  });
```

### Caso 2: Gestor busca un cliente específico
```javascript
// Frontend - Ver detalles de un cliente
const clienteId = 5;
fetch(`http://localhost:5246/Cuentas/ObtenerCuentaCliente/${clienteId}`)
  .then(response => response.json())
  .then(data => {
    if (data.success) {
      console.log(`Cliente: ${data.data.nombreCompleto}`);
      mostrarDetallesCliente(data.data);
    } else {
   alert('Cliente no encontrado');
    }
  });
```

### Caso 3: Gestor intenta ver un Administrador
```javascript
// Frontend - Intenta acceder a cuenta de Admin (ID = 1, Rol = 0)
fetch('http://localhost:5246/Cuentas/ObtenerCuentaCliente/1')
  .then(response => response.json())
  .then(data => {
    // Respuesta: 404 - Cliente no encontrado
    // El sistema protege la cuenta del Admin
    console.log(data.message); // "Cliente no encontrado o no es un cliente válido"
  });
```

---

## ?? Ejemplos de Prueba

### Test 1: Obtener todos los clientes (Postman/curl)

```bash
GET http://localhost:5246/Cuentas/ObtenerCuentasClientes
```

**Resultado esperado:**
- Status: 200 OK
- Lista de clientes
- NO incluye Admins ni Gestores

---

### Test 2: Obtener cliente específico

```bash
GET http://localhost:5246/Cuentas/ObtenerCuentaCliente/5
```

**Resultado esperado:**
- Status: 200 OK
- Datos del cliente con ID 5
- Incluye nombre completo concatenado

---

### Test 3: Intentar obtener un Admin

```bash
GET http://localhost:5246/Cuentas/ObtenerCuentaCliente/1
# Asumiendo que ID=1 es un Admin (Rol=0)
```

**Resultado esperado:**
- Status: 404 Not Found
- Mensaje: "Cliente no encontrado o no es un cliente válido"

---

### Test 4: ID inexistente

```bash
GET http://localhost:5246/Cuentas/ObtenerCuentaCliente/9999
```

**Resultado esperado:**
- Status: 404 Not Found
- Mensaje: "Cliente no encontrado o no es un cliente válido"

---

## ?? Estructura de Datos Devueltos

### Objeto Cliente

```typescript
interface ClienteResponse {
  success: boolean;
  data: {
    id: number;
  telefono: number;
    nombre: string;
    primerApellido: string;
    segundoApellido: string;
    correo: string;
  rol: number;  // Siempre será 2 (Cliente)
    nombreCompleto: string;// Concatenación automática
  }
}
```

### Lista de Clientes

```typescript
interface ClientesListResponse {
  success: boolean;
  data: ClienteInfo[];
  total: number;
}

interface ClienteInfo {
  id: number;
  telefono: number;
  nombre: string;
  primerApellido: string;
  segundoApellido: string;
  correo: string;
  rol: number;  // Siempre 2
  nombreCompleto: string;
}
```

---

## ?? Logs del Sistema

Los endpoints generan logs útiles para debugging:

### Logs de ObtenerCuentasClientes:
```
?? Obteniendo cuentas de clientes...
? Se encontraron 5 clientes
```

### Logs de ObtenerCuentaCliente (éxito):
```
?? Buscando cliente con ID: 5
? Cliente encontrado: Melissa Quijano
```

### Logs de ObtenerCuentaCliente (no encontrado):
```
?? Buscando cliente con ID: 1
? No se encontró cliente con ID: 1
```

---

## ?? Integración Frontend

### Ejemplo React/Vue/Angular

```javascript
// Service para gestores
class ClienteService {
  async obtenerTodosLosClientes() {
    const response = await fetch('/Cuentas/ObtenerCuentasClientes');
    const data = await response.json();
    return data.data; // Array de clientes
  }

  async obtenerClientePorId(id) {
    const response = await fetch(`/Cuentas/ObtenerCuentaCliente/${id}`);
    if (response.status === 404) {
      throw new Error('Cliente no encontrado');
    }
    const data = await response.json();
    return data.data; // Objeto cliente
  }
}

// Uso en componente
const service = new ClienteService();

// Cargar lista de clientes
const clientes = await service.obtenerTodosLosClientes();
console.log(`Total de clientes: ${clientes.length}`);

// Ver detalle de un cliente
const cliente = await service.obtenerClientePorId(5);
console.log(`Nombre: ${cliente.nombreCompleto}`);
console.log(`Email: ${cliente.correo}`);
```

---

## ?? Limitaciones y Consideraciones

### 1. Sin Autenticación
- Actualmente NO valida que el usuario que llama sea realmente un Gestor
- **Recomendación:** Agregar autenticación JWT en el futuro

### 2. Sin Paginación
- Devuelve TODOS los clientes
- **Recomendación:** Agregar paginación si hay muchos clientes
```csharp
// Futuro: Agregar parámetros de paginación
[HttpGet("ObtenerCuentasClientes")]
public async Task<ActionResult> ObtenerCuentasClientes(
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 10)
{
    // Implementar Skip/Take para paginación
}
```

### 3. Sin Filtros
- No permite filtrar por nombre, correo, etc.
- **Recomendación:** Agregar búsqueda/filtros
```csharp
// Futuro: Agregar filtros
[HttpGet("ObtenerCuentasClientes")]
public async Task<ActionResult> ObtenerCuentasClientes(
    [FromQuery] string? nombre = null,
    [FromQuery] string? correo = null)
{
    // Implementar filtros
}
```

---

## ?? Próximas Mejoras Sugeridas

1. **Autenticación y Autorización**
 ```csharp
   [HttpGet("ObtenerCuentasClientes")]
   [Authorize(Roles = "Gestor,Administrador")]
 public async Task<ActionResult> ObtenerCuentasClientes()
   ```

2. **Paginación**
   - Agregar parámetros `page` y `pageSize`
 - Devolver `totalPages`, `currentPage`

3. **Búsqueda y Filtros**
   - Filtro por nombre
- Filtro por correo
   - Filtro por teléfono

4. **Ordenamiento**
   - Ordenar por nombre
   - Ordenar por fecha de registro
   - Ascendente/Descendente

5. **Cache**
   - Implementar cache para reducir consultas a BD
   - Invalidar cache al crear/editar clientes

---

## ?? Comparación con Endpoint Original

| Característica | ObtenerCuentas (Original) | ObtenerCuentasClientes (Nuevo) |
|----------------|---------------------------|--------------------------------|
| **Devuelve** | Todas las cuentas (Admin, Gestor, Cliente) | SOLO clientes |
| **Uso recomendado** | Solo para Administradores | Para Gestores |
| **Seguridad** | Expone todos los roles | Oculta Admin/Gestor |
| **Nombre completo** | No incluido | Incluido (concatenado) |
| **Total de registros** | No incluido | Incluido |

---

## ? Resumen

### Nuevos Endpoints Creados:
1. ? `GET /Cuentas/ObtenerCuentasClientes` - Lista de clientes
2. ? `GET /Cuentas/ObtenerCuentaCliente/{id}` - Cliente específico

### Seguridad Implementada:
- ? Filtrado automático por rol (solo clientes)
- ? Contraseñas nunca expuestas
- ? Validación de existencia y rol

### Logs Implementados:
- ? Logs informativos con emojis
- ? Logs de error detallados
- ? Conteo de resultados

---

**Fecha de implementación:** ${new Date().toLocaleDateString()}  
**Versión:** 2.2  
**Estado:** ? Implementado y funcional  
**Build:** ? Exitoso sin errores
