# PRUEBA DOTNETCORE - ANGULAR

Prueba técnica con .NET core y Angular

## Datos iniciales de base de datos

Usuarios iniciales:

| usuario  | password  |
| -------- | --------- |
| rendarac | admin1234 |

Roles: Admin, User

El script de base la base de datos se encuentra en la ruta:  backend/database

## Avances

### Primer avance

Debido a un inconveniente con git en mi máquina local
tuve que reinicar el repositorio por lo que el primer
avance corresponde al primer commit 'REINICIO repositorio git'

Avances:

* Script de base de datos
* Añadidos: Controladores Login, User
* Añadido: pantalla de login

### Segundo avance

En el segundo avance se completó la característica de inicio de sesión  

con una implementación básica de JWT

Falta: Administrar los intentos fallidos

Avances:

* Añadido: Funciones de login, logout en LoginController
* Añadido: Servicios de Login, User y guards para las rutas en frontend
* Añadido: Mostrar información de usuario en la página inicial (home)


## TERCER AVANCE

Avances
* Añadido: gestión de JWT con uso de cookies
* Añadido: actualización de perfil de usario

## CUARTO AVANCE
Acances
* Añadido: actualización de constraseña de usuario
* Añadido: dashboard para admin, tabla de usuarios, diálogo para nuevo usuario