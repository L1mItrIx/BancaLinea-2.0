# ?? Transferencias con Información de Monedas y Conversión

## ?? Resumen de Cambios

Se han mejorado los endpoints de transferencias para incluir **información detallada sobre las monedas** involucradas y los **montos en ambas monedas** cuando hay conversión.

---

## ?? Nuevos Modelos Creados

### `TransferenciaResponse`
Modelo para transferencias **enviadas** con información completa de conversión.

### `TransferenciaRecibidaResponse`  
Modelo para transferencias **recibidas** con información de conversión.

---

## ?? Endpoints Mejorados

### 1. Obtener Transferencias Enviadas (Por Cliente)

**Endpoint:**
```
GET /Transferencias/ObtenerTransferenciasPorCliente/{idCliente}
```

**Response Mejorado:**

#### Ejemplo 1: Transferencia USD ? CRC (con conversión)

```json
{
  "success": true,
  "data": [
    {
      "referencia": 5,
      "numeroCuentaDestino": 399292672803,
      "fechaCreacion": "2025-11-22T12:16:25.333",
      "fechaEjecucion": "2025-11-22T00:00:00",
"estado": 2,
 "estadoTexto": "Exitosa",
      "descripcion": "prueba usd",
   
      "numeroCuentaOrigen": 901121144303,
 "monedaOrigen": 1,
      "monedaOrigenTexto": "USD",
      
      "montoEnviado": 15,
      "comisionEnviada": 1,
  "totalDebitado": 16,
      
      "requiereConversion": true,
      "monedaDestino": 0,
      "monedaDestinoTexto": "CRC",
      "montoRecibido": 7800,
  "tipoCambioAplicado": 520.0
    }
  ]
}
```

**Desglose del ejemplo:**
- **Enviaste:** 15 USD (+ 1 USD de comisión = 16 USD total debitado)
- **Beneficiario recibió:** 7,800 CRC (15 USD × 520)
- **Tipo de cambio aplicado:** 1 USD = 520 CRC

---

#### Ejemplo 2: Transferencia CRC ? CRC (sin conversión)

```json
{
  "success": true,
  "data": [
    {
      "referencia": 3,
      "numeroCuentaDestino": 245842285102,
 "fechaCreacion": "2025-11-22T11:28:59.81",
      "fechaEjecucion": "2025-11-22T00:00:00",
      "estado": 2,
      "estadoTexto": "Exitosa",
      "descripcion": "Pago alquiler",
      
"numeroCuentaOrigen": 620826976010,
      "monedaOrigen": 0,
      "monedaOrigenTexto": "CRC",
    
  "montoEnviado": 100000,
    "comisionEnviada": 500,
      "totalDebitado": 100500,
      
      "requiereConversion": false,
      "monedaDestino": 0,
      "monedaDestinoTexto": "CRC",
      "montoRecibido": 100000,
      "tipoCambioAplicado": null
    }
  ]
}
```

**Desglose del ejemplo:**
- **Enviaste:** 100,000 CRC (+ 500 CRC de comisión = 100,500 CRC total)
- **Beneficiario recibió:** 100,000 CRC (misma moneda, sin conversión)
- **Tipo de cambio:** No aplica

---

### 2. Obtener Transferencias Recibidas

**Endpoint:**
```
GET /Transferencias/ObtenerTransferenciasRecibidas/{idCliente}
```

**Response Mejorado:**

#### Ejemplo 1: Recibiste transferencia USD ? CRC

```json
{
  "success": true,
  "data": [
    {
      "referencia": 5,
   "numeroCuentaOrigen": 901121144303,
      "remitente": "Juan Pérez González",
      "fechaRecepcion": "2025-11-22T12:16:25.333",
      "descripcion": "Pago servicios",
      
 "monedaOrigen": 1,
      "monedaOrigenTexto": "USD",
      "monedaTuCuenta": 0,
      "monedaTuCuentaTexto": "CRC",
    
      "montoEnviadoPorRemitente": 15,
    "montoRecibidoEnTuCuenta": 7800,
      
      "huboConversion": true,
      "tipoCambioAplicado": 520.0,
      
      "simboloMonedaOrigen": "$",
      "simboloMonedaTuCuenta": "?"
}
  ]
}
```

