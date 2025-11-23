# ?? Sistema de Historial de Transacciones

## ? Resumen de Implementación

Se ha creado un sistema completo para gestionar el historial de transacciones que combina **Transferencias** y **Pagos de Servicios** en una vista unificada.

---

## ?? Endpoints Creados

### 1. **Historial Completo** (Admin/Gestor)
```
GET /Historial/ObtenerHistorialTransacciones
```

**Respuesta:**
```json
{
  "success": true,
  "data": [
    {
      "id": 5,
      "fechaCreacion": "2025-11-22T12:16:25.333",
      "fechaEjecucion": "2025-11-22T00:00:00",
      "tipo": "Transferencia",
  "tipoIcono": "transfer",
      
      "idCliente": 5,
  "nombreCliente": "Melissa Quijano Munoz",
 
      "numeroCuenta": 901121144303,
      "monedaCuenta": 1,
      "monedaCuentaTexto": "USD",
      "simboloMoneda": "$",
      
      "monto": 15,
      "comision": 1,
      "montoTotal": 16,
  
      "estado": 2,
      "estadoTexto": "Exitosa",
      
  "descripcion": "Transferencia bancaria",
      "destino": "399292672803",
      "referencia": "TRF-5"
    },
    {
      "id": 1,
    "fechaCreacion": "2025-11-22T10:00:00",
      "fechaEjecucion": "2025-11-22T00:00:00",
      "tipo": "Pago de Servicio",
      "tipoIcono": "payment",
      
      "idCliente": 5,
      "nombreCliente": "Melissa Quijano Munoz",

      "numeroCuenta": 620826976010,
      "monedaCuenta": 0,
      "monedaCuentaTexto": "CRC",
   "simboloMoneda": "?",
      
      "monto": 30000,
      "comision": 300,
      "montoTotal": 30300,
    
      "estado": 2,
      "estadoTexto": "Exitosa",
      
  "descripcion": "Electricidad",
      "destino": "234653457568",
      "referencia": "PAG-1"
    }
  ],
  "total": 2,
  "totalTransferencias": 1,
  "totalPagos": 1
}
```

---

### 2. **Historial por Cliente**
```
GET /Historial/ObtenerHistorialPorCliente/{idCliente}
```

**Ejemplo:**
```
GET /Historial/ObtenerHistorialPorCliente/5
```

**Respuesta:**
```json
{
  "success": true,
  "data": [...],  // Mismo formato que arriba
  "total": 10
}
```

---

### 3. **Historial por Cuenta Bancaria**
```
GET /Historial/ObtenerHistorialPorCuenta/{idCuentaBancaria}
```

**Ejemplo:**
```
GET /Historial/ObtenerHistorialPorCuenta/2
```

**Respuesta:**
```json
{
  "success": true,
  "data": [...],
  "total": 5,
"numeroCuenta": 620826976010,
  "moneda": "CRC"
}
```

---

## ?? Estructura de Datos

### TransaccionHistorial

| Campo | Tipo | Descripción | Ejemplo |
|-------|------|-------------|---------|
| `id` | `int` | ID de la transacción | `5` |
| `fechaCreacion` | `DateTime` | Cuándo se creó | `2025-11-22T12:16:25` |
| `fechaEjecucion` | `DateTime` | Cuándo se ejecutó | `2025-11-22T00:00:00` |
| `tipo` | `string` | Tipo de transacción | `"Transferencia"` o `"Pago de Servicio"` |
| `tipoIcono` | `string` | Para iconos en frontend | `"transfer"` o `"payment"` |
| `idCliente` | `int` | ID del cliente | `5` |
| `nombreCliente` | `string` | Nombre completo | `"Melissa Quijano Munoz"` |
| `numeroCuenta` | `long` | Número de cuenta bancaria | `620826976010` |
| `monedaCuenta` | `Moneda` | Enum de moneda (0=CRC, 1=USD) | `0` |
| `monedaCuentaTexto` | `string` | Moneda en texto | `"CRC"` |
| `simboloMoneda` | `string` | Símbolo de moneda | `"?"` |
| `monto` | `long` | Monto de la transacción | `30000` |
| `comision` | `long` | Comisión cobrada | `300` |
| `montoTotal` | `long` | Total debitado | `30300` |
| `estado` | `EstadoTra` | Estado de la transacción | `2` |
| `estadoTexto` | `string` | Estado en texto | `"Exitosa"` |
| `descripcion` | `string` | Descripción | `"Electricidad"` |
| `destino` | `string` | Cuenta destino o contrato | `"234653457568"` |
| `referencia` | `string` | Referencia única | `"TRF-5"` o `"PAG-1"` |

---

## ??? Base de Datos

### Vista Creada: `vw_HistorialTransacciones`

