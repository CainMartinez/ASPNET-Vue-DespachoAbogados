#!/bin/bash

# Script para ejecutar tests en Docker con .NET 9.0
# Uso: ./run-tests.sh

echo "Ejecutando tests en contenedor Docker con .NET 9.0..."
echo ""

# Ir al directorio del backend
cd "$(dirname "$0")"

# Construir imagen de tests usando el contexto correcto
docker build -f AbogadosAPI.Tests/Dockerfile.test \
             --file AbogadosAPI.Tests/Dockerfile.test \
             --build-arg BUILDKIT_CONTEXT_KEEP_GIT_DIR=1 \
             -t abogados-tests \
             --progress=plain \
             .

if [ $? -eq 0 ]; then
    echo ""
    echo "Imagen construida correctamente. Ejecutando tests..."
    echo ""
    
    # Ejecutar tests
    docker run --rm abogados-tests
    
    echo ""
    echo "Tests completados"
else
    echo ""
    echo "Error al construir la imagen de tests"
    exit 1
fi
