# ?? Sistema de Auditoría de Endpoints

## ? Resumen

Se ha implementado un **sistema de auditoría automático** que registra **TODOS** los endpoints que se usan en la API.

---

## ?? ¿Qué hace?

El sistema intercepta **todas las peticiones HTTP** y registra:
- ? Fecha y hora exacta
- ? Endpoint usado (ej: `/Transferencias/RegistrarTransferencia`)
- ? Método HTTP (GET, POST, PUT, DELETE)
- ? Código de estado (200, 404, 500, etc.)
- ? Tiempo de respuesta en milisegundos
- ? Dirección IP del cliente
- ? Parámetros de la petición

---

## ?? Endpoint Principal

### **GET** `/Auditoria/ObtenerMovimientos`

Obtiene **todos los movimientos** registrados en el sistema.

**Ejemplo de petición:**
```sh
GET http://localhost:5246/Auditoria/ObtenerMovimientos
```

**Respuesta:**
```json
{
  "success": true,
  "data": [
    {
      "id": 5,
      "fechaHora": "2025-11-22T14:30:15",
      "endpoint": "/Transferencias/RegistrarTransferencia",
      "metodoHttp": "POST",
      "usuarioId": null,
      "usuarioNombre": null,
"direccionIP": "::1",
      "codigoEstado": 200,
      "tiempoRespuestaMs": 125,
      "parametros": null
    },
    {
   "id": 4,
      "fechaHora": "2025-11-22T14:28:45",
   "endpoint": "/Historial/ObtenerHistorialPorCliente/5",
      "metodoHttp": "GET",
      "direccionIP": "::1",
      "codigoEstado": 200,
      "tiempoRespuestaMs": 89,
      "parametros": null
    },
    {
 "id": 3,
      "fechaHora": "2025-11-22T14:25:12",
      "endpoint": "/Cuentas/ValidarCuenta",
      "metodoHttp": "POST",
      "direccionIP": "::1",
      "codigoEstado": 200,
      "tiempoRespuestaMs": 203,
      "parametros": null
    }
  ],
"estadisticas": {
    "total": 45,
    "ultimaHora": 12,
    "totalExitosas": 40,
    "totalErrores": 5,
    "tiempoPromedioMs": 156.3,
    "endpointsMasUsados": [
      {
        "endpoint": "/Historial/ObtenerHistorialPorCliente/5",
        "cantidad": 8,
        "metodos": ["GET"]
      },
      {
        "endpoint": "/Transferencias/RegistrarTransferencia",
     "cantidad": 6,
   "metodos": ["POST"]
      },
      {
   "endpoint": "/Cuentas/ValidarCuenta",
        "cantidad": 5,
        "metodos": ["POST"]
      }
    ]
  }
}
```

---

## ?? Otros Endpoints Disponibles

### 1. **Movimientos por Endpoint Específico**
```
GET /Auditoria/ObtenerMovimientosPorEndpoint?endpoint=Transferencias
```

Filtra los logs que contengan "Transferencias" en el endpoint.

**Respuesta:**
```json
{
  "success": true,
  "data": [
    {
      "id": 10,
      "endpoint": "/Transferencias/RegistrarTransferencia",
      "metodoHttp": "POST",
  "codigoEstado": 200
    },
    {
      "id": 8,
      "endpoint": "/Transferencias/ObtenerTransferenciasPorCliente/5",
      "metodoHttp": "GET",
 "codigoEstado": 200
    }
  ],
  "total": 2,
  "endpoint": "Transferencias"
}
```

---

### 2. **Estadísticas por Método HTTP**
```
GET /Auditoria/ObtenerEstadisticasPorMetodo
```

Muestra estadísticas agrupadas por método (GET, POST, PUT, DELETE).

**Respuesta:**
```json
{
  "success": true,
  "data": [
    {
      "metodo": "GET",
      "cantidad": 25,
      "exitosas": 24,
      "errores": 1,
      "tiempoPromedioMs": 98.5
    },
    {
      "metodo": "POST",
  "cantidad": 15,
      "exitosas": 13,
      "errores": 2,
   "tiempoPromedioMs": 215.3
 },
 {
      "metodo": "PUT",
      "cantidad": 3,
      "exitosas": 3,
      "errores": 0,
      "tiempoPromedioMs": 145.0
    },
    {
      "metodo": "DELETE",
      "cantidad": 2,
      "exitosas": 0,
      "errores": 2,
      "tiempoPromedioMs": 67.5
    }
  ]
}
```