Combina datos de `Transferencia` y `PagoServicio` en una sola vista.

```sql
-- Consultar la vista
SELECT * FROM vw_HistorialTransacciones
ORDER BY FechaCreacion DESC;
```

---

### Procedimientos Almacenados Creados

#### 1. **sp_ObtenerHistorialPorCliente**
```sql
EXEC sp_ObtenerHistorialPorCliente @IdCliente = 5;
```

#### 2. **sp_ObtenerHistorialPorCuentaBancaria**
```sql
EXEC sp_ObtenerHistorialPorCuentaBancaria @IdCuentaBancaria = 2;
```

#### 3. **sp_ObtenerHistorialPorFechas**
```sql
EXEC sp_ObtenerHistorialPorFechas 
    @FechaInicio = '2025-11-01', 
    @FechaFin = '2025-11-30',
    @IdCliente = 5;  -- Opcional
```

#### 4. **sp_ObtenerExtractoCuenta**
```sql
EXEC sp_ObtenerExtractoCuenta 
    @IdCuentaBancaria = 2, 
    @Mes = 11, 
    @Anio = 2025;
```

**Devuelve 3 conjuntos de resultados:**
1. Información de la cuenta
2. Transacciones del mes
3. Resumen (totales, comisiones, etc.)

#### 5. **sp_ObtenerEstadisticasCliente**
```sql
EXEC sp_ObtenerEstadisticasCliente @IdCliente = 5;
```

**Devuelve:**
- Total de transacciones
- Cantidad por tipo
- Total gastado
- Total en comisiones
- Transacciones por mes (últimos 12 meses)

---

## ?? Ejemplo de Uso en Frontend

### React/Vue/Angular

```javascript
// Servicio de Historial
class HistorialService {
  async obtenerHistorialCliente(idCliente) {
    const response = await fetch(
    `/Historial/ObtenerHistorialPorCliente/${idCliente}`
  );
    const data = await response.json();
    return data.data;
  }

  async obtenerHistorialCuenta(idCuentaBancaria) {
    const response = await fetch(
      `/Historial/ObtenerHistorialPorCuenta/${idCuentaBancaria}`
  );
    const data = await response.json();
    return data.data;
  }

  async obtenerHistorialCompleto() {
    const response = await fetch('/Historial/ObtenerHistorialTransacciones');
    const data = await response.json();
    return data.data;
  }
}

// Uso en componente
const historial = await historialService.obtenerHistorialCliente(5);

historial.forEach(transaccion => {
  console.log(`[${transaccion.referencia}] ${transaccion.descripcion}`);
  console.log(`Monto: ${transaccion.simboloMoneda}${transaccion.monto}`);
  console.log(`Estado: ${transaccion.estadoTexto}`);
  console.log(`Tipo: ${transaccion.tipo}`);
});
```

---

### Mostrar en Tabla

```javascript
function MostrarHistorial({ transacciones }) {
  return (
    <table>
      <thead>
        <tr>
          <th>Fecha</th>
       <th>Referencia</th>
          <th>Tipo</th>
          <th>Descripción</th>
          <th>Monto</th>
     <th>Comisión</th>
          <th>Total</th>
     <th>Estado</th>
        </tr>
      </thead>
      <tbody>
 {transacciones.map(t => (
      <tr key={t.referencia}>
          <td>{new Date(t.fechaCreacion).toLocaleDateString()}</td>
            <td>{t.referencia}</td>
            <td>
<span className={`icon-${t.tipoIcono}`}>
         {t.tipo}
          </span>
      </td>
 <td>{t.descripcion}</td>
        <td>{t.simboloMoneda}{t.monto.toLocaleString()}</td>
            <td>{t.simboloMoneda}{t.comision.toLocaleString()}</td>
      <td><strong>{t.simboloMoneda}{t.montoTotal.toLocaleString()}</strong></td>
            <td>
       <span className={`badge-${t.estado}`}>
    {t.estadoTexto}
       </span>
  </td>
   </tr>
 ))}
      </tbody>
    </table>
  );
}
```

---

## ?? Filtros y Búsqueda

Puedes filtrar y buscar en el frontend:

```javascript
// Filtrar por tipo
const soloTransferencias = historial.filter(t => t.tipo === 'Transferencia');
const soloPagos = historial.filter(t => t.tipo === 'Pago de Servicio');

// Filtrar por estado
const exitosas = historial.filter(t => t.estado === 2);
const pendientes = historial.filter(t => t.estado === 0);

// Filtrar por fecha
const esteMes = historial.filter(t => {
  const fecha = new Date(t.fechaCreacion);
  return fecha.getMonth() === new Date().getMonth();
});

// Buscar por descripción
const busqueda = 'electricidad';
const resultados = historial.filter(t => 
  t.descripcion.toLowerCase().includes(busqueda.toLowerCase())
);

// Calcular totales
const totalGastado = historial
  .filter(t => t.estado === 2)
  .reduce((sum, t) => sum + t.montoTotal, 0);

console.log(`Total gastado: ${totalGastado}`);
```

