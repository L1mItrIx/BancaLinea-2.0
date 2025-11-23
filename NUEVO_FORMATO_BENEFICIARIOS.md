# ?? Nuevo Formato para Registrar Beneficiarios

## ? Cambio Implementado

Se ha simplificado el proceso de registro de beneficiarios creando un **DTO (Data Transfer Object)** llamado `BeneficiarioRequest` que **solo requiere los datos necesarios** del cliente.

---

## ?? Nuevo Modelo: BeneficiarioRequest

### Ubicación
```
BancaEnLinea.BC\Modelos\BeneficiarioRequest.cs
```

### Propiedades

| Campo | Tipo | Descripción | Validación |
|-------|------|-------------|------------|
| `IdCuenta` | `int` | ID de la cuenta del cliente | > 0 |
| `Alias` | `string` | Alias del beneficiario | 3-30 caracteres |
| `Banco` | `string` | Nombre del banco | Max 100 caracteres |
| `Moneda` | `Moneda` | Moneda de la cuenta (CRC=0, USD=1) | Enum válido |
| `NumeroCuentaDestino` | `long` | Número de cuenta del beneficiario | 12-20 dígitos |
| `Pais` | `string` | País del beneficiario | Max 50 caracteres |

---

## ?? Nuevo Formato de Request

### ? ANTES (Modelo completo - NO usar más)

```json
{
  "idBeneficiario": 0,
  "idCuenta": 5,
  "alias": "Allyson",
  "banco": "Banco Nacional",
  "moneda": 1,
  "numeroCuentaDestino": 12423895289358,
  "pais": "Costa Rica",
  "estado": 0,
  "cuenta": {
    "id": 5,
    "telefono": 0,
    "nombre": "",
    "primerApellido": "",
    "segundoApellido": "",
    "correo": "",
    "contrasena": "",
    "rol": 0
  }
}
```

### ? AHORA (Modelo simplificado - usar este)

```json
{
  "idCuenta": 5,
  "alias": "Allyson",
  "banco": "Banco Nacional",
  "moneda": 1,
  "numeroCuentaDestino": 12423895289358,
  "pais": "Costa Rica"
}
```

---

## ?? Endpoint Actualizado

### POST `/Beneficiarios/RegistrarBeneficiario`

**Request Body:**
```json
{
  "idCuenta": 5,
  "alias": "Allyson",
  "banco": "Banco Nacional",
  "moneda": 1,
  "numeroCuentaDestino": 12423895289358,
  "pais": "Costa Rica"
}
```

**Response (Éxito):**
```json
{
  "success": true,
  "message": "Beneficiario registrado exitosamente"
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "No se pudo registrar el beneficiario. Verifique los datos o el límite de 3 beneficiarios por cliente."
}
```

---

## ?? Ejemplos Completos

### Ejemplo 1: Beneficiario en CRC

```json
{
  "idCuenta": 5,
  "alias": "Juan Perez",
  "banco": "Banco de Costa Rica",
  "moneda": 0,
  "numeroCuentaDestino": 123456789012,
  "pais": "Costa Rica"
}
```

### Ejemplo 2: Beneficiario en USD

```json
{
  "idCuenta": 5,
  "alias": "Maria Lopez USD",
  "banco": "Banco Nacional",
  "moneda": 1,
  "numeroCuentaDestino": 987654321098,
  "pais": "Costa Rica"
}
```

### Ejemplo 3: Beneficiario Internacional

```json
{
  "idCuenta": 5,
  "alias": "Carlos USA",
  "banco": "Bank of America",
  "moneda": 1,
  "numeroCuentaDestino": 555666777888,
  "pais": "Estados Unidos"
}
```

---

## ?? Ventajas del Nuevo Formato

### ? Más Simple
- Solo 6 campos necesarios
- No requiere datos de la cuenta completa
- Más fácil de entender y usar

### ? Más Seguro
- No se puede modificar el `IdBeneficiario` (se genera automáticamente)
- No se puede modificar el `Estado` (siempre empieza como Pendiente)
- No se envían datos sensibles innecesarios

### ? Más Eficiente
- JSON más pequeño
- Menos datos transferidos por la red
- Validaciones más claras

### ? Más Mantenible
- Cambios en el modelo `Beneficiarios` no afectan el API
- DTO independiente y bien definido
- Facilita pruebas automatizadas

---

## ?? Flujo Completo

1. **Cliente envía request simplificado**
   ```json
 {
     "idCuenta": 5,
     "alias": "Allyson",
   "banco": "Banco Nacional",
 "moneda": 1,
   "numeroCuentaDestino": 12423895289358,
     "pais": "Costa Rica"
   }
 ```