---

### 3. **Movimientos Recientes**
```
GET /Auditoria/ObtenerMovimientosRecientes?cantidad=20
```

Obtiene los últimos N movimientos (por defecto 50).

---

### 4. **Limpiar Logs**
```
DELETE /Auditoria/LimpiarMovimientos
```

Elimina todos los registros de actividad.

**Respuesta:**
```json
{
  "success": true,
  "message": "Se eliminaron 45 registros exitosamente"
}
```

---

## ??? Logs en Consola

Cuando la API está corriendo, verás logs en tiempo real en la consola:

```
?? API iniciada con sistema de auditoría activo
?? Para ver los movimientos: GET /Auditoria/ObtenerMovimientos

?? [14:30:15] POST /Transferencias/RegistrarTransferencia ? 200 (125ms)
?? [14:30:18] GET /Historial/ObtenerHistorialPorCliente/5 ? 200 (89ms)
?? [14:30:22] POST /Cuentas/ValidarCuenta ? 200 (203ms)
? [14:30:25] GET /Cuentas/ObtenerCuenta/999 ? ERROR: Cuenta no encontrada
?? [14:30:30] GET /Auditoria/ObtenerMovimientos ? 200 (45ms)
```

---

## ?? Ejemplo de Uso en Frontend

### Dashboard de Auditoría

```javascript
// Servicio de Auditoría
class AuditoriaService {
  async obtenerMovimientos() {
    const response = await fetch('/Auditoria/ObtenerMovimientos');
    const data = await response.json();
    return data;
  }

async obtenerEstadisticas() {
    const response = await fetch('/Auditoria/ObtenerEstadisticasPorMetodo');
  const data = await response.json();
    return data.data;
  }

  async limpiarLogs() {
    const response = await fetch('/Auditoria/LimpiarMovimientos', {
      method: 'DELETE'
    });
    return await response.json();
  }
}

// Uso en componente
const auditoriaService = new AuditoriaService();

// Obtener movimientos
const result = await auditoriaService.obtenerMovimientos();
console.log(`Total de movimientos: ${result.estadisticas.total}`);
console.log(`Exitosas: ${result.estadisticas.totalExitosas}`);
console.log(`Errores: ${result.estadisticas.totalErrores}`);
console.log(`Tiempo promedio: ${result.estadisticas.tiempoPromedioMs}ms`);

// Mostrar endpoints más usados
result.estadisticas.endpointsMasUsados.forEach(e => {
  console.log(`${e.endpoint}: ${e.cantidad} veces`);
});

// Mostrar logs
result.data.forEach(log => {
  console.log(`[${log.fechaHora}] ${log.metodoHttp} ${log.endpoint} ? ${log.codigoEstado}`);
});
```

---

### Tabla de Movimientos

```html
<table>
  <thead>
    <tr>
      <th>Fecha/Hora</th>
      <th>Método</th>
      <th>Endpoint</th>
      <th>Estado</th>
      <th>Tiempo</th>
      <th>IP</th>
    </tr>
  </thead>
  <tbody>
    <tr v-for="log in movimientos" :key="log.id">
      <td>{{ formatDate(log.fechaHora) }}</td>
      <td>
        <span :class="`badge-${log.metodoHttp.toLowerCase()}`">
        {{ log.metodoHttp }}
        </span>
   </td>
    <td>{{ log.endpoint }}</td>
    <td>
        <span :class="getStatusClass(log.codigoEstado)">
      {{ log.codigoEstado }}
   </span>
  </td>
      <td>{{ log.tiempoRespuestaMs }}ms</td>
      <td>{{ log.direccionIP }}</td>
    </tr>
  </tbody>
</table>

<style>
.badge-get { background: #28a745; color: white; }
.badge-post { background: #007bff; color: white; }
.badge-put { background: #ffc107; color: black; }
.badge-delete { background: #dc3545; color: white; }

.status-200 { color: #28a745; }
.status-400 { color: #ffc107; }
.status-500 { color: #dc3545; }
</style>
```

---

### Gráfica de Estadísticas

