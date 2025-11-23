# ?? Sistema Simple de Registro de Acciones

## ? Resumen

Sistema **SIMPLE** para registrar acciones en la base de datos y visualizarlas.

---

## ??? Base de Datos

### Tabla: `Accion`

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `Id` | INT | ID autoincremental |
| `Fecha` | DATETIME | Fecha y hora de la acción |
| `Descripcion` | NVARCHAR(500) | Descripción de la acción |
| `IdUsuario` | INT (NULL) | ID del usuario (opcional) |
| `NombreUsuario` | NVARCHAR(200) (NULL) | Nombre del usuario (opcional) |

### Crear la Tabla

```sql
-- Ejecutar el archivo: SQL_ACCIONES.sql
```

---

## ?? Endpoints (Solo 2)

### 1. **Registrar una Acción**

```
POST /Acciones/RegistrarAccion
```

**Body:**
```json
{
  "descripcion": "Se creó una cuenta bancaria en USD",
  "idUsuario": 5,
  "nombreUsuario": "Melissa Quijano"
}
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Acción registrada exitosamente"
}
```

---

### 2. **Obtener Todas las Acciones**

```
GET /Acciones/ObtenerAcciones
```

**Respuesta:**
```json
{
  "success": true,
  "data": [
    {
      "id": 4,
      "fecha": "2025-11-22T15:30:00",
      "descripcion": "Se realizó una transferencia de $100",
    "idUsuario": 5,
      "nombreUsuario": "Melissa Quijano"
    },
    {
      "id": 3,
      "fecha": "2025-11-22T15:28:00",
      "descripcion": "Se creó una cuenta bancaria en CRC",
    "idUsuario": 5,
      "nombreUsuario": "Melissa Quijano"
    },
    {
   "id": 2,
      "fecha": "2025-11-22T15:25:00",
      "descripcion": "Se creó una cuenta de usuario",
      "idUsuario": 5,
      "nombreUsuario": "Melissa Quijano"
    },
    {
      "id": 1,
    "fecha": "2025-11-22T15:00:00",
      "descripcion": "Sistema iniciado",
"idUsuario": null,
"nombreUsuario": null
    }
  ],
  "total": 4
}
```

---

## ?? Ejemplos de Uso

### Registrar Acciones Manualmente

```csharp
// En cualquier parte de tu código donde quieras registrar una acción:

// 1. Cuando se crea una cuenta
await RegistrarAccion("Se creó una cuenta de usuario", 5, "Melissa Quijano");

// 2. Cuando se crea una cuenta bancaria
await RegistrarAccion("Se creó una cuenta bancaria en USD", 5, "Melissa Quijano");

// 3. Cuando se realiza una transferencia
await RegistrarAccion($"Se realizó una transferencia de ${monto}", idUsuario, nombreUsuario);

// 4. Cuando se paga un servicio
await RegistrarAccion($"Se pagó el servicio de {nombreServicio}", idUsuario, nombreUsuario);

// 5. Cuando se cambia un parámetro
await RegistrarAccion("Se cambió la comisión de transferencias", idAdmin, nombreAdmin);
```

### Desde el Frontend

```javascript
// Servicio
class AccionesService {
  async registrarAccion(descripcion, idUsuario, nombreUsuario) {
    const response = await fetch('/Acciones/RegistrarAccion', {
    method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
   descripcion,
        idUsuario,
     nombreUsuario
      })
    });
    return await response.json();
  }

  async obtenerAcciones() {
    const response = await fetch('/Acciones/ObtenerAcciones');
    const data = await response.json();
    return data.data;
  }
}

// Uso
const service = new AccionesService();

// Registrar acción
await service.registrarAccion(
  'Se creó una cuenta bancaria',
  5,
  'Melissa Quijano'
);

// Obtener todas las acciones
const acciones = await service.obtenerAcciones();
console.log(`Total: ${acciones.length}`);
acciones.forEach(a => {
  console.log(`[${a.fecha}] ${a.descripcion} - ${a.nombreUsuario || 'Sistema'}`);
});
```

---

## ?? Ejemplo de Tabla en Frontend

```html
<table>
  <thead>
    <tr>
      <th>Fecha</th>
      <th>Acción</th>
      <th>Usuario</th>
    </tr>
  </thead>
  <tbody>
    <tr v-for="accion in acciones" :key="accion.id">
      <td>{{ formatDate(accion.fecha) }}</td>
      <td>{{ accion.descripcion }}</td>
      <td>{{ accion.nombreUsuario || 'Sistema' }}</td>
    </tr>
  </tbody>
</table>
```

---

## ?? Cómo Integrarlo en tus Controllers Existentes

### Ejemplo 1: Al crear una cuenta

```csharp
[HttpPost("RegistrarCuenta")]
public async Task<ActionResult> RegistrarCuenta([FromBody] Cuenta cuenta)
{
    var resultado = await gestionCuentaBW.registrarCuenta(cuenta);
    
    if (resultado)
    {
        // Registrar la acción
        await gestionAccionBW.registrarAccion(new Accion
        {
            Descripcion = $"Se creó una cuenta de usuario para {cuenta.Nombre} {cuenta.PrimerApellido}",
 IdUsuario = cuenta.Id,
        NombreUsuario = $"{cuenta.Nombre} {cuenta.PrimerApellido}"
     });
    }
    
    return Ok(resultado);
}
```

