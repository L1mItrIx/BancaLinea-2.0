# ?? Módulo de Transferencias - Documentación Completa

## ?? Descripción General

El módulo de **Transferencias** permite a los clientes realizar transferencias bancarias con las siguientes características:
- ? Transferencias inmediatas
- ? Transferencias programadas (hasta 90 días)
- ? Aprobación requerida para montos ? ?100,000
- ? Protección contra duplicados (IdempotencyKey)
- ? Cancelación hasta 24 horas antes
- ? Cálculo automático de comisiones y saldos

---

## ?? Reglas de Negocio Implementadas

### ?? Constantes del Sistema

| Constante | Valor | Descripción |
|-----------|-------|-------------|
| **COMISION_FIJA** | ?500 | Comisión por transferencia |
| **UMBRAL_APROBACION** | ?100,000 | Monto que requiere aprobación |
| **LIMITE_DIARIO_DEFAULT** | ?500,000 | Límite diario de transferencias |
| **DIAS_MAXIMOS_PROGRAMACION** | 90 días | Máximo tiempo de programación |
| **HORAS_MINIMAS_CANCELACION** | 24 horas | Tiempo mínimo para cancelar |

---

## ? Validaciones Implementadas

### 1?? **Antes de Crear Transferencia:**
- ? Datos de transferencia válidos
- ? IdempotencyKey único (evita duplicados)
- ? Fecha de ejecución ? 90 días
- ? Cuenta bancaria origen existe y está **Activa**
- ? Cliente está activo y tiene rol **Cliente**
- ? Saldo suficiente (Saldo ? Monto + Comisión)
- ? No supera límite diario (?500,000)
- ? Número de cuenta destino válido (12-20 dígitos)

### 2?? **Cálculos Automáticos:**
```
Comisión = ?500 (fija)
MontoTotal = Monto + Comisión
SaldoPosterior = SaldoAnterior - MontoTotal
```

### 3?? **Determinación de Estado Inicial:**
| Condición | Estado |
|-----------|--------|
| Monto ? ?100,000 | **Pendiente** (requiere aprobación) |
| Ejecución = Hoy && Monto < ?100,000 | **Pendiente** (se ejecuta inmediatamente) |
| Ejecución > Hoy && Monto < ?100,000 | **Programada** |

### 4?? **Cancelación:**
- ? Solo transferencias **Pendientes** o **Programadas**
- ? Debe faltar al menos **24 horas** para la ejecución
- ? Solo el cliente dueño puede cancelar

### 5?? **Aprobación/Rechazo:**
- ? Solo **Administradores** o **Gestores** pueden aprobar/rechazar
- ? Solo transferencias en estado **Pendiente**
- ? Al aprobar: se ejecuta inmediatamente y se debita el saldo

---

## ?? Estados de Transferencia

| Estado | Valor | Descripción |
|--------|-------|-------------|
| **Pendiente** | 0 | Esperando aprobación (monto ? ?100,000) |
| **Programada** | 1 | Agendada para ejecutar en el futuro |
| **Exitosa** | 2 | Ejecutada correctamente |
| **Fallida** | 3 | Error en la ejecución |
| **Cancelada** | 4 | Cancelada por el cliente |
| **Rechazada** | 5 | Rechazada por admin/gestor |

---

## ??? Estructura de la Base de Datos

### Tabla: `Transferencia`

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `Referencia` | INT (PK, Identity) | Identificador único |
| `IdempotencyKey` | NVARCHAR(100) UNIQUE | Clave para evitar duplicados |
| `IdCuentaBancariaOrigen` | INT (FK) | Cuenta bancaria que transfiere |
| `NumeroCuentaDestino` | BIGINT | Cuenta destino (12-20 dígitos, externa) |
| `Monto` | BIGINT | Monto base de la transferencia |
| `Comision` | BIGINT | Comisión (?500) |
| `MontoTotal` | BIGINT | Monto + Comisión |
| `SaldoAnterior` | BIGINT | Saldo antes de la transferencia |
| `SaldoPosterior` | BIGINT | Saldo después de la transferencia |
| `FechaCreacion` | DATETIME | Cuándo se creó la transferencia |
| `FechaEjecucion` | DATETIME | Cuándo se ejecutará |
| `Estado` | INT | Estado de la transferencia (0-5) |
| `Descripcion` | NVARCHAR(500) | Descripción/motivo rechazo |
| `IdAprobador` | INT (FK, nullable) | Quien aprobó/rechazó |
| `FechaAprobacion` | DATETIME (nullable) | Cuándo se aprobó/rechazó |

### Relaciones:
- **FK**: `IdCuentaBancariaOrigen` ? `CuentaBancaria.Id` (NO ACTION)
- **FK**: `IdAprobador` ? `Cuenta.Id` (NO ACTION)
- **UNIQUE**: `IdempotencyKey`

---

## ?? Endpoints de la API

### Base URL: `/Transferencias`

### 1?? Registrar Transferencia
```http
POST /Transferencias/RegistrarTransferencia
Content-Type: application/json

{
  "idempotencyKey": "UUID-UNICO-12345",
  "idCuentaBancariaOrigen": 1,
  "numeroCuentaDestino": 123456789012,
  "monto": 50000,
  "fechaEjecucion": "2024-12-25T10:00:00",
  "descripcion": "Pago de servicios"
}
```

**Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Transferencia realizada exitosamente",
  "referencia": 1
}
```

**Respuesta con Aprobación Requerida:**
```json
{
  "success": true,
  "message": "Transferencia pendiente de aprobación (supera ?100,000)",
  "referencia": 2
}
```

---

### 2?? Obtener Transferencias por Cuenta Bancaria
```http
GET /Transferencias/ObtenerTransferenciasPorCuenta/{idCuentaBancaria}
```

---

### 3?? Obtener Todas las Transferencias
```http
GET /Transferencias/ObtenerTodasLasTransferencias
```

---

### 4?? Obtener Transferencia por ID
```http
GET /Transferencias/ObtenerTransferenciaPorId/{referencia}
```

---

### 5?? Obtener Transferencias Pendientes de Aprobación
```http
GET /Transferencias/ObtenerTransferenciasPendientes
```

---

### 6?? Cancelar Transferencia
```http
PUT /Transferencias/CancelarTransferencia/{referencia}?idCliente=2
```

**Requisitos:**
- Solo el cliente dueño
- Debe faltar ? 24 horas para ejecución
- Estado = Pendiente o Programada

---

### 7?? Aprobar Transferencia
```http
PUT /Transferencias/AprobarTransferencia/{referencia}?idAprobador=1
```

**Requisitos:**
- Solo Admin o Gestor
- Estado = Pendiente
- Se ejecuta inmediatamente

---

### 8?? Rechazar Transferencia
```http
PUT /Transferencias/RechazarTransferencia/{referencia}?idAprobador=1
Content-Type: application/json

"Saldo insuficiente en cuenta destino"
```

---

## ?? Flujo de una Transferencia

### ?? **Escenario 1: Transferencia Inmediata (< ?100,000)**
```
1. Cliente solicita transferencia de ?50,000 para HOY
2. Sistema valida cuenta activa y saldo suficiente
3. Sistema calcula:
   - Comisión: ?500
   - MontoTotal: ?50,500
- SaldoPosterior: SaldoActual - ?50,500
4. Sistema valida límite diario
5. Estado inicial: Pendiente
6. ? Sistema ejecuta INMEDIATAMENTE:
   - Debita ?50,500 de cuenta origen
   - Estado ? Exitosa
7. ? Transferencia completada
```

### ?? **Escenario 2: Transferencia con Aprobación (? ?100,000)**
```
1. Cliente solicita transferencia de ?150,000
2. Sistema valida cuenta activa y saldo suficiente
3. Sistema calcula:
   - Comisión: ?500
   - MontoTotal: ?150,500
4. Estado inicial: Pendiente ??
5. ?? QUEDA EN ESPERA de aprobación
6. Admin/Gestor aprueba
7. ? Sistema ejecuta:
   - Debita ?150,500
   - Estado ? Exitosa
```

### ?? **Escenario 3: Transferencia Programada**
```
1. Cliente solicita transferencia de ?30,000 para dentro de 15 días
2. Sistema valida todo
3. Estado inicial: Programada ??
4. Sistema espera hasta la fecha programada
5. (En producción: Job scheduler ejecuta automáticamente)
6. ? Se ejecuta en la fecha indicada
```

### ?? **Escenario 4: Cancelación**
```
1. Cliente tiene transferencia Programada para dentro de 3 días
2. Cliente solicita cancelar (faltan 72 horas > 24 horas ?)
3. ? Sistema cancela
4. Estado ? Cancelada
```

---

## ?? Casos de Error

| Error | Causa |
|-------|-------|
| "Saldo insuficiente" | SaldoActual < MontoTotal |
| "Cuenta bancaria no está activa" | Estado != Activa |
| "Se ha excedido el límite diario" | Transferencias del día ? ?500,000 |
| "Solo se pueden cancelar con 24 horas de anticipación" | Faltan < 24 horas |
| "Esta transferencia ya fue procesada" | IdempotencyKey duplicado |

---

## ?? Archivos Creados

1. ? `BancaEnLinea.BC\Modelos\Transferencia.cs` - Modelo actualizado
2. ? `BancaEnLinea.DA\Entidades\TransferenciaDA.cs` - Entidad
3. ? `BancaEnLinea.BC\ReglasDeNegocio\ReglasDeTransferencia.cs` - Reglas
4. ? `BancaEnLinea.BW\Interfaces\DA\IGestionTransferenciaDA.cs` - Interfaz DA
5. ? `BancaEnLinea.BW\Interfaces\BW\IGestionTransferenciaBW.cs` - Interfaz BW
6. ? `BancaEnLinea.DA\Acciones\GestionTransferenciaDA.cs` - Implementación DA
7. ? `BancaEnLinea.BW\CU\GestionTransferenciaBW.cs` - Lógica de negocio
8. ? `BancaEnLinea.API\Controllers\TransferenciasController.cs` - Controller
9. ? Actualizado `BancaEnLineaContext.cs` y `Program.cs`
10. ? `SQL_TRANSFERENCIAS.sql` - Script SQL

---

## ? Estado del Proyecto

- ? **Arquitectura**: Completa (DA ? BW ? API)
- ? **Reglas de negocio**: Todas implementadas
- ? **Validaciones**: Completas
- ? **Cálculos automáticos**: Implementados
- ? **Protección duplicados**: IdempotencyKey
- ? **Aprobaciones**: Sistema completo
- ? **Cancelaciones**: Implementadas
- ?? **Base de datos**: Pendiente ejecutar SQL
- ?? **Job Scheduler**: Pendiente (para transferencias programadas)

---

## ?? Próximos Pasos

1. **Ejecutar SQL**: `SQL_TRANSFERENCIAS.sql`
2. **Probar endpoints** desde Postman/Frontend
3. **Implementar Job Scheduler** (Hangfire/Quartz) para transferencias programadas

---

**Fecha de Creación**: [Hoy]  
**Versión**: 1.0  
**Estado**: ? Listo para Usar
