# CARAXES-INMOBILIARIA-LAB2

> Sistema de gestión de alquileres temporarios para una agencia inmobiliaria.
> Primera entrega: ABM de Propietarios e Inquilinos.

## 👥 Integrantes del Grupo

* **José Bossa** - *jose.bossa.3@gmail.com* - [@josebossa3-cmyk](https://github.com/josebossa3-cmyk)
* **Fernando Suarez** - *jorgefernandosuarez@gmail.com* - [@Fernando-Suarez](https://github.com/Fernando-Suarez)
* **Jesús Emanuel García** - *dupre.dev@gmail.com* - [@emadupre](https://github.com/emadupre)

## 📐 Modelado de Datos

### Diagrama Entidad-Relación (DER) / Diagrama de Clases

```mermaid
erDiagram
    USUARIO ||--o{ RESERVA : "crea"
    USUARIO |o--o{ RESERVA : "termina"
    USUARIO ||--o{ PAGO : "crea"
    USUARIO |o--o{ PAGO : "anula"

    PROPIETARIO ||--o{ INMUEBLE : "posee"
    TIPO_INMUEBLE ||--o{ INMUEBLE : "clasifica"
    INMUEBLE ||--o{ IMAGEN_INMUEBLE : "tiene"

    INQUILINO ||--o{ RESERVA : "realiza"
    INMUEBLE ||--o{ RESERVA : "asociada"
    RESERVA ||--o{ PAGO : "contiene"
    RESERVA ||--o{ RESERVA : "renovacion"

    USUARIO {
        int Id PK
        string Email
        string PasswordHash
        string NombreCompleto
        string Avatar
        string Rol
        datetime FechaCreacion
    }

    PROPIETARIO {
        int Id PK
        string DNI
        string NombreCompleto
        string Telefono
        string Email
        string Direccion
        datetime FechaAlta
    }

    INQUILINO {
        int Id PK
        string DNI
        string NombreCompleto
        string Telefono
        string Email
        string Direccion
        datetime FechaAlta
    }

    TIPO_INMUEBLE {
        int Id PK
        string Nombre
    }

    INMUEBLE {
        int Id PK
        int PropietarioId FK
        int TipoInmuebleId FK
        string Direccion
        int Cupo
        decimal PrecioPorDia
        decimal PorcentajeReserva
        bool Estado
        string Coordenadas
        string ImagenPortada
        datetime FechaAlta
    }

    IMAGEN_INMUEBLE {
        int Id PK
        int InmuebleId FK
        string Url
        bool EsPortada
        int Orden
    }

    RESERVA {
        int Id PK
        int InquilinoId FK
        int InmuebleId FK
        date FechaInicio
        date FechaFin
        date FechaFinOriginal
        decimal MontoPorDia
        decimal PorcentajeReserva
        string Estado
        date FechaTerminacion
        decimal Multa
        int ReservaRenovadaDeId FK
        int UsuarioCreadorId FK
        int UsuarioTerminadorId FK
        datetime FechaCreacion
    }

    PAGO {
        int Id PK
        int ReservaId FK
        string Concepto
        datetime FechaPago
        decimal Importe
        string Estado
        int UsuarioCreadorId FK
        int UsuarioAnuladorId FK
        datetime FechaAnulacion
    }