### Ejemplo 2: Al crear una cuenta bancaria

```csharp
[HttpPost("RegistrarCuentaBancaria")]
public async Task<ActionResult> RegistrarCuentaBancaria([FromBody] CuentaBancaria cuenta, [FromQuery] int idCuenta)
{
    var resultado = await gestionCuentaBancariaBW.registrarCuentaBancaria(cuenta, idCuenta);
    
    if (resultado)
    {
        // Obtener nombre del usuario
        var usuario = await gestionCuentaDA.obtenerCuentaPorId(idCuenta);
   
        // Registrar la acción
        await gestionAccionBW.registrarAccion(new Accion
        {
            Descripcion = $"Se creó una cuenta bancaria {cuenta.Tipo} en {cuenta.Moneda}",
     IdUsuario = idCuenta,
NombreUsuario = $"{usuario.Nombre} {usuario.PrimerApellido}"
     });
    }
    
    return Ok(resultado);
}
```

### Ejemplo 3: Al realizar una transferencia

```csharp
[HttpPost("RegistrarTransferencia")]
public async Task<ActionResult> RegistrarTransferencia([FromBody] TransferenciaRequest request)
{
    var (exito, mensaje, referencia) = await gestionTransferenciaBW.registrarTransferencia(request);
  
    if (exito)
    {
    // Obtener cuenta origen para saber el usuario
  var cuenta = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(request.IdCuentaBancariaOrigen);
        var usuario = await gestionCuentaDA.obtenerCuentaPorId(cuenta.IdCuenta);
      
        // Registrar la acción
        await gestionAccionBW.registrarAccion(new Accion
        {
            Descripcion = $"Se realizó una transferencia de {request.Monto} a la cuenta {request.NumeroCuentaDestino}",
            IdUsuario = usuario.Id,
            NombreUsuario = $"{usuario.Nombre} {usuario.PrimerApellido}"
 });
    }
    
    return Ok(new { success = exito, message = mensaje, referencia });
}
```

### Ejemplo 4: Al pagar un servicio

```csharp
[HttpPost("RealizarPago")]
public async Task<ActionResult> RealizarPago([FromBody] PagoServicioRequest request)
{
    var (exito, mensaje, idPago) = await gestionPagoServicioBW.realizarPago(request);
    
    if (exito)
    {
   // Obtener usuario
 var cuenta = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(request.IdCuentaBancariaOrigen);
   var usuario = await gestionCuentaDA.obtenerCuentaPorId(cuenta.IdCuenta);
        
      // Registrar la acción
        await gestionAccionBW.registrarAccion(new Accion
        {
Descripcion = $"Se pagó un servicio por {request.Monto}",
            IdUsuario = usuario.Id,
            NombreUsuario = $"{usuario.Nombre} {usuario.PrimerApellido}"
 });
    }
    
    return Ok(new { success = exito, message = mensaje, idPago });
}
```

---

## ?? Consultas SQL Útiles

```sql
-- Ver todas las acciones
SELECT * FROM Accion ORDER BY Fecha DESC;

-- Ver acciones de un usuario específico
SELECT * FROM Accion 
WHERE IdUsuario = 5 
ORDER BY Fecha DESC;

-- Ver acciones del día de hoy
SELECT * FROM Accion 
WHERE CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY Fecha DESC;

-- Contar acciones por usuario
SELECT 
    NombreUsuario,
    COUNT(*) AS TotalAcciones
FROM Accion
WHERE NombreUsuario IS NOT NULL
GROUP BY NombreUsuario
ORDER BY TotalAcciones DESC;

-- Acciones de la última hora
SELECT * FROM Accion
WHERE Fecha >= DATEADD(HOUR, -1, GETDATE())
ORDER BY Fecha DESC;
```

---

## ? Resumen

### Archivos Creados:
- ? `BancaEnLinea.BC\Modelos\Accion.cs`
- ? `BancaEnLinea.DA\Entidades\AccionDA.cs`
- ? `BancaEnLinea.BW\Interfaces\DA\IGestionAccionDA.cs`
- ? `BancaEnLinea.BW\Interfaces\BW\IGestionAccionBW.cs`
- ? `BancaEnLinea.DA\Acciones\GestionAccionDA.cs`
- ? `BancaEnLinea.BW\CU\GestionAccionBW.cs`
- ? `BancaEnLinea.API\Controllers\AccionesController.cs`
- ? `SQL_ACCIONES.sql`

### Endpoints:
- ? `POST /Acciones/RegistrarAccion` - Registrar una acción
- ? `GET /Acciones/ObtenerAcciones` - Obtener todas las acciones

### Tabla en BD:
- ? `Accion` (Id, Fecha, Descripcion, IdUsuario, NombreUsuario)

### Estado:
- **Build:** ? Exitoso
- **Script SQL:** ? Listo para ejecutar
- **Documentación:** ? Completa

---

## ?? Pasos para Usar:

1. **Ejecutar el script SQL:** `SQL_ACCIONES.sql`
2. **Registrar acciones manualmente:** Agregar código en tus controllers
3. **Ver acciones:** `GET /Acciones/ObtenerAcciones`

---

**Sistema simple, sin middleware, sin complicaciones. Solo 2 endpoints y una tabla.** ?