```javascript
// Con Chart.js
const stats = await auditoriaService.obtenerEstadisticas();

const chart = new Chart(ctx, {
  type: 'bar',
  data: {
labels: stats.map(s => s.metodo),
    datasets: [
      {
      label: 'Exitosas',
        data: stats.map(s => s.exitosas),
        backgroundColor: '#28a745'
   },
      {
        label: 'Errores',
  data: stats.map(s => s.errores),
        backgroundColor: '#dc3545'
      }
  ]
  }
});
```

---

## ?? Casos de Uso

### 1. **Monitoreo en Tiempo Real**
```javascript
// Actualizar cada 5 segundos
setInterval(async () => {
  const result = await auditoriaService.obtenerMovimientos();
  actualizarDashboard(result);
}, 5000);
```

### 2. **Detectar Errores**
```javascript
const result = await auditoriaService.obtenerMovimientos();
const errores = result.data.filter(l => l.codigoEstado >= 400);

if (errores.length > 0) {
  console.warn(`?? Se encontraron ${errores.length} errores`);
  errores.forEach(e => {
    console.error(`${e.endpoint} ? ${e.codigoEstado}`);
  });
}
```

### 3. **Análisis de Rendimiento**
```javascript
const result = await auditoriaService.obtenerMovimientos();

// Endpoints más lentos
const masLentos = result.data
  .sort((a, b) => b.tiempoRespuestaMs - a.tiempoRespuestaMs)
  .slice(0, 10);

console.log('Endpoints más lentos:');
masLentos.forEach(l => {
  console.log(`${l.endpoint}: ${l.tiempoRespuestaMs}ms`);
});
```

### 4. **Auditoría de Seguridad**
```javascript
// Ver qué IPs están accediendo
const result = await auditoriaService.obtenerMovimientos();

const porIP = result.data.reduce((acc, log) => {
  acc[log.direccionIP] = (acc[log.direccionIP] || 0) + 1;
return acc;
}, {});

console.log('Accesos por IP:');
Object.entries(porIP).forEach(([ip, cantidad]) => {
  console.log(`${ip}: ${cantidad} peticiones`);
});
```

---

## ?? Características del Sistema

### ? Ventajas

1. **Automático** - No requiere código en cada endpoint
2. **Sin Base de Datos** - Los logs están en memoria (rápido)
3. **Transparente** - No afecta el funcionamiento de la API
4. **En Tiempo Real** - Los logs se registran al instante
5. **Estadísticas Integradas** - Calcula automáticamente métricas

### ?? Limitaciones

1. **Memoria Volátil** - Los logs se pierden al reiniciar el servidor
2. **Sin Persistencia** - No se guardan en base de datos
3. **Límite de Memoria** - Muchos logs pueden consumir RAM

---

## ?? Mejoras Futuras (Opcional)

### 1. **Persistir en Base de Datos**
```csharp
// Guardar en SQL Server en lugar de memoria
await context.EndpointLogs.AddAsync(log);
await context.SaveChangesAsync();
```

### 2. **Agregar Usuario Autenticado**
```csharp
log.UsuarioId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
log.UsuarioNombre = context.User.Identity?.Name;
```

### 3. **Filtrar Endpoints**
```csharp
// No registrar Swagger o Health Checks
if (requestPath.StartsWith("/swagger") || requestPath.StartsWith("/health"))
return;
```

### 4. **Límite de Logs**
```csharp
// Mantener solo los últimos 1000 registros
if (_logs.Count > 1000)
    _logs.RemoveRange(0, _logs.Count - 1000);
```

---

## ?? Ejemplo Completo de Dashboard

