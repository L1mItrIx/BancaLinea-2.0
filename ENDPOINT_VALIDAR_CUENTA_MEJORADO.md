# ?? Endpoint de Validación de Cuenta Mejorado

## ?? Resumen de Cambios

Se ha mejorado el endpoint `POST /Cuentas/ValidarCuenta` para incluir información del rol del usuario y la ruta de redirección correspondiente, facilitando la navegación automática en el frontend.

---

## ?? Endpoint

### POST `/Cuentas/ValidarCuenta`

**Descripción:**  
Valida las credenciales de un usuario (correo y contraseña) y devuelve información completa del usuario incluyendo su rol y la ruta a la que debe ser redirigido.

---

## ?? Request

### Formato del Request

```json
{
  "correo": "usuario@example.com",
  "contrasena": "Password123!"
}
```

### Modelo: LoginRequest

```csharp
public class LoginRequest
{
    public string Correo { get; set; }
    public string Contrasena { get; set; }
}
```

---

## ?? Response

### Response Exitoso (200 OK)

#### Ejemplo 1: Login como Administrador

**Request:**
```json
{
  "correo": "admin@banco.com",
  "contrasena": "Admin123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Inicio de sesión exitoso",
  "data": {
    "id": 1,
 "telefono": 88887777,
    "nombre": "Admin",
    "primerApellido": "Sistema",
    "segundoApellido": "Banco",
  "correo": "admin@banco.com",
    "rol": 0,
    "rolNombre": "Administrador",
    "nombreCompleto": "Admin Sistema Banco",
    "rutaRedireccion": "/admin/dashboard"
  }
}
```

---

#### Ejemplo 2: Login como Gestor

**Request:**
```json
{
  "correo": "juan.gestor@banco.com",
  "contrasena": "Gestor123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Inicio de sesión exitoso",
  "data": {
    "id": 2,
    "telefono": 87654321,
    "nombre": "Juan",
  "primerApellido": "Pérez",
    "segundoApellido": "González",
    "correo": "juan.gestor@banco.com",
    "rol": 1,
    "rolNombre": "Gestor",
    "nombreCompleto": "Juan Pérez González",
    "rutaRedireccion": "/gestor/dashboard"
  }
}
```

---

#### Ejemplo 3: Login como Cliente

**Request:**
```json
{
  "correo": "melqui@gmail.com",
  "contrasena": "C@rene!@71"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Inicio de sesión exitoso",
  "data": {
    "id": 5,
    "telefono": 88338833,
    "nombre": "Melissa",
    "primerApellido": "Quijano",
 "segundoApellido": "Munoz",
 "correo": "melqui@gmail.com",
    "rol": 2,
    "rolNombre": "Cliente",
    "nombreCompleto": "Melissa Quijano Munoz",
    "rutaRedireccion": "/cliente/dashboard"
  }
}
```

---

### Response de Error (401 Unauthorized)

**Cuando las credenciales son incorrectas:**

```json
{
  "success": false,
  "message": "Correo o contraseña incorrectos"
}
```

---

### Response de Error (500 Internal Server Error)

**Cuando hay un error en el servidor:**

```json
{
  "success": false,
  "message": "Error interno: [detalle del error]"
}
```

---

## ?? Estructura de Datos de Respuesta

### Objeto Data

| Campo | Tipo | Descripción | Ejemplo |
|-------|------|-------------|---------|
| `id` | `int` | ID único del usuario | `5` |
| `telefono` | `long` | Teléfono del usuario | `88338833` |
| `nombre` | `string` | Nombre del usuario | `"Melissa"` |
| `primerApellido` | `string` | Primer apellido | `"Quijano"` |
| `segundoApellido` | `string` | Segundo apellido | `"Munoz"` |
| `correo` | `string` | Email del usuario | `"melqui@gmail.com"` |
| `rol` | `int` | Número del rol (0=Admin, 1=Gestor, 2=Cliente) | `2` |
| **`rolNombre`** ? | `string` | **Nombre legible del rol** | `"Cliente"` |
| **`nombreCompleto`** ? | `string` | **Nombre completo concatenado** | `"Melissa Quijano Munoz"` |
| **`rutaRedireccion`** ? | `string` | **Ruta para redirección automática** | `"/cliente/dashboard"` |

? = **Nuevos campos agregados**

---

## ?? Mapeo de Roles

| Rol (int) | RolNombre | Ruta de Redirección | Permisos |
|-----------|-----------|---------------------|----------|
| `0` | `"Administrador"` | `/admin/dashboard` | Acceso total al sistema |
| `1` | `"Gestor"` | `/gestor/dashboard` | Gestión de clientes, beneficiarios, transferencias |
| `2` | `"Cliente"` | `/cliente/dashboard` | Operaciones bancarias propias |

---

## ?? Integración Frontend

### Ejemplo JavaScript/TypeScript

```javascript
async function login(correo, contrasena) {
  try {
    const response = await fetch('http://localhost:5246/Cuentas/ValidarCuenta', {
      method: 'POST',
    headers: {
        'Content-Type': 'application/json'
      },
  body: JSON.stringify({ correo, contrasena })
    });

    const result = await response.json();

    if (result.success) {
      const { data } = result;
      
  // Guardar datos del usuario
      localStorage.setItem('userId', data.id);
      localStorage.setItem('userName', data.nombreCompleto);
  localStorage.setItem('userRole', data.rolNombre);
      localStorage.setItem('userEmail', data.correo);

      // Mostrar mensaje de bienvenida
      console.log(`Bienvenido ${data.nombreCompleto} (${data.rolNombre})`);

      // Redirigir automáticamente según el rol
      window.location.href = data.rutaRedireccion;

    } else {
      // Mostrar error
      alert(result.message);
    }
  } catch (error) {
    console.error('Error en login:', error);
    alert('Error de conexión con el servidor');
  }
}

// Uso
login('melqui@gmail.com', 'C@rene!@71');
```

---

### Ejemplo React

```jsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

function LoginForm() {
  const [correo, setCorreo] = useState('');
  const [contrasena, setContrasena] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const response = await fetch('http://localhost:5246/Cuentas/ValidarCuenta', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ correo, contrasena })
});

      const result = await response.json();

      if (result.success) {
      const { data } = result;
 
        // Guardar en contexto/estado global
        sessionStorage.setItem('user', JSON.stringify(data));

        // Redirigir según el rol
        navigate(data.rutaRedireccion);

        // O usar switch para rutas personalizadas
  // switch (data.rol) {
     //   case 0: navigate('/admin'); break;
        //   case 1: navigate('/gestor'); break;
        //   case 2: navigate('/cliente'); break;
        // }

      } else {
        setError(result.message);
    }
    } catch (err) {
      setError('Error de conexión');
    }
  };

  return (
 <form onSubmit={handleLogin}>
      <input 
  type="email" 
        value={correo}
        onChange={(e) => setCorreo(e.target.value)}
        placeholder="Correo"
      required
      />
      <input 
        type="password" 
        value={contrasena}
        onChange={(e) => setContrasena(e.target.value)}
        placeholder="Contraseña"
    required
      />
      {error && <p className="error">{error}</p>}
      <button type="submit">Iniciar Sesión</button>
    </form>
  );
}
```

---

### Ejemplo Vue.js

```vue
<template>
  <form @submit.prevent="login">
    <input v-model="correo" type="email" placeholder="Correo" required />
    <input v-model="contrasena" type="password" placeholder="Contraseña" required />
    <p v-if="error" class="error">{{ error }}</p>
    <button type="submit">Iniciar Sesión</button>
  </form>
</template>

<script>
export default {
  data() {
    return {
      correo: '',
      contrasena: '',
      error: ''
    };
  },
  methods: {
    async login() {
      this.error = '';

      try {
        const response = await fetch('http://localhost:5246/Cuentas/ValidarCuenta', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
            correo: this.correo,
         contrasena: this.contrasena
          })
        });

     const result = await response.json();

  if (result.success) {
          // Guardar usuario
 this.$store.commit('setUser', result.data);

          // Redirigir automáticamente
    this.$router.push(result.data.rutaRedireccion);

        } else {
          this.error = result.message;
     }
      } catch (err) {
        this.error = 'Error de conexión';
   }
    }
  }
};
</script>
```

---

## ?? Ejemplo con Guard/Middleware

### React Router Guard

```jsx
import { Navigate } from 'react-router-dom';

function ProtectedRoute({ children, requiredRole }) {
  const user = JSON.parse(sessionStorage.getItem('user'));

  if (!user) {
    return <Navigate to="/login" />;
  }

  if (requiredRole !== undefined && user.rol !== requiredRole) {
    // Redirigir a su dashboard correcto
    return <Navigate to={user.rutaRedireccion} />;
  }

  return children;
}

// Uso en rutas
<Route path="/admin/*" element={
  <ProtectedRoute requiredRole={0}>
    <AdminDashboard />
  </ProtectedRoute>
} />

<Route path="/gestor/*" element={
  <ProtectedRoute requiredRole={1}>
    <GestorDashboard />
  </ProtectedRoute>
} />

<Route path="/cliente/*" element={
  <ProtectedRoute requiredRole={2}>
 <ClienteDashboard />
  </ProtectedRoute>
} />
```

---

## ?? Logs del Sistema

El endpoint genera logs útiles para debugging:

### Login Exitoso - Administrador
```
?? Intento de login: admin@banco.com
? Login exitoso - Administrador: Admin
```

### Login Exitoso - Gestor
```
?? Intento de login: juan.gestor@banco.com
? Login exitoso - Gestor: Juan
```

### Login Exitoso - Cliente
```
?? Intento de login: melqui@gmail.com
? Login exitoso - Cliente: Melissa
```

### Login Fallido
```
?? Intento de login: usuario@fake.com
? Login fallido para: usuario@fake.com
```

### Error del Sistema
```
?? Intento de login: melqui@gmail.com
?? ERROR en login: Connection timeout
```

---

## ?? Pruebas

### Test 1: Login como Administrador

**Request:**
```bash
POST http://localhost:5246/Cuentas/ValidarCuenta
Content-Type: application/json

{
  "correo": "admin@banco.com",
  "contrasena": "Admin123!"
}
```

**Verificar:**
- ? Status: 200 OK
- ? `success: true`
- ? `data.rol: 0`
- ? `data.rolNombre: "Administrador"`
- ? `data.rutaRedireccion: "/admin/dashboard"`

---

### Test 2: Login como Gestor

**Request:**
```bash
POST http://localhost:5246/Cuentas/ValidarCuenta
Content-Type: application/json

{
  "correo": "juan.gestor@banco.com",
  "contrasena": "Gestor123!"
}
```

**Verificar:**
- ? Status: 200 OK
- ? `data.rol: 1`
- ? `data.rolNombre: "Gestor"`
- ? `data.rutaRedireccion: "/gestor/dashboard"`

---

### Test 3: Login como Cliente

**Request:**
```bash
POST http://localhost:5246/Cuentas/ValidarCuenta
Content-Type: application/json

{
  "correo": "melqui@gmail.com",
  "contrasena": "C@rene!@71"
}
```

**Verificar:**
- ? Status: 200 OK
- ? `data.rol: 2`
- ? `data.rolNombre: "Cliente"`
- ? `data.rutaRedireccion: "/cliente/dashboard"`

---

### Test 4: Credenciales Incorrectas

**Request:**
```bash
POST http://localhost:5246/Cuentas/ValidarCuenta
Content-Type: application/json

{
  "correo": "melqui@gmail.com",
  "contrasena": "password_incorrecta"
}
```

**Verificar:**
- ? Status: 401 Unauthorized
- ? `success: false`
- ? `message: "Correo o contraseña incorrectos"`

---

## ?? Ventajas del Nuevo Formato

### 1. Redirección Automática
```javascript
// ANTES: Lógica manual en frontend
if (user.rol === 0) window.location.href = '/admin/dashboard';
else if (user.rol === 1) window.location.href = '/gestor/dashboard';
else window.location.href = '/cliente/dashboard';

// AHORA: Directo desde el backend
window.location.href = data.rutaRedireccion;
```

### 2. Nombre de Rol Legible
```javascript
// ANTES: Convertir número a texto
const rolNombre = user.rol === 0 ? 'Admin' : user.rol === 1 ? 'Gestor' : 'Cliente';

// AHORA: Ya viene del backend
console.log(`Bienvenido ${data.rolNombre}`);
```

### 3. Nombre Completo
```javascript
// ANTES: Concatenar en frontend
const nombreCompleto = `${user.nombre} ${user.primerApellido} ${user.segundoApellido}`;

// AHORA: Ya viene concatenado
console.log(`Usuario: ${data.nombreCompleto}`);
```

---

## ?? Rutas Sugeridas por Rol

### Administrador (`/admin/dashboard`)
- `/admin/dashboard` - Panel principal
- `/admin/usuarios` - Gestión de usuarios
- `/admin/gestores` - Gestión de gestores
- `/admin/clientes` - Vista de todos los clientes
- `/admin/reportes` - Reportes del sistema
- `/admin/configuracion` - Configuración global

### Gestor (`/gestor/dashboard`)
- `/gestor/dashboard` - Panel principal
- `/gestor/clientes` - Lista de clientes
- `/gestor/beneficiarios` - Aprobar/rechazar beneficiarios
- `/gestor/transferencias` - Aprobar/rechazar transferencias
- `/gestor/reportes` - Reportes de clientes

### Cliente (`/cliente/dashboard`)
- `/cliente/dashboard` - Panel principal
- `/cliente/cuentas` - Mis cuentas bancarias
- `/cliente/transferencias` - Realizar transferencias
- `/cliente/beneficiarios` - Mis beneficiarios
- `/cliente/servicios` - Pago de servicios
- `/cliente/historial` - Historial de movimientos

---

## ?? Seguridad

### Datos NO Expuestos
- ? Contraseña (nunca se devuelve)
- ? Datos sensibles de otras cuentas

### Datos Expuestos
- ? Información básica del usuario autenticado
- ? Rol y permisos
- ? Información para personalización de UI

### Recomendaciones
1. Implementar JWT tokens para autenticación
2. Agregar refresh tokens
3. Implementar rate limiting
4. Agregar captcha después de N intentos fallidos
5. Registrar intentos de login en logs de auditoría

---

## ?? Resumen de Cambios

### Campos Nuevos en Response
- ? `rolNombre` - Nombre legible del rol
- ? `nombreCompleto` - Nombre completo concatenado
- ? `rutaRedireccion` - Ruta para redirección automática

### Logs Mejorados
- ? Emoji ?? para intentos de login
- ? Emoji ? para login exitoso con rol
- ? Emoji ? para login fallido
- ? Emoji ?? para errores del sistema

### Compatibilidad
- ? **Retrocompatible** - Todos los campos anteriores se mantienen
- ? Solo se agregaron nuevos campos
- ? Clientes antiguos siguen funcionando

---

## ? Estado

- **Implementado:** ? Completado
- **Build:** ? Exitoso sin errores
- **Testing:** ? Pendiente
- **Documentación:** ? Completa

---

**Fecha de implementación:** ${new Date().toLocaleDateString()}  
**Versión:** 2.3  
**Autor:** Sistema de Banca en Línea