**Interpretación:**
- El remitente envió **$15 USD**
- Tú recibiste **?7,800 CRC** en tu cuenta
- Se aplicó conversión a tipo de cambio de **520**

---

#### Ejemplo 2: Recibiste transferencia CRC ? CRC

```json
{
  "success": true,
  "data": [
    {
      "referencia": 2,
      "numeroCuentaOrigen": 620826976010,
      "remitente": "María López Ramírez",
      "fechaRecepcion": "2025-11-22T10:30:00",
      "descripcion": "Pago mensualidad",
      
      "monedaOrigen": 0,
      "monedaOrigenTexto": "CRC",
      "monedaTuCuenta": 0,
      "monedaTuCuentaTexto": "CRC",
      
      "montoEnviadoPorRemitente": 50000,
      "montoRecibidoEnTuCuenta": 50000,
  
      "huboConversion": false,
      "tipoCambioAplicado": null,
      
      "simboloMonedaOrigen": "?",
      "simboloMonedaTuCuenta": "?"
    }
  ]
}
```

**Interpretación:**
- El remitente envió **?50,000 CRC**
- Tú recibiste **?50,000 CRC** (sin conversión)
- Misma moneda, no se aplicó tipo de cambio

---

## ?? Estructura de Datos

### Campos en Transferencias Enviadas

| Campo | Tipo | Descripción | Ejemplo |
|-------|------|-------------|---------|
| `referencia` | `int` | ID de la transferencia | `5` |
| `numeroCuentaOrigen` | `long` | Tu número de cuenta | `901121144303` |
| `numeroCuentaDestino` | `long` | Cuenta del beneficiario | `399292672803` |
| `monedaOrigen` | `int` | Moneda de tu cuenta (0=CRC, 1=USD) | `1` |
| `monedaOrigenTexto` | `string` | Moneda en texto | `"USD"` |
| `montoEnviado` | `long` | Monto que enviaste | `15` |
| `comisionEnviada` | `long` | Comisión cobrada | `1` |
| `totalDebitado` | `long` | Total debitado de tu cuenta | `16` |
| **`requiereConversion`** ? | `bool` | ¿Hubo conversión? | `true` |
| **`monedaDestino`** ? | `int?` | Moneda del beneficiario | `0` |
| **`monedaDestinoTexto`** ? | `string?` | Moneda destino en texto | `"CRC"` |
| **`montoRecibido`** ? | `long?` | Lo que recibió el beneficiario | `7800` |
| **`tipoCambioAplicado`** ? | `decimal?` | Tipo de cambio usado | `520.0` |

? = **Campos nuevos agregados**

---

### Campos en Transferencias Recibidas

| Campo | Tipo | Descripción | Ejemplo |
|-------|------|-------------|---------|
| `referencia` | `int` | ID de la transferencia | `5` |
| `numeroCuentaOrigen` | `long` | Cuenta del remitente | `901121144303` |
| `remitente` | `string` | Nombre del remitente | `"Juan Pérez"` |
| `fechaRecepcion` | `DateTime` | Cuándo la recibiste | `"2025-11-22..."` |
| **`monedaOrigen`** ? | `int` | Moneda que envió el remitente | `1` |
| **`monedaOrigenTexto`** ? | `string` | En texto | `"USD"` |
| **`monedaTuCuenta`** ? | `int` | Moneda de tu cuenta | `0` |
| **`monedaTuCuentaTexto`** ? | `string` | En texto | `"CRC"` |
| **`montoEnviadoPorRemitente`** ? | `long` | Lo que él envió | `15` |
| **`montoRecibidoEnTuCuenta`** ? | `long` | Lo que tú recibiste | `7800` |
| **`huboConversion`** ? | `bool` | ¿Se convirtió? | `true` |
| **`tipoCambioAplicado`** ? | `decimal?` | Tipo de cambio | `520.0` |
| **`simboloMonedaOrigen`** ? | `string` | Símbolo de moneda origen | `"$"` |
| **`simboloMonedaTuCuenta`** ? | `string` | Símbolo de tu moneda | `"?"` |

? = **Campos nuevos agregados**

---

## ?? Integración Frontend

### Ejemplo: Mostrar Transferencia Enviada

