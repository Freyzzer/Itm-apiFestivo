pipeline {
    agent any
    
    environment {
        DOCKER_REGISTRY = 'localhost:5000' // Registro Docker local
        API_IMAGE_NAME = 'dockerapifestivos'
        DB_IMAGE_NAME = 'dockerbdfestivos'
        NETWORK_NAME = 'redcalendariolaboral'
    }
    
    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/Freyzzer/Itm-apiFestivo.git'
                echo 'Código del repositorio descargado correctamente'
            }
        }
        
        stage('Build API') {
            steps {
                script {
                    // Construir la imagen Docker para el microservicio API
                    docker.build("${API_IMAGE_NAME}", '-f Dockerfile.api .')
                    echo 'Imagen Docker para API construida correctamente'
                }
            }
        }
        
        stage('Build Database') {
            steps {
                script {
                    // Construir la imagen Docker para la base de datos
                    docker.build("${DB_IMAGE_NAME}", '-f Dockerfile.db .')
                    echo 'Imagen Docker para base de datos construida correctamente'
                }
            }
        }
        
        stage('Create Network') {
            steps {
                script {
                    // Crear red Docker si no existe
                    def networkExists = sh(script: "docker network ls --filter name=${NETWORK_NAME} --format '{{.Name}}'", returnStdout: true).trim()
                    if (!networkExists) {
                        sh "docker network create ${NETWORK_NAME}"
                        echo "Red Docker ${NETWORK_NAME} creada"
                    } else {
                        echo "Red Docker ${NETWORK_NAME} ya existe"
                    }
                }
            }
        }
        
        stage('Deploy Database') {
            steps {
                script {
                    // Detener y eliminar contenedor existente si existe
                    sh "docker stop ${DB_IMAGE_NAME} || true"
                    sh "docker rm ${DB_IMAGE_NAME} || true"
                    
                    // Ejecutar contenedor de base de datos en la red creada
                    sh """
                        docker run -d \
                        --name ${DB_IMAGE_NAME} \
                        --network ${NETWORK_NAME} \
                        -e 'ACCEPT_EULA=Y' \
                        -e 'SA_PASSWORD=yourStrong(!)Password' \
                        ${DB_IMAGE_NAME}
                    """
                    echo 'Contenedor de base de datos desplegado correctamente'
                }
            }
        }
        
        stage('Deploy API') {
            steps {
                script {
                    // Detener y eliminar contenedor existente si existe
                    sh "docker stop ${API_IMAGE_NAME} || true"
                    sh "docker rm ${API_IMAGE_NAME} || true"
                    
                    // Ejecutar contenedor de API en la misma red
                    sh """
                        docker run -d \
                        --name ${API_IMAGE_NAME} \
                        --network ${NETWORK_NAME} \
                        -p 8080:80 \
                        ${API_IMAGE_NAME}
                    """
                    echo 'Contenedor de API desplegado correctamente'
                }
            }
        }
        
        stage('Health Check') {
            steps {
                script {
                    // Verificar que los servicios estén funcionando
                    sleep(time: 30, unit: 'SECONDS') // Esperar a que los servicios inicien
                    
                    // Verificar estado del contenedor de API
                    def apiStatus = sh(script: "docker inspect -f '{{.State.Status}}' ${API_IMAGE_NAME}", returnStdout: true).trim()
                    if (apiStatus != 'running') {
                        error "El contenedor de API no está corriendo. Estado: ${apiStatus}"
                    }
                    
                    // Verificar estado del contenedor de base de datos
                    def dbStatus = sh(script: "docker inspect -f '{{.State.Status}}' ${DB_IMAGE_NAME}", returnStdout: true).trim()
                    if (dbStatus != 'running') {
                        error "El contenedor de base de datos no está corriendo. Estado: ${dbStatus}"
                    }
                    
                    echo 'Todos los servicios están funcionando correctamente'
                }
            }
        }
    }
    
    post {
        success {
            echo 'Pipeline ejecutado exitosamente! Los contenedores están corriendo en la red redcalendariolaboral'
        }
        failure {
            echo 'Pipeline falló. Revisar los logs para más detalles.'
        }
    }
}