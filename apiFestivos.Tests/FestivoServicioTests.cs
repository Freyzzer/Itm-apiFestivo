using apiFestivos.Aplicacion.Servicios;
using apiFestivos.Core.Interfaces.Repositorios;
using apiFestivos.Dominio.Entidades;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apiFestivos.Tests
{
    public class FestivoServicioTests
    {
        private readonly Mock<IFestivoRepositorio> _mockRepositorio;
        private readonly FestivoServicio _servicio;

        public FestivoServicioTests()
        {
            _mockRepositorio = new Mock<IFestivoRepositorio>();
            _servicio = new FestivoServicio(_mockRepositorio.Object);
        }

        [Fact]
        public async Task EsFestivo_FechaEsFestiva_DevuelveVerdadero()
        {
            // Arrange
            var fecha = new DateTime(2025, 1, 1);
            var festivos = new List<Festivo>
{
new Festivo { Id = 1, Dia = 1, Mes = 1, Nombre = "Año nuevo", IdTipo = 1, DiasPascua = 0 }
};
            _mockRepositorio.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await _servicio.EsFestivo(fecha);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task EsFestivo_FechaNoEsFestiva_DevuelveFalso()
        {
            // Arrange
            var fecha = new DateTime(2025, 1, 2);
            var festivos = new List<Festivo>
{
new Festivo { Id = 1, Dia = 1, Mes = 1, Nombre = "Año nuevo", IdTipo = 1, DiasPascua = 0 }
};
            _mockRepositorio.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await _servicio.EsFestivo(fecha);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ObtenerAño_FestivoTipo1_DevuelveFechaEsperada()
        {
            // Arrange
            var año = 2025;
            var festivos = new List<Festivo>
{
new Festivo { Id = 1, Dia = 1, Mes = 1, Nombre = "Año nuevo", IdTipo = 1, DiasPascua = 0 }
};
            _mockRepositorio.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await _servicio.ObtenerAño(año);

            // Assert
            var fechaFestivo = resultado.First();
            Assert.Equal(new DateTime(2025, 1, 1), fechaFestivo.Fecha);
            Assert.Equal("Año nuevo", fechaFestivo.Nombre);
        }

        [Fact]
        public async Task ObtenerAño_FestivoTipo2_DevuelveSiguienteLunes()
        {
            // Arrange
            var año = 2025;
            var festivos = new List<Festivo>
{
new Festivo { Id = 2, Dia = 6, Mes = 1, Nombre = "Santos Reyes", IdTipo = 2, DiasPascua = 0 }
};
            _mockRepositorio.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await _servicio.ObtenerAño(año);

            // Assert
            var fechaFestivo = resultado.First();
            var fechaBase = new DateTime(2025, 1, 6); // Lunes
            Assert.Equal(fechaBase, fechaFestivo.Fecha);
            Assert.Equal("Santos Reyes", fechaFestivo.Nombre);
        }

        [Fact]
        public async Task ObtenerAño_FestivoTipo4_DevuelveSiguienteLunesBasadoEnPascua()
        {
            // Arrange
            var año = 2025;
            var festivos = new List<Festivo>
{
new Festivo { Id = 8, Dia = 0, Mes = 0, Nombre = "Ascensión del Señor", IdTipo = 4, DiasPascua = 40 }
};
            _mockRepositorio.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await _servicio.ObtenerAño(año);

            // Assert
            var fechaFestivo = resultado.First();
            Assert.Equal(new DateTime(2025, 6, 2), fechaFestivo.Fecha);
            Assert.Equal("Ascensión del Señor", fechaFestivo.Nombre);
        }
    }
}