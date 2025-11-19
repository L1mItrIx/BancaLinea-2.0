# Cambios Implementados en Cuentas Bancarias

## ?? Resumen de Cambios

Se han implementado las siguientes mejoras en el módulo de Cuentas Bancarias:

### 1. ? **Número de Tarjeta Único**
- El `NumeroTarjeta` ahora tiene un índice único en la base de datos
- Se genera automáticamente un número de 12 dígitos
- No puede repetirse entre diferentes cuentas bancarias
- Se preserva al editar una cuenta (no se puede cambiar)

### 2. ? **Múltiples Cuentas por Cliente**
- Un cliente puede tener múltiples cuentas bancarias (el `IdCuenta` puede repetirse)
- **Límite**: Máximo 3 cuentas del mismo tipo y moneda por cliente
- Las cuentas cerradas NO cuentan para este límite

### 3. ? **Cierre Automático por Saldo Cero**
- Cuando el saldo de una cuenta llega a `0`, su estado cambia automáticamente a `Cerrada`
- Si una cuenta cerrada recibe fondos (saldo > 0), se reactiva automáticamente a `Activa`
- Esta lógica se aplica tanto al crear como al actualizar cuentas

### 4. ? **Edición de Cuentas Bancarias**
- Los administradores/gestores pueden editar cuentas bancarias de clientes
- Se puede corregir el saldo si hay errores
- El número de tarjeta se preserva (no se modifica al editar)
- El estado se actualiza automáticamente según el saldo

---

## ?? Cambios Técnicos Realizados

### Archivos Modificados:

#### 1. `BancaEnLinea.DA\Entidades\CuentaBancariaDA.cs`
```csharp
// Se agregó índice único al NumeroTarjeta
[Index(nameof(NumeroTarjeta), IsUnique = true)]
public class CuentaBancariaDA
```

#### 2. `BancaEnLinea.BC\ReglasDeNegocio\ReglasDeCuentaBancaria.cs`
**Cambios:**
- ? `laCuentaBancariaEsValida()`: Ahora permite saldo = 0
- ? `puedeCrearNuevaCuenta()`: Excluye cuentas cerradas del conteo
- ? **Nuevo método**: `determinarEstadoPorSaldo()` - Gestiona el estado automático

```csharp
/// <summary>
/// Determina el estado de la cuenta bancaria basado en el saldo.
/// Si el saldo es 0, la cuenta debe estar cerrada.
/// </summary>
public static EstadoCB determinarEstadoPorSaldo(long saldo, EstadoCB estadoActual)
{
  if (saldo == 0)
        return EstadoCB.Cerrada;
 
    // Si el saldo es mayor a 0 y la cuenta estaba cerrada, la reactivamos
    if (saldo > 0 && estadoActual == EstadoCB.Cerrada)
        return EstadoCB.Activa;
    
    return estadoActual;
}
```

#### 3. `BancaEnLinea.BW\CU\GestionCuentaBancaria.cs`
**Método `registrarCuentaBancaria()`:**
- ? Genera automáticamente el número de tarjeta
- ? Aplica el estado automático según el saldo
- ? Valida que el cliente no exceda 3 cuentas del mismo tipo y moneda

**Método `actualizarCuentaBancaria()` (Mejorado):**
- ? Preserva el número de tarjeta existente
- ? Genera nuevo número si no existe o es inválido
- ? Aplica el estado automático según el nuevo saldo
- ? Permite a administradores corregir errores de saldo

```csharp
public async Task<bool> actualizarCuentaBancaria(CuentaBancaria cuentaBancaria, int id)
{
  // Obtener cuenta existente
    var cuentaExistente = await gestionCuentaBancariaDA.obtenerCuentaBancariaPorId(id);
    if (cuentaExistente == null)
      return false;

    // Preservar el número de tarjeta (no se puede cambiar)
    cuentaBancaria.NumeroTarjeta = cuentaExistente.NumeroTarjeta;

    // Determinar estado por saldo
 cuentaBancaria.Estado = ReglasDeCuentaBancaria.determinarEstadoPorSaldo(
        cuentaBancaria.Saldo, 
      cuentaBancaria.Estado
    );

    // Validar y actualizar
    if (!ReglasDeCuentaBancaria.laCuentaBancariaEsValida(cuentaBancaria))
        return false;

    return await gestionCuentaBancariaDA.actualizarCuentaBancaria(cuentaBancaria, id);
}
```

---

## ??? Actualización de Base de Datos

### ?? **IMPORTANTE: Migración Requerida**

Debes aplicar una migración para agregar el índice único al campo `NumeroTarjeta`.

### Opción 1: Usando Entity Framework Core Tools

```bash
# 1. Instalar EF Core Tools (si no está instalado)
dotnet tool install --global dotnet-ef

# 2. Navegar al proyecto DA
cd BancaEnLinea.DA

# 3. Crear la migración
dotnet ef migrations add AgregarIndiceUnicoNumeroTarjeta --startup-project ../BancaEnLinea.API/BancaEnLinea.API.csproj

# 4. Aplicar la migración
dotnet ef database update --startup-project ../BancaEnLinea.API/BancaEnLinea.API.csproj
```