---

## ?? Estadísticas

### Obtener Estadísticas del Cliente

```javascript
async function obtenerEstadisticas(idCliente) {
  const historial = await historialService.obtenerHistorialCliente(idCliente);
  
  const stats = {
    total: historial.length,
    transferencias: historial.filter(t => t.tipo === 'Transferencia').length,
    pagos: historial.filter(t => t.tipo === 'Pago de Servicio').length,
    exitosas: historial.filter(t => t.estado === 2).length,
    pendientes: historial.filter(t => t.estado === 0).length,
 totalGastado: historial
      .filter(t => t.estado === 2)
      .reduce((sum, t) => sum + t.montoTotal, 0),
    totalComisiones: historial
      .filter(t => t.estado === 2)
      .reduce((sum, t) => sum + t.comision, 0)
  };
  
  return stats;
}

// Uso
const stats = await obtenerEstadisticas(5);
console.log(`Has realizado ${stats.total} transacciones`);
console.log(`Total gastado: ${stats.totalGastado}`);
console.log(`Comisiones pagadas: ${stats.totalComisiones}`);
```

---

## ?? Ejemplos de UI

### Dashboard con Resumen

```html
<div class="dashboard">
  <div class="card">
    <h3>Transacciones Totales</h3>
    <p class="number">{{ stats.total }}</p>
</div>
  
  <div class="card">
    <h3>Total Gastado</h3>
    <p class="number">?{{ stats.totalGastado.toLocaleString() }}</p>
  </div>
  
  <div class="card">
    <h3>Comisiones</h3>
    <p class="number">?{{ stats.totalComisiones.toLocaleString() }}</p>
  </div>
</div>

<div class="historial">
  <h2>Últimas Transacciones</h2>
  <div v-for="t in historial" :key="t.referencia" class="transaccion-card">
    <div class="icon" :class="`icon-${t.tipoIcono}`"></div>
    <div class="info">
      <p class="descripcion">{{ t.descripcion }}</p>
      <p class="fecha">{{ formatDate(t.fechaCreacion) }}</p>
    </div>
    <div class="monto">
   <p class="total">{{ t.simboloMoneda }}{{ t.montoTotal.toLocaleString() }}</p>
 <span class="badge" :class="`badge-${t.estado}`">{{ t.estadoTexto }}</span>
    </div>
  </div>
</div>
```

---

## ?? Configuración

### Archivos Creados

- ? **`BancaEnLinea.BC\Modelos\TransaccionHistorial.cs`** - Modelo de respuesta
- ? **`BancaEnLinea.API\Controllers\HistorialController.cs`** - Controlador de historial
- ? **`SQL_HISTORIAL_TRANSACCIONES.sql`** - Script de base de datos

### Métodos Agregados

- ? `IGestionPagoServicioBW.obtenerTodosPagos()`
- ? `IGestionPagoServicioDA.obtenerTodosPagos()`
- ? `GestionPagoServicioBW.obtenerTodosPagos()`
- ? `GestionPagoServicioDA.obtenerTodosPagos()`

---

## ?? Próximos Pasos Sugeridos

### 1. Extractos en PDF (TODO)

Crear `ExtractosController.cs`:
```csharp
[HttpGet("GenerarExtractoPDF/{idCuentaBancaria}")]
public async Task<IActionResult> GenerarExtractoPDF(
    int idCuentaBancaria, 
    int mes, 
    int anio)
{
    // Usar sp_ObtenerExtractoCuenta
    // Generar PDF con iTextSharp o QuestPDF
    // Retornar File(pdfBytes, "application/pdf")
}
```

### 2. Comprobantes (TODO)

Crear `ComprobantesController.cs`:
```csharp
[HttpGet("GenerarComprobantePDF")]
public async Task<IActionResult> GenerarComprobantePDF(
    int idTransaccion, 
    string tipo)
{
    // Generar comprobante de una transacción específica
}
```

### 3. Exportar a CSV/Excel (TODO)

```csharp
[HttpGet("ExportarCSV/{idCliente}")]
public async Task<IActionResult> ExportarCSV(int idCliente)
{
    // Generar archivo CSV del historial
}
```

---

## ? Estado Actual

- **Build:** ? Exitoso sin errores
- **Endpoints:** ? 3 endpoints funcionando
- **Base de Datos:** ? Vista y 5 procedimientos almacenados
- **Documentación:** ? Completa

---

**Fecha:** 2025-11-22  
**Versión:** 3.0  
**Autor:** Sistema de Banca en Línea
