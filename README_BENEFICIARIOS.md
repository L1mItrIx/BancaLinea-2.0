# ?? Módulo de Beneficiarios - Documentación Completa (ACTUALIZADA)

## ?? Descripción General

El módulo de **Beneficiarios** permite a los **CLIENTES** gestionar una lista de cuentas bancarias externas de destino frecuentes para facilitar transferencias futuras. 

**?? IMPORTANTE**: Cada **CLIENTE** puede tener hasta **3 beneficiarios en total**, sin importar cuántas cuentas bancarias tenga.

---

## ?? Cambios Clave Implementados

| Cambio | Antes | Ahora |
|--------|-------|-------|
| **Relación** | IdCuentaOrigen ? CuentaBancaria | IdCuenta ? Cuenta (Cliente) |
| **Límite** | 10 beneficiarios por cuenta bancaria | **3 beneficiarios por cliente** |
| **Alias** | 3-50 caracteres | **3-30 caracteres** |
| **País** | Max 100 caracteres | **Max 50 caracteres** |
| **Número Cuenta Destino** | 10-20 dígitos | **12-20 dígitos** |
| **Cuenta Destino** | No especificado | **NO pertenece a nuestra BD (es externa)** |

---

## ?? Reglas de Negocio Implementadas

### ? Validaciones al Crear/Editar Beneficiario:

| Regla | Descripción |
|-------|-------------|
| **Alias** | Obligatorio, entre **3 y 30 caracteres**, único por cliente |
| **Banco** | Obligatorio, máximo 100 caracteres |
| **País** | Obligatorio, máximo 50 caracteres |
| **Número de Cuenta Destino** | Obligatorio, entre **12 y 20 dígitos**, único por cliente, **externo (no de nuestra BD)** |
| **Moneda** | Obligatorio (CRC=0 o USD=1) |
| **IdCuenta** | Obligatorio, debe existir en la tabla Cuenta (Cliente) |
| **Límite de Beneficiarios** | **Máximo 3 beneficiarios POR CLIENTE** |

### ?? Restricciones de Unicidad:
1. **Alias único**: No se puede repetir el mismo alias para el mismo cliente
2. **Número de cuenta destino único**: No se puede agregar la misma cuenta destino dos veces para el mismo cliente
3. **Límite de 3 beneficiarios**: Un cliente no puede tener más de 3 beneficiarios, **independientemente de cuántas cuentas bancarias tenga**

---

## ??? Estructura de la Base de Datos

### Tabla: `Beneficiario`

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `IdBeneficiario` | INT (PK, Identity) | Identificador único del beneficiario |
| `IdCuenta` | INT (FK) | **Referencia a Cuenta (Cliente)** |
| `Alias` | NVARCHAR(30) | Nombre descriptivo del beneficiario |
| `Banco` | NVARCHAR(100) | Nombre del banco destino |
| `Moneda` | INT | 0=CRC, 1=USD |
| `NumeroCuentaDestino` | BIGINT | Número de cuenta del beneficiario (12-20 dígitos, **externo**) |
| `Pais` | NVARCHAR(50) | País del banco destino |

### Relaciones:
- **FK**: `IdCuenta` ? `Cuenta.Id` (CASCADE DELETE)
- **NO hay relación con CuentaBancaria**

---

## ?? Endpoints de la API

### Base URL: `/Beneficiarios`

### 1?? Registrar Beneficiario
```http
POST /Beneficiarios/RegistrarBeneficiario
Content-Type: application/json

{
  "idCuenta": 2,
  "alias": "Mamá - BAC",
  "banco": "Banco BAC San José",
  "moneda": 0,
  "numeroCuentaDestino": 123456789012,
  "pais": "Costa Rica"
}
```

**Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Beneficiario registrado exitosamente"
}
```

**Respuesta de Error (400):**
```json
{
  "success": false,
  "message": "No se pudo registrar el beneficiario. Verifique los datos o el límite de 3 beneficiarios por cliente."
}
```

---

### 2?? Obtener Beneficiarios por Cliente
```http
GET /Beneficiarios/ObtenerBeneficiariosPorCliente/{idCuenta}
```

**Ejemplo:**
```http
GET /Beneficiarios/ObtenerBeneficiariosPorCliente/2
```

**Respuesta (200):**
```json
{
  "success": true,
  "data": [
    {
  "idBeneficiario": 1,
      "idCuenta": 2,
      "alias": "Mamá - BAC",
      "banco": "Banco BAC San José",
"moneda": 0,
      "numeroCuentaDestino": 123456789012,
      "pais": "Costa Rica",
      "cuenta": {
      "id": 2,
        "nombre": "Juan",
 "primerApellido": "Pérez",
        "segundoApellido": "González",
        "correo": "juan.cliente@email.com",
   "telefono": 87654321,
        "rol": 2
}
    }
  ]
}
```

---

### 3?? Obtener Todos los Beneficiarios
```http
GET /Beneficiarios/ObtenerTodosLosBeneficiarios
```

---

### 4?? Obtener Beneficiario por ID
```http
GET /Beneficiarios/ObtenerBeneficiarioPorId/{idBeneficiario}
```

---

### 5?? Actualizar Beneficiario
```http
PUT /Beneficiarios/ActualizarBeneficiario/{idBeneficiario}
Content-Type: application/json

{
  "alias": "Mamá - BAC Actualizado",
"banco": "Banco BAC San José",
  "moneda": 0,
  "numeroCuentaDestino": 123456789012,
  "pais": "Costa Rica"
}
```

**Nota:** El `idCuenta` se preserva automáticamente, no se puede cambiar.

---

### 6?? Eliminar Beneficiario
```http
DELETE /Beneficiarios/EliminarBeneficiario/{idBeneficiario}
```

---

## ?? Casos de Uso

### Caso 1: Cliente con 2 Cuentas Bancarias Agrega Beneficiario
```
1. Cliente tiene:
   - Cuenta bancaria #1 (Ahorros CRC)
   - Cuenta bancaria #2 (Corriente USD)
2. Cliente ya tiene 2 beneficiarios
3. Cliente intenta agregar el 3er beneficiario
4. ? Sistema permite (límite 3 por cliente, no por cuenta)
```

### Caso 2: Intento de Agregar 4to Beneficiario
```
1. Cliente ya tiene 3 beneficiarios
2. Intenta agregar un 4to
3. ? Sistema rechaza: "Límite de 3 beneficiarios por cliente alcanzado"
```

### Caso 3: Número de Cuenta Destino Externo
```
1. Cliente agrega beneficiario con cuenta 123456789012
2. Este número NO existe en nuestra base de datos
3. ? Sistema permite (es un número externo)
4. Solo valida que tenga 12-20 dígitos
```

---

## ?? Diferencias Clave con el Diseño Anterior

| Aspecto | Antes | Ahora |
|---------|-------|-------|
| **Relación principal** | CuentaBancaria | **Cuenta (Cliente)** |
| **Límite** | 10 por cuenta bancaria | **3 por cliente** |
| **Escenario** | Cliente con 3 cuentas bancarias = 30 beneficiarios posibles | **Cliente con 3 cuentas bancarias = 3 beneficiarios máximo** |
| **Cuenta destino** | Podía ser de nuestra BD | **Es externa (no de nuestra BD)** |

---

## ? Estado del Proyecto

- ? **Compilación**: Exitosa
- ? **Arquitectura**: Completa (DA ? BW ? API)
- ? **Reglas de negocio**: Actualizadas
- ? **Validaciones**: Completas (3-30 chars alias, 12-20 dígitos cuenta)
- ? **Relaciones**: IdCuenta ? Cuenta (no CuentaBancaria)
- ? **Documentación**: Actualizada
- ? **Scripts SQL**: Listos (SQL_BENEFICIARIOS.sql)
- ?? **Base de datos**: Pendiente ejecutar SQL

---

## ?? Ejemplo Completo

```json
// Cliente ID = 2 (Juan Pérez) tiene 3 cuentas bancarias
// Pero solo puede tener 3 beneficiarios en total

// Beneficiario 1
POST /Beneficiarios/RegistrarBeneficiario
{
  "idCuenta": 2,
  "alias": "Mamá",
  "banco": "BAC San José",
  "moneda": 0,
  "numeroCuentaDestino": 123456789012,
  "pais": "Costa Rica"
}

// Beneficiario 2
POST /Beneficiarios/RegistrarBeneficiario
{
  "idCuenta": 2,
  "alias": "Papá",
  "banco": "BCR",
  "moneda": 0,
  "numeroCuentaDestino": 987654321098,
  "pais": "Costa Rica"
}

// Beneficiario 3
POST /Beneficiarios/RegistrarBeneficiario
{
  "idCuenta": 2,
"alias": "Hermano USA",
  "banco": "Bank of America",
  "moneda": 1,
  "numeroCuentaDestino": 112233445566,
"pais": "Estados Unidos"
}

// Beneficiario 4 - RECHAZADO
POST /Beneficiarios/RegistrarBeneficiario
{
  "idCuenta": 2,
  "alias": "Tía",
  "banco": "Scotiabank",
  "moneda": 0,
  "numeroCuentaDestino": 998877665544,
"pais": "Costa Rica"
}
// ? Error: "Límite de 3 beneficiarios por cliente alcanzado"
```

---

**Fecha de Actualización**: [Fecha Actual]  
**Versión**: 2.0 (Actualizado)  
**Estado**: ? Listo para Usar