```html
<!DOCTYPE html>
<html>
<head>
    <title>Dashboard de Auditoría</title>
    <style>
        body {
            font-family: Arial, sans-serif;
 padding: 20px;
            background: #f5f5f5;
        }
        .stats {
display: grid;
      grid-template-columns: repeat(4, 1fr);
 gap: 20px;
            margin-bottom: 30px;
}
        .stat-card {
            background: white;
        padding: 20px;
     border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .stat-card h3 {
            margin: 0 0 10px 0;
     color: #666;
font-size: 14px;
        }
        .stat-card .number {
         font-size: 32px;
          font-weight: bold;
    color: #333;
        }
    table {
     width: 100%;
            background: white;
            border-radius: 8px;
          overflow: hidden;
     }
        th, td {
            padding: 12px;
       text-align: left;
            border-bottom: 1px solid #eee;
        }
        th {
          background: #f8f9fa;
            font-weight: 600;
        }
    </style>
</head>
<body>
    <h1>?? Dashboard de Auditoría</h1>
    
    <div class="stats">
        <div class="stat-card">
            <h3>Total Movimientos</h3>
        <div class="number" id="total">0</div>
        </div>
  <div class="stat-card">
   <h3>Exitosas</h3>
  <div class="number" id="exitosas" style="color: #28a745;">0</div>
        </div>
    <div class="stat-card">
            <h3>Errores</h3>
      <div class="number" id="errores" style="color: #dc3545;">0</div>
        </div>
    <div class="stat-card">
          <h3>Tiempo Promedio</h3>
          <div class="number" id="promedio">0<span style="font-size: 16px;">ms</span></div>
        </div>
    </div>

    <table id="logs-table">
        <thead>
            <tr>
 <th>Hora</th>
          <th>Método</th>
         <th>Endpoint</th>
       <th>Estado</th>
  <th>Tiempo</th>
        </tr>
</thead>
        <tbody id="logs-body">
 </tbody>
    </table>

    <script>
      async function cargarMovimientos() {
            const response = await fetch('http://localhost:5246/Auditoria/ObtenerMovimientos');
     const result = await response.json();
         
      // Actualizar estadísticas
 document.getElementById('total').textContent = result.estadisticas.total;
      document.getElementById('exitosas').textContent = result.estadisticas.totalExitosas;
          document.getElementById('errores').textContent = result.estadisticas.totalErrores;
  document.getElementById('promedio').innerHTML = 
                Math.round(result.estadisticas.tiempoPromedioMs) + '<span style="font-size: 16px;">ms</span>';
         
     // Actualizar tabla
     const tbody = document.getElementById('logs-body');
            tbody.innerHTML = result.data.slice(0, 20).map(log => `
      <tr>
         <td>${new Date(log.fechaHora).toLocaleTimeString()}</td>
           <td><span style="background: ${getMethodColor(log.metodoHttp)}; color: white; padding: 4px 8px; border-radius: 4px;">${log.metodoHttp}</span></td>
 <td>${log.endpoint}</td>
  <td style="color: ${getStatusColor(log.codigoEstado)}">${log.codigoEstado}</td>
        <td>${log.tiempoRespuestaMs}ms</td>
            </tr>
 `).join('');
        }

        function getMethodColor(method) {
            const colors = {
        'GET': '#28a745',
   'POST': '#007bff',
         'PUT': '#ffc107',
 'DELETE': '#dc3545'
            };
  return colors[method] || '#6c757d';
    }

    function getStatusColor(status) {
     if (status >= 200 && status < 300) return '#28a745';
 if (status >= 400 && status < 500) return '#ffc107';
          if (status >= 500) return '#dc3545';
  return '#6c757d';
        }

     // Cargar al inicio
        cargarMovimientos();

        // Actualizar cada 5 segundos
        setInterval(cargarMovimientos, 5000);
    </script>
</body>
</html>
```

---

## ? Resumen

### Archivos Creados:
- ? `BancaEnLinea.BC\Modelos\EndpointLog.cs`
- ? `BancaEnLinea.API\Middleware\EndpointLoggingMiddleware.cs`
- ? `BancaEnLinea.API\Controllers\AuditoriaController.cs`

### Archivos Modificados:
- ? `BancaEnLinea.API\Program.cs`

### Endpoints Disponibles:
- ? `GET /Auditoria/ObtenerMovimientos`
- ? `GET /Auditoria/ObtenerMovimientosPorEndpoint?endpoint={texto}`
- ? `GET /Auditoria/ObtenerEstadisticasPorMetodo`
- ? `GET /Auditoria/ObtenerMovimientosRecientes?cantidad={n}`
- ? `DELETE /Auditoria/LimpiarMovimientos`

### Estado:
- **Build:** ? Exitoso sin errores
- **Middleware:** ? Activo y registrando
- **Logs en Consola:** ? Funcionando
- **Endpoints:** ? Listos para usar

---

**¡El sistema de auditoría está completamente funcional!** ????

Ahora cada vez que uses cualquier endpoint de tu API, quedará registrado automáticamente y podrás verlo en `/Auditoria/ObtenerMovimientos`.