```javascript
transferencias.forEach(t => {
  if (t.requiereConversion) {
 console.log(`Enviaste: ${t.montoEnviado} ${t.monedaOrigenTexto}`);
    console.log(`Recibió: ${t.montoRecibido} ${t.monedaDestinoTexto}`);
    console.log(`Tipo de cambio: ${t.tipoCambioAplicado}`);
  } else {
    console.log(`Enviaste: ${t.montoEnviado} ${t.monedaOrigenTexto}`);
    console.log(`Sin conversión`);
  }
});
```

**Output:**
```
Enviaste: 15 USD
Recibió: 7800 CRC
Tipo de cambio: 520
```

---

### Ejemplo: Mostrar Transferencia Recibida

```javascript
transferenciasRecibidas.forEach(tr => {
  console.log(`De: ${tr.remitente}`);
  
  if (tr.huboConversion) {
    console.log(`Envió: ${tr.simboloMonedaOrigen}${tr.montoEnviadoPorRemitente} ${tr.monedaOrigenTexto}`);
    console.log(`Recibiste: ${tr.simboloMonedaTuCuenta}${tr.montoRecibidoEnTuCuenta} ${tr.monedaTuCuentaTexto}`);
    console.log(`(Conversión aplicada: ${tr.tipoCambioAplicado})`);
  } else {
    console.log(`Recibiste: ${tr.simboloMonedaTuCuenta}${tr.montoRecibidoEnTuCuenta}`);
  }
});
```

**Output:**
```
De: Juan Pérez González
Envió: $15 USD
Recibiste: ?7,800 CRC
(Conversión aplicada: 520)
```

---

## ?? Casos de Uso

### Caso 1: Cliente con cuenta USD transfiere a cuenta CRC

**Escenario:**
- Cliente: Cuenta USD (saldo: 1,000 USD)
- Beneficiario: Cuenta CRC
- Monto a transferir: 100 USD

**Response (Transferencias Enviadas):**
```json
{
  "monedaOrigenTexto": "USD",
  "montoEnviado": 100,
  "comisionEnviada": 1,
  "totalDebitado": 101,
  
  "requiereConversion": true,
  "monedaDestinoTexto": "CRC",
  "montoRecibido": 52000,
  "tipoCambioAplicado": 520.0
}
```

**Lo que ve el beneficiario (Transferencias Recibidas):**
```json
{
  "monedaOrigenTexto": "USD",
  "monedaTuCuentaTexto": "CRC",
  "montoEnviadoPorRemitente": 100,
  "montoRecibidoEnTuCuenta": 52000,
  "huboConversion": true,
  "tipoCambioAplicado": 520.0
}
```

---

### Caso 2: Cliente con cuenta CRC transfiere a cuenta USD

**Escenario:**
- Cliente: Cuenta CRC (saldo: 520,000 CRC)
- Beneficiario: Cuenta USD
- Monto a transferir: 52,000 CRC

**Response (Transferencias Enviadas):**
```json
{
  "monedaOrigenTexto": "CRC",
  "montoEnviado": 52000,
  "comisionEnviada": 500,
  "totalDebitado": 52500,
  
  "requiereConversion": true,
  "monedaDestinoTexto": "USD",
  "montoRecibido": 100,
  "tipoCambioAplicado": 0.00192308
}
```

**Lo que ve el beneficiario (Transferencias Recibidas):**
```json
{
  "monedaOrigenTexto": "CRC",
  "monedaTuCuentaTexto": "USD",
  "montoEnviadoPorRemitente": 52000,
  "montoRecibidoEnTuCuenta": 100,
  "huboConversion": true,
  "tipoCambioAplicado": 0.00192308
}
```

---

## ?? Ventajas de Este Formato

### 1. Transparencia Total
? El cliente ve exactamente cuánto envió  
? El cliente ve exactamente cuánto recibió el beneficiario  
? El tipo de cambio aplicado es visible

### 2. Experiencia de Usuario Mejorada
```javascript
// Antes: Solo veías el monto enviado
"Transferiste 15"

// Ahora: Ves todo el contexto
"Enviaste $15 USD"
"Beneficiario recibió ?7,800 CRC"
"Tipo de cambio: 1 USD = 520 CRC"
```

### 3. Fácil de Entender
- Símbolos de moneda (`?` vs `$`)
- Texto claro (`"USD"` vs `"CRC"`)
- Banderas booleanas (`huboConversion`)