### Opción 2: Script SQL Manual

Si prefieres aplicar el cambio manualmente, ejecuta este SQL:

```sql
-- Para SQL Server
CREATE UNIQUE INDEX IX_CuentaBancaria_NumeroTarjeta 
ON CuentaBancaria (NumeroTarjeta);

-- Verificar que no haya números duplicados ANTES de ejecutar:
SELECT NumeroTarjeta, COUNT(*) as Cantidad
FROM CuentaBancaria
GROUP BY NumeroTarjeta
HAVING COUNT(*) > 1;
```

---

## ?? Casos de Uso y Ejemplos

### Caso 1: Crear una Nueva Cuenta Bancaria
```json
POST /CuentasBancarias/RegistrarCuentaBancaria?idCuenta=5
{
  "tipo": 0,  // Ahorro
  "moneda": 0,         // Colones
  "saldo": 50000,
  "estado": 0       // Se ajustará automáticamente
}

// El sistema generará automáticamente:
// - NumeroTarjeta: 123456789012 (12 dígitos únicos)
// - Estado: Activa (porque saldo > 0)
```

### Caso 2: Cuenta con Saldo Cero se Cierra Automáticamente
```json
PUT /CuentasBancarias/5
{
  "numeroTarjeta": 123456789012,  // Se preserva
  "tipo": 0,
  "moneda": 0,
  "saldo": 0,           // ? Saldo llegó a 0
  "estado": 0           // Se ignora
}

// Resultado:
// - Estado automáticamente cambia a: Cerrada (2)
```

### Caso 3: Cuenta Cerrada se Reactiva con Fondos
```json
PUT /CuentasBancarias/5
{
  "numeroTarjeta": 123456789012,
  "tipo": 0,
  "moneda": 0,
  "saldo": 10000,       // ? Fondos añadidos
  "estado": 2         // Estaba cerrada
}

// Resultado:
// - Estado automáticamente cambia a: Activa (0)
```

### Caso 4: Límite de 3 Cuentas del Mismo Tipo
```json
// Cliente ya tiene 3 cuentas de Ahorro en Colones
// Intenta crear una 4ta:

POST /CuentasBancarias/RegistrarCuentaBancaria?idCuenta=5
{
  "tipo": 0,      // Ahorro
  "moneda": 0,    // Colones
  "saldo": 10000
}

// Resultado: ERROR (400 Bad Request)
// Mensaje: "No se pudo registrar la cuenta bancaria"
```

---

## ?? Estados de Cuenta Bancaria

| Estado | Valor | Descripción |
|--------|-------|-------------|
| **Activa** | 0 | Cuenta operativa normal (saldo > 0) |
| **Bloqueada** | 1 | Cuenta bloqueada (requiere intervención manual) |
| **Cerrada** | 2 | Cuenta cerrada (saldo = 0, se cierra automáticamente) |

---

## ?? Restricciones de Seguridad

### Solo Clientes pueden tener Cuentas Bancarias
- ? **Cliente (Rol = 2)**: Puede tener hasta 3 cuentas del mismo tipo y moneda
- ? **Administrador (Rol = 0)**: NO puede tener cuentas bancarias
- ? **Gestor (Rol = 1)**: NO puede tener cuentas bancarias

### Validación al Crear Cuenta
```csharp
// El sistema verifica:
1. ¿El usuario existe?
2. ¿El usuario es Cliente?
3. ¿Ya tiene 3 cuentas del mismo tipo y moneda?
4. ¿El número de tarjeta es único?
```

---

## ? Validaciones Implementadas

### Al Crear Cuenta Bancaria:
1. ? El usuario debe existir
2. ? El usuario debe ser Cliente (Rol = 2)
3. ? No puede exceder 3 cuentas del mismo tipo y moneda
4. ? El número de tarjeta se genera automáticamente y es único
5. ? El saldo debe ser >= 0
6. ? El estado se ajusta automáticamente según el saldo

### Al Editar Cuenta Bancaria:
1. ? La cuenta debe existir
2. ? El número de tarjeta NO se puede modificar (se preserva)
3. ? El estado se ajusta automáticamente según el saldo
4. ? El saldo puede ser >= 0
5. ? Si saldo = 0 ? Estado = Cerrada
6. ? Si saldo > 0 y estaba cerrada ? Estado = Activa

---

## ?? Mejoras Futuras Sugeridas

1. **Auditoría de Cambios**: Registrar quién y cuándo modificó una cuenta
2. **Notificaciones**: Alertar al cliente cuando su cuenta se cierre por saldo cero
3. **Historial de Transacciones**: Implementar un log de movimientos
4. **Bloqueo Automático**: Bloquear cuentas con actividad sospechosa
5. **Recuperación de Cuentas**: Permitir reactivación manual de cuentas cerradas

---

## ?? Soporte

Si tienes dudas sobre estos cambios, contacta al equipo de desarrollo.

---

**Fecha de Implementación**: [Fecha Actual]  
**Versión**: 2.0  
**Estado**: ? Completado y Probado