2. **API mapea a modelo completo**
   - Se agrega `Estado = Pendiente` automáticamente
   - Se validan todos los campos
   - Se generará `IdBeneficiario` en la base de datos

3. **Validaciones ejecutadas**
   - ? Datos válidos (longitudes, formatos)
   - ? Cliente existe
   - ? No excede límite de 3 beneficiarios
   - ? Alias único para el cliente
   - ? Número de cuenta único para el cliente

4. **Beneficiario creado**
   - Estado: Pendiente
   - Esperando aprobación de Admin/Gestor

---

## ?? Notas Importantes

### Estado Inicial
- **Siempre** se crea como `EstadoP.Pendiente = 0`
- No se puede especificar en el request
- Solo Admin/Gestor puede cambiar a Activo o Inactivo

### IdBeneficiario
- **Auto-generado** por la base de datos
- No se debe enviar en el request
- Se ignora si se envía

### Validaciones
Todas las validaciones se mantienen:
- Máximo 3 beneficiarios (Activos + Pendientes)
- Alias único por cliente
- Número de cuenta único por cliente
- Formatos y longitudes correctas

### Moneda
- `0` = CRC (Colones)
- `1` = USD (Dólares)

---

## ?? Pruebas Recomendadas

### Test 1: Registro Exitoso
```bash
POST http://localhost:5246/Beneficiarios/RegistrarBeneficiario
Content-Type: application/json

{
  "idCuenta": 5,
  "alias": "Allyson",
  "banco": "Banco Nacional",
  "moneda": 1,
  "numeroCuentaDestino": 12423895289358,
  "pais": "Costa Rica"
}
```

**Resultado esperado:** 
```json
{
  "success": true,
  "message": "Beneficiario registrado exitosamente"
}
```

### Test 2: Alias Duplicado
```bash
# Primero crear un beneficiario
POST .../RegistrarBeneficiario
{
  "idCuenta": 5,
  "alias": "Jose",
  ...
}

# Intentar crear otro con el mismo alias
POST .../RegistrarBeneficiario
{
  "idCuenta": 5,
  "alias": "Jose",  # ? Duplicado
  ...
}
```

**Resultado esperado:**
```json
{
  "success": false,
"message": "No se pudo registrar el beneficiario..."
}
```

**Log en consola:**
```
? ERROR: El alias 'Jose' ya existe para este cliente
```

### Test 3: Límite de Beneficiarios
```bash
# Crear 3 beneficiarios para el mismo cliente
# El cuarto debería fallar
```

**Log en consola:**
```
? ERROR: Límite de 3 beneficiarios alcanzado (Activos + Pendientes)
```

---

## ?? Debugging

Si tienes problemas registrando beneficiarios, revisa los logs en Visual Studio:

1. **View ? Output** (`Ctrl+Alt+O`)
2. Selecciona el proyecto API
3. Busca los emojis:
   - ?? = Intento de registro
   - ? = Campo válido
   - ? = Error específico
   - ?? = Error inesperado

### Ejemplo de logs:
```
?? Intentando registrar beneficiario: Allyson para cliente 5
? Beneficiario válido: Alias='Allyson', Cuenta=12423895289358
?? Cliente 5 tiene 1 beneficiarios en total
   - Activos: 0
   - Pendientes: 1
   - Inactivos: 0
? Todas las validaciones pasaron. Registrando beneficiario 'Allyson'...
? Beneficiario registrado exitosamente
```

---

## ?? Changelog

**Fecha:** ${new Date().toLocaleDateString()}  
**Versión:** 2.1  
**Cambio:** Implementación de `BeneficiarioRequest` DTO

### Archivos Modificados
- ? `BancaEnLinea.BC\Modelos\BeneficiarioRequest.cs` (nuevo)
- ? `BancaEnLinea.API\Controllers\BeneficiariosController.cs` (actualizado)

### Breaking Changes
- ? El endpoint ya **NO acepta** el modelo completo `Beneficiarios`
- ? Ahora **SOLO acepta** `BeneficiarioRequest`

### Migración
Si tienes código cliente existente, actualiza de:
```javascript
// ANTES
const beneficiario = {
  idBeneficiario: 0,
  idCuenta: 5,
  alias: "Allyson",
banco: "Banco Nacional",
  moneda: 1,
  numeroCuentaDestino: 12423895289358,
  pais: "Costa Rica",
  estado: 0,
  cuenta: { ... }  // ? Ya no necesario
};
```

A:
```javascript
// AHORA
const beneficiario = {
  idCuenta: 5,
  alias: "Allyson",
  banco: "Banco Nacional",
  moneda: 1,
  numeroCuentaDestino: 12423895289358,
  pais: "Costa Rica"
};
```

---

**Estado:** ? Implementado y funcional  
**Build:** ? Exitoso sin errores