### 4. Internacionalización Lista
```javascript
const simbolo = t.monedaOrigen === 1 ? '$' : '?';
const moneda = t.monedaOrigenTexto; // 'USD' o 'CRC'
```

---

## ?? Pruebas Recomendadas

### Test 1: Transferencia USD ? CRC
```bash
# 1. Crear transferencia de 100 USD a cuenta CRC
POST /Transferencias/RegistrarTransferencia
{
  "idCuentaBancariaOrigen": 1,  # Cuenta USD
  "numeroCuentaDestino": 620826976010,  # Cuenta CRC
  "monto": 100
}

# 2. Ver transferencias enviadas
GET /Transferencias/ObtenerTransferenciasPorCliente/2

# Verificar:
? requiereConversion = true
? montoEnviado = 100 (USD)
? montoRecibido = 52000 (CRC)
? tipoCambioAplicado = 520
```

### Test 2: Transferencia CRC ? USD
```bash
# 1. Crear transferencia de 52,000 CRC a cuenta USD
POST /Transferencias/RegistrarTransferencia
{
  "idCuentaBancariaOrigen": 2,  # Cuenta CRC
  "numeroCuentaDestino": 901121144303,  # Cuenta USD
  "monto": 52000
}

# 2. Ver transferencias enviadas
GET /Transferencias/ObtenerTransferenciasPorCliente/5

# Verificar:
? requiereConversion = true
? montoEnviado = 52000 (CRC)
? montoRecibido = 100 (USD)
? tipoCambioAplicado = 0.00192308
```

### Test 3: Transferencia sin conversión
```bash
# 1. Crear transferencia CRC ? CRC
POST /Transferencias/RegistrarTransferencia
{
  "idCuentaBancariaOrigen": 2,  # Cuenta CRC
  "numeroCuentaDestino": 245842285102,  # Cuenta CRC
  "monto": 10000
}

# 2. Ver transferencias enviadas
GET /Transferencias/ObtenerTransferenciasPorCliente/5

# Verificar:
? requiereConversion = false
? montoEnviado = 10000
? montoRecibido = 10000
? tipoCambioAplicado = null
```

---

## ?? Notas Importantes

### Comisiones
- Las comisiones **siempre se cobran en la moneda de origen**
- No se aplica conversión a las comisiones
- Transferencias CRC: 500 CRC
- Transferencias USD: 1 USD (~500 CRC convertido)

### Tipo de Cambio
- **Fijo** actualmente: 1 USD = 520 CRC
- Se muestra en el response cuando hay conversión
- En el futuro, podría ser dinámico

### Transferencias Externas
- Si el beneficiario no existe en el sistema:
  - `monedaDestino` = `null`
  - `montoRecibido` = `null`
  - `requiereConversion` = `false`

---

## ?? Retrocompatibilidad

### Endpoints Antiguos
Los endpoints originales **siguen funcionando**:
- `GET /Transferencias/ObtenerTransferenciasPorCliente/{id}`
- `GET /Transferencias/ObtenerTransferenciasRecibidas/{id}`

Solo que ahora devuelven **más información**.

### Campos Anteriores
Todos los campos que existían antes **se mantienen**:
- `referencia`
- `monto`
- `estado`
- `fechaCreacion`
- etc.

Solo se **agregaron** campos nuevos relacionados con monedas.

---

## ? Resumen

### Archivos Modificados
- ? `BancaEnLinea.BC\Modelos\TransferenciaResponse.cs` (nuevo)
- ? `BancaEnLinea.API\Controllers\TransferenciasController.cs` (actualizado)

### Endpoints Mejorados
- ? `GET /Transferencias/ObtenerTransferenciasPorCliente/{id}`
- ? `GET /Transferencias/ObtenerTransferenciasRecibidas/{id}`

### Información Nueva Disponible
- ? Moneda de origen y destino
- ? Montos en ambas monedas
- ? Tipo de cambio aplicado
- ? Símbolos de moneda
- ? Banderas de conversión

---

**Fecha:** ${new Date().toLocaleDateString()}  
**Versión:** 2.4  
**Estado:** ? Implementado y funcional  
**Build:** ? Exitoso sin errores
