using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PeopleManagement.Application.DTOs;
using PeopleManagement.Application.Interfaces;
using PeopleManagement.Application.Services;
using PeopleManagement.Domain.Entities;
using PeopleManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PeopleManagement.Test.Services
{
    public class PeopleServiceTests
    {
        private readonly Mock<IPersonRepository> _mockRepository;
        private readonly Mock<IPasswordHasher<Person>> _mockPasswordHasher;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PeopleService _peopleService;

        private readonly Person _defaultPerson;
        private readonly Person _defaultPersonAux;
        private readonly PersonDto _defaultPersonDto;
        private readonly PersonDto _defaultPersonAuxDto;

        public PeopleServiceTests()
        {
            _mockRepository = new Mock<IPersonRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher<Person>>();
            _mockMapper = new Mock<IMapper>();
            _peopleService = new PeopleService(_mockRepository.Object, _mockPasswordHasher.Object, _mockMapper.Object);

            _defaultPerson = new Person
            {
                Id = 1,
                Name = "Test Person 1",
                CPF = "12345678901",
                Email = "test@example.com",
                Gender = GenderType.Male,
                DateOfBirth = DateTime.Now.AddYears(-30)
            };

            _defaultPersonAux = new Person
            {
                Id = 2,
                Name = "Test Person 2",
                CPF = "12345678901",
                Email = "test@example.com",
                Gender = GenderType.Male,
                DateOfBirth = DateTime.Now.AddYears(-30)
            };

            _defaultPersonDto = new PersonDto
            {
                Name = "Test Person",
                CPF = "12345678901",
                Email = "test@example.com",
                Gender = GenderType.Male,
                DateOfBirth = DateTime.Now.AddYears(-30)
            };


            _defaultPersonAuxDto = new PersonDto
            {
                Name = "Test Person",
                CPF = "12345678901",
                Email = "test@example.com",
                Gender = GenderType.Male,
                DateOfBirth = DateTime.Now.AddYears(-30)
            };
        }

        [Fact]
        public async Task SearchAsync_WhenQueryIsEmpty_ReturnsAllPeople()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var query = new FilterCriteriaDto();
            var people = new List<Person> 
            {
                _defaultPerson,
                _defaultPersonAux
            };
            var personDtos = new List<PersonDto>
            {
                _defaultPersonDto,
                _defaultPersonAuxDto
            };

            _mockRepository.Setup(repo => repo.GetAllAsync(cancellationToken))
                .ReturnsAsync(people);

            _mockMapper.Setup(m => m.Map<PersonDto>(It.IsAny<Person>()))
                .Returns<Person>(p => new PersonDto 
                { 
                    Name = p.Name, 
                    CPF = p.CPF 
                });

            // Act
            var result = await _peopleService.SearchAsync(query, cancellationToken);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Person 1", result[0].Name);
            Assert.Equal("Test Person 2", result[1].Name);
            _mockRepository.Verify(repo => repo.GetAllAsync(cancellationToken), Times.Once);
            _mockRepository.Verify(repo => repo.QueryAsync(It.IsAny<FilterCriteriaDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SearchAsync_WhenQueryIsNotEmpty_ReturnsFilteredPeople()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var query = new FilterCriteriaDto { Name = "Test" };
            var people = new List<Person> 
            {
                _defaultPerson
            };
            var personDtos = new List<PersonDto>
            {
                _defaultPersonDto
            };

            _mockRepository.Setup(repo => repo.QueryAsync(query, cancellationToken))
                .ReturnsAsync(people);

            _mockMapper.Setup(m => m.Map<PersonDto>(It.IsAny<Person>()))
                .Returns<Person>(p => new PersonDto 
                { 
                    Name = p.Name, 
                    CPF = p.CPF 
                });

            // Act
            var result = await _peopleService.SearchAsync(query, cancellationToken);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Person 1", result[0].Name);
            _mockRepository.Verify(repo => repo.QueryAsync(query, cancellationToken), Times.Once);
            _mockRepository.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_WhenCpfIsUnique_AddsNewPerson()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var personDto = _defaultPersonDto;
            var person = _defaultPerson;

            _mockRepository.Setup(repo => repo.GetByCpfAsync(personDto.CPF, cancellationToken))
                .ReturnsAsync((Person)null);

            _mockMapper.Setup(m => m.Map<Person>(personDto))
                .Returns(person);

            _mockPasswordHasher.Setup(ph => ph.HashPassword(person, personDto.Password))
                .Returns("hashedPassword");

            // Act
            await _peopleService.AddAsync(personDto, cancellationToken);

            // Assert
            _mockRepository.Verify(repo => repo.GetByCpfAsync(personDto.CPF, cancellationToken), Times.Once);
            _mockPasswordHasher.Verify(ph => ph.HashPassword(person, personDto.Password), Times.Once);
            _mockRepository.Verify(repo => repo.AddAsync(person, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenCpfExists_ThrowsException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var personDto = _defaultPersonDto;
            var existingPerson = _defaultPerson;

            _mockRepository.Setup(repo => repo.GetByCpfAsync(personDto.CPF, cancellationToken))
                .ReturnsAsync(existingPerson);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PeopleManagementException>(
                () => _peopleService.AddAsync(personDto, cancellationToken));
            
            Assert.Equal("This CPF is already registered.", exception.Message);
            _mockRepository.Verify(repo => repo.GetByCpfAsync(personDto.CPF, cancellationToken), Times.Once);
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenPersonExists_ReturnsPerson()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var cpf = "12345678901";
            var person = _defaultPerson;
            var personDto = _defaultPersonDto;

            _mockRepository.Setup(repo => repo.GetByCpfAsync(cpf, cancellationToken))
                .ReturnsAsync(person);

            _mockMapper.Setup(m => m.Map<PersonDto>(person))
                .Returns(personDto);

            // Act
            var result = await _peopleService.GetByIdAsync(cpf, cancellationToken);

            // Assert
            Assert.Equal("Test Person", result.Name);
            Assert.Equal(cpf, result.CPF);
            _mockRepository.Verify(repo => repo.GetByCpfAsync(cpf, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenPersonDoesNotExist_ThrowsException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var cpf = "12345678901";

            _mockRepository.Setup(repo => repo.GetByCpfAsync(cpf, cancellationToken))
                .ReturnsAsync((Person)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PeopleManagementException>(
                () => _peopleService.GetByIdAsync(cpf, cancellationToken));
            
            Assert.Equal("Person not found.", exception.Message);
            _mockRepository.Verify(repo => repo.GetByCpfAsync(cpf, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenPersonExistsAndCpfUnchanged_UpdatesPerson()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var cpf = "12345678901";
            var personDto = _defaultPersonDto;
            var existingPerson = _defaultPerson;
            var updatedPerson = _defaultPersonAux;
            personDto.Name = "Updated Person";
            personDto.Email = "updated@example.com";

            _mockRepository.Setup(repo => repo.GetByCpfAsync(cpf, cancellationToken))
                .ReturnsAsync(existingPerson);

            _mockMapper.Setup(m => m.Map<Person>(personDto))
                .Returns(updatedPerson);

            _mockRepository.Setup(repo => repo.UpdateAsync(existingPerson.Id, It.IsAny<Person>(), cancellationToken))
                .ReturnsAsync(updatedPerson);

            _mockMapper.Setup(m => m.Map<PersonDto>(updatedPerson))
                .Returns(personDto);

            // Act
            var result = await _peopleService.UpdateAsync(cpf, personDto, cancellationToken);

            // Assert
            Assert.Equal("Updated Person", result.Name);
            Assert.Equal("updated@example.com", result.Email);
            Assert.Equal(GenderType.Male, result.Gender);
            _mockRepository.Verify(repo => repo.GetByCpfAsync(cpf, cancellationToken), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateAsync(existingPerson.Id, It.IsAny<Person>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenPersonExists_DeletesPerson()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var cpf = "12345678901";
            var person = _defaultPerson;
            var personDto = _defaultPersonDto;

            _mockRepository.Setup(repo => repo.GetByCpfAsync(cpf, cancellationToken))
                .ReturnsAsync(person);

            _mockRepository.Setup(repo => repo.DeleteAsync(person.Id, cancellationToken))
                .ReturnsAsync(true);

            _mockMapper.Setup(m => m.Map<PersonDto>(person))
                .Returns(personDto);

            // Act
            var result = await _peopleService.DeleteAsync(cpf, cancellationToken);

            // Assert
            Assert.Equal("Test Person", result.Name);
            Assert.Equal(cpf, result.CPF);
            _mockRepository.Verify(repo => repo.GetByCpfAsync(cpf, cancellationToken), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(person.Id, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenPersonDoesNotExist_ThrowsException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var cpf = "12345678901";

            _mockRepository.Setup(repo => repo.GetByCpfAsync(cpf, cancellationToken))
                .ReturnsAsync((Person)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PeopleManagementException>(
                () => _peopleService.DeleteAsync(cpf, cancellationToken));
            
            Assert.Equal("Person not found.", exception.Message);
            _mockRepository.Verify(repo => repo.GetByCpfAsync(cpf, cancellationToken), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}