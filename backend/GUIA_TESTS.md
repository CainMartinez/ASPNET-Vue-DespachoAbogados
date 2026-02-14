# Guía de Ejecución de Tests - AbogadosAPI

## 📝 Resumen del Proyecto de Tests

- **2 Tests Unitarios**: Prueban la lógica del servicio ClienteService
- **1 Test de Integración**: Prueba el endpoint completo de la API

---

## ✅ Tests Incluidos

### Tests Unitarios (ClienteServiceTests.cs)

1. **GetAllAsync_DebeRetornarTodosLosClientes**
   - Verifica que el servicio devuelva todos los clientes de la base de datos
   - Seeds: 2 clientes de prueba
   - Assertion: Debe retornar exactamente 2 clientes

2. **GetByIdAsync_ConIdValido_DebeRetornarCliente**
   - Verifica que el servicio devuelva un cliente específico por su ID
   - Seeds: Cliente con ID 1
   - Assertion: Debe retornar el cliente con nombre "Juan"

### Test de Integración (ClientesIntegrationTest.cs)

1. **GET_Clientes_DebeRetornar200ConListaDeClientes**
   - Hace una petición HTTP real a `/api/clientes`
   - Verifica que retorne código 200 y una lista de clientes
   - Usa `WebApplicationFactory` con base de datos en memoria
   - Seeds: 2 clientes de prueba
   - Assertions: Status 200 + al menos 2 clientes en respuesta

---

## 🚀 Ejecución de Tests - Paso a Paso

### 🐳 Ejecutar tests en Docker

**VENTAJA**: No necesitas tener .NET 9.0 SDK instalado localmente.

#### Paso 1: Navegar al directorio del backend
```bash
cd /backend
```

#### Paso 2: Ejecutar el script de tests
```bash
./run-tests.sh
```

**¡Eso es todo!** El script:
1. Construye una imagen Docker con .NET 9.0 SDK
2. Copia todos los archivos necesarios del proyecto
3. Restaura dependencias NuGet
4. Compila el proyecto
5. Ejecuta todos los tests
6. Muestra los resultados en la consola

**Resultado esperado:**
```
Test Run Successful.
Total tests: 3
     Passed: 3
 Total time: ~0.6 seconds
```

---

## ⚙️ Cómo Funcionan los Tests

### Tests Unitarios (`ClienteServiceTests`)

- Usan base de datos **en memoria** (InMemory Database)
- Cada test es completamente **independiente**
- Seeds de datos en el método constructor
- Implementa `IDisposable` para limpiar la base de datos después de cada test
- Validan **únicamente** la lógica del servicio `ClienteService`

### Test de Integración (`ClientesIntegrationTest`)

- Usa `CustomWebApplicationFactory` para levantar un **servidor de pruebas completo**
- La variable de entorno `USE_IN_MEMORY_DATABASE=true` hace que `Program.cs` omita la configuración de MySQL
- `CustomWebApplicationFactory` configura su propio DbContext con InMemory database
- Hace peticiones HTTP **reales** al servidor de pruebas
- Valida el **flujo completo**: Controller → Service → Repository → Database

---

## 🚨 Solución de Problemas

### ❌ Error: "You must install or update .NET to run this application"

**Causa**: No tienes .NET SDK 9.0 instalado localmente.

**Solución**: Usa la **Opción 1 (Docker)** que incluye .NET 9.0 automáticamente:
```bash
cd /Users/cain/Downloads/DAM/ASP-Abogados/backend
./run-tests.sh
```

---

### ❌ Error: "Permission denied: ./run-tests.sh"

**Causa**: El script no tiene permisos de ejecución.

**Solución**: Da permisos de ejecución al script:
```bash
chmod +x run-tests.sh
./run-tests.sh
```

---

### ❌ Los tests fallan con error de conexión MySQL

**Causa**: El test de integración está intentando conectarse a MySQL en lugar de usar InMemory.

**Solución**: Esto ya está resuelto con la configuración actual de `CustomWebApplicationFactory` que establece `USE_IN_MEMORY_DATABASE=true`.

---

### ❌ Error al construir imagen Docker

**Causa**: Posible problema con caché de Docker.

**Solución**: Reconstruye la imagen sin caché:
```bash
cd /Users/cain/Downloads/DAM/ASP-Abogados/backend
docker build --no-cache -f AbogadosAPI.Tests/Dockerfile.test -t abogados-tests .
docker run --rm abogados-tests
```


## 📝 Notas Técnicas Importantes

### Variables de Entorno en Tests

El sistema de tests usa la variable de entorno `USE_IN_MEMORY_DATABASE` para controlar el tipo de base de datos:

- **`USE_IN_MEMORY_DATABASE=true`**: Usa InMemory Database (para tests)
- **No establecida**: Usa MySQL (para producción)

Esta variable se establece automáticamente en `CustomWebApplicationFactory.cs` para tests de integración.

### Cómo Funciona el Test de Integración

1. `CustomWebApplicationFactory` establece `USE_IN_MEMORY_DATABASE=true`
2. Al iniciarse la aplicación, `Program.cs` detecta esta variable
3. Si está activa, **omite** la configuración de MySQL
4. `CustomWebApplicationFactory` configura su propio DbContext con InMemory
5. Seeds de datos de prueba se insertan automáticamente
6. El test hace peticiones HTTP al servidor de pruebas
7. El servidor responde usando la base de datos en memoria