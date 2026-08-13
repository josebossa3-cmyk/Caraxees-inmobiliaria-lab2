-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: inmobilariadb
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `imagenesinmueble`
--

DROP TABLE IF EXISTS `imagenesinmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `imagenesinmueble` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `InmuebleId` int NOT NULL,
  `Url` varchar(500) NOT NULL,
  `EsPortada` tinyint(1) NOT NULL DEFAULT '0',
  `Orden` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_ImagenesInmueble_Inmuebles` (`InmuebleId`),
  CONSTRAINT `FK_ImagenesInmueble_Inmuebles` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `imagenesinmueble`
--

LOCK TABLES `imagenesinmueble` WRITE;
/*!40000 ALTER TABLE `imagenesinmueble` DISABLE KEYS */;
/*!40000 ALTER TABLE `imagenesinmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inmuebles`
--

DROP TABLE IF EXISTS `inmuebles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmuebles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PropietarioId` int NOT NULL,
  `TipoInmuebleId` int NOT NULL,
  `Direccion` varchar(250) NOT NULL,
  `Cupo` int NOT NULL,
  `PrecioPorDia` decimal(18,2) NOT NULL,
  `PorcentajeReserva` decimal(5,2) NOT NULL DEFAULT '30.00',
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  `Coordenadas` varchar(100) DEFAULT NULL,
  `ImagenPortada` varchar(500) DEFAULT NULL,
  `FechaAlta` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_Inmuebles_Propietarios` (`PropietarioId`),
  KEY `FK_Inmuebles_TiposInmueble` (`TipoInmuebleId`),
  CONSTRAINT `FK_Inmuebles_Propietarios` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`),
  CONSTRAINT `FK_Inmuebles_TiposInmueble` FOREIGN KEY (`TipoInmuebleId`) REFERENCES `tiposinmueble` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inmuebles`
--

LOCK TABLES `inmuebles` WRITE;
/*!40000 ALTER TABLE `inmuebles` DISABLE KEYS */;
/*!40000 ALTER TABLE `inmuebles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inquilinos`
--

DROP TABLE IF EXISTS `inquilinos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilinos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DNI` varchar(20) NOT NULL,
  `NombreCompleto` varchar(150) NOT NULL,
  `Telefono` varchar(30) DEFAULT NULL,
  `Email` varchar(150) DEFAULT NULL,
  `Direccion` varchar(250) DEFAULT NULL,
  `FechaAlta` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `DNI` (`DNI`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilinos`
--

LOCK TABLES `inquilinos` WRITE;
/*!40000 ALTER TABLE `inquilinos` DISABLE KEYS */;
INSERT INTO `inquilinos` VALUES (1,'30555666','Pedro Sánchez','3515559876','pedros@mail.com','San Martín 789','2026-08-13 17:26:01'),(2,'31888777','Lucía Fernández','3515555432','luciaf@mail.com','9 de Julio 321','2026-08-13 17:26:01');
/*!40000 ALTER TABLE `inquilinos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pagos`
--

DROP TABLE IF EXISTS `pagos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pagos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ReservaId` int NOT NULL,
  `Concepto` varchar(150) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `Importe` decimal(18,2) NOT NULL,
  `Estado` varchar(20) NOT NULL DEFAULT 'Activo',
  `UsuarioCreadorId` int DEFAULT NULL,
  `UsuarioAnuladorId` int DEFAULT NULL,
  `FechaAnulacion` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Pagos_Reservas` (`ReservaId`),
  KEY `FK_Pagos_UsuariosCreador` (`UsuarioCreadorId`),
  KEY `FK_Pagos_UsuariosAnulador` (`UsuarioAnuladorId`),
  CONSTRAINT `FK_Pagos_Reservas` FOREIGN KEY (`ReservaId`) REFERENCES `reservas` (`Id`),
  CONSTRAINT `FK_Pagos_UsuariosAnulador` FOREIGN KEY (`UsuarioAnuladorId`) REFERENCES `usuarios` (`Id`),
  CONSTRAINT `FK_Pagos_UsuariosCreador` FOREIGN KEY (`UsuarioCreadorId`) REFERENCES `usuarios` (`Id`),
  CONSTRAINT `pagos_chk_1` CHECK ((`Estado` in (_utf8mb4'Activo',_utf8mb4'Anulado')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pagos`
--

LOCK TABLES `pagos` WRITE;
/*!40000 ALTER TABLE `pagos` DISABLE KEYS */;
/*!40000 ALTER TABLE `pagos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propietarios`
--

DROP TABLE IF EXISTS `propietarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietarios` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DNI` varchar(20) NOT NULL,
  `NombreCompleto` varchar(150) NOT NULL,
  `Telefono` varchar(30) DEFAULT NULL,
  `Email` varchar(150) DEFAULT NULL,
  `Direccion` varchar(250) DEFAULT NULL,
  `FechaAlta` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `DNI` (`DNI`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propietarios`
--

LOCK TABLES `propietarios` WRITE;
/*!40000 ALTER TABLE `propietarios` DISABLE KEYS */;
INSERT INTO `propietarios` VALUES (1,'20333444','Juan Carlos Pérez','3515551234','jcperez@mail.com','Av. Colón 123','2026-08-13 17:25:58'),(2,'27444555','María Laura Gómez','3515555678','mlgomez@mail.com','Belgrano 456','2026-08-13 17:25:58');
/*!40000 ALTER TABLE `propietarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reservas`
--

DROP TABLE IF EXISTS `reservas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `InquilinoId` int NOT NULL,
  `InmuebleId` int NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `FechaFinOriginal` date NOT NULL,
  `MontoPorDia` decimal(18,2) NOT NULL,
  `PorcentajeReserva` decimal(5,2) NOT NULL,
  `Estado` varchar(20) NOT NULL DEFAULT 'Vigente',
  `FechaTerminacion` date DEFAULT NULL,
  `Multa` decimal(18,2) DEFAULT NULL,
  `ReservaRenovadaDeId` int DEFAULT NULL,
  `UsuarioCreadorId` int DEFAULT NULL,
  `UsuarioTerminadorId` int DEFAULT NULL,
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_Reservas_Inquilinos` (`InquilinoId`),
  KEY `FK_Reservas_Inmuebles` (`InmuebleId`),
  KEY `FK_Reservas_UsuariosCreador` (`UsuarioCreadorId`),
  KEY `FK_Reservas_UsuariosTerminador` (`UsuarioTerminadorId`),
  KEY `FK_Reservas_ReservaOriginal` (`ReservaRenovadaDeId`),
  CONSTRAINT `FK_Reservas_Inmuebles` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`),
  CONSTRAINT `FK_Reservas_Inquilinos` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`),
  CONSTRAINT `FK_Reservas_ReservaOriginal` FOREIGN KEY (`ReservaRenovadaDeId`) REFERENCES `reservas` (`Id`),
  CONSTRAINT `FK_Reservas_UsuariosCreador` FOREIGN KEY (`UsuarioCreadorId`) REFERENCES `usuarios` (`Id`),
  CONSTRAINT `FK_Reservas_UsuariosTerminador` FOREIGN KEY (`UsuarioTerminadorId`) REFERENCES `usuarios` (`Id`),
  CONSTRAINT `CK_Reservas_Fechas` CHECK ((`FechaFin` > `FechaInicio`)),
  CONSTRAINT `reservas_chk_1` CHECK ((`Estado` in (_utf8mb4'Vigente',_utf8mb4'Finalizada',_utf8mb4'TerminadaAnticipadamente')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reservas`
--

LOCK TABLES `reservas` WRITE;
/*!40000 ALTER TABLE `reservas` DISABLE KEYS */;
/*!40000 ALTER TABLE `reservas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tiposinmueble`
--

DROP TABLE IF EXISTS `tiposinmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tiposinmueble` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tiposinmueble`
--

LOCK TABLES `tiposinmueble` WRITE;
/*!40000 ALTER TABLE `tiposinmueble` DISABLE KEYS */;
INSERT INTO `tiposinmueble` VALUES (1,'Casa'),(2,'Departamento'),(4,'Loft'),(3,'Monoambiente');
/*!40000 ALTER TABLE `tiposinmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(256) NOT NULL,
  `PasswordHash` text NOT NULL,
  `NombreCompleto` varchar(150) NOT NULL,
  `Avatar` varchar(500) DEFAULT NULL,
  `Rol` varchar(20) NOT NULL,
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`),
  CONSTRAINT `usuarios_chk_1` CHECK ((`Rol` in (_utf8mb4'Administrador',_utf8mb4'Empleado')))
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'admin@inmobiliaria.com','AQAAAAEAACcQAAAAE...','Administrador General',NULL,'Administrador','2026-08-13 17:24:25'),(2,'empleado@inmobiliaria.com','AQAAAAEAACcQAAAAE...','Empleado Demo',NULL,'Empleado','2026-08-13 17:24:25');
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'inmobilariadb'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-08-13 17:33:21
