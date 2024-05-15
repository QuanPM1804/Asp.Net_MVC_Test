using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Asp_MVC1.Controllers;
using Asp_MVC1.Models;

namespace Asp_MVC1.Tests
{
    public class RookiesControllerTests
    {
        private readonly Mock<IPersonRepository> _mockRepository;
        private readonly RookiesController _controller;
        private readonly List<Person> _persons;

        public RookiesControllerTests()
        {
            _mockRepository = new Mock<IPersonRepository>();
            _controller = new RookiesController(_mockRepository.Object);
            _persons = new List<Person>
        {
            new Person { FirstName = "Van A", LastName = "Nguyen", Gender = "Male", DateOfBirth = new DateTime(2000, 3, 15), PhoneNumber = "1234567890", BirthPlace = "Ha Noi", IsGraduated = true, Id = 1 },
            new Person { FirstName = "Van B", LastName = "Nguyen", Gender = "Male", DateOfBirth = new DateTime(1999, 6, 20), PhoneNumber = "9876543210", BirthPlace = "Sai Gon", IsGraduated = false, Id = 2 },
            new Person { FirstName = "Thi C", LastName = "Nguyen", Gender = "Female", DateOfBirth = new DateTime(2001, 6, 20), PhoneNumber = "9876543210", BirthPlace = "Sai Gon", IsGraduated = false, Id = 3 },
        };
        }

        [Fact]
        public void Index_ReturnsViewResultWithAllPersons()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
            // Act
            var result = _controller.Index();
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Person>>(viewResult.Model);
            Assert.Equal(_persons.Count, model.Count());
        }

        [Fact]
        public void MaleMembers_ReturnsViewResultWithMalePersons()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
            var expectedMaleMembers = _persons.Where(p => p.Gender == "Male").ToList();
            // Act
            var result = _controller.MaleMembers();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Person>>(viewResult.Model);
            // Assert
            Assert.Equal(expectedMaleMembers.Count, model.Count());
        }
        [Fact]
        public void OldestMember_ReturnsViewResultWithOldestPerson()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
            var expectedOldestMember = _persons.OrderBy(p => p.DateOfBirth).FirstOrDefault();
            // Act
            var result = _controller.OldestMember();
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Person>(viewResult.Model);
            Assert.Equal(expectedOldestMember, model);
        }

        [Fact]
        public void FullNames_ReturnsViewResultWithFullNames()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
            var expectedFullNames = _persons.Select(p => $"{p.LastName} {p.FirstName}").ToList();
            // Act
            var result = _controller.FullNames();
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Model);
            Assert.Equal(expectedFullNames, model);
        }

        [Fact]
        public void FilterByBirthYear_ReturnsViewResultWithFilteredPersons()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
            var year = 2000;
            var expectedFilteredPersons = _persons.Where(p => p.DateOfBirth.Year == year).ToList();
            // Act
            var result = _controller.FilterByBirthYear(year);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Person>>(viewResult.Model);
            // Assert
            Assert.Equal(expectedFilteredPersons.Count, model.Count());
        }

        [Fact]
        public void Create_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();
            // Assert
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void Detail_ReturnsViewResultWithPerson()
        {
            // Arrange
            var personId = 1;
            var expectedPerson = _persons.FirstOrDefault(p => p.Id == personId);
            _mockRepository.Setup(repo => repo.GetById(personId)).Returns(expectedPerson);
            // Act
            var result = _controller.Detail(personId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Person>(viewResult.Model);
            // Assert
            Assert.Equal(expectedPerson, model);
        }

        [Fact]
        public void Edit_WithValidId_ReturnsViewResultWithPerson()
        {
            // Arrange
            var personId = 1;
            var expectedPerson = _persons.FirstOrDefault(p => p.Id == personId);
            _mockRepository.Setup(repo => repo.GetById(personId)).Returns(expectedPerson);
            // Act
            var result = _controller.Edit(personId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Person>(viewResult.Model);
            // Assert
            Assert.Equal(expectedPerson, model);
        }

        [Fact]
        public void EditPost_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var personId = 1;
            var existingPerson = _persons.FirstOrDefault(p => p.Id == personId);
            var updatedPerson = new Person { Id = existingPerson.Id, FirstName = "Updated", LastName = existingPerson.LastName, Gender = existingPerson.Gender, DateOfBirth = existingPerson.DateOfBirth, PhoneNumber = existingPerson.PhoneNumber, BirthPlace = existingPerson.BirthPlace, IsGraduated = existingPerson.IsGraduated };
            // Act
            var result = _controller.Edit(personId, updatedPerson);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            // Assert
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
        [Fact]
        public void EditPost_WithInvalidModel_ReturnsViewResult()
        {
            // Arrange
            var personId = 1;
            var existingPerson = _persons.First(p => p.Id == personId);
            var updatedPerson = new Person { Id = existingPerson.Id, FirstName = null, LastName = existingPerson.LastName, Gender = existingPerson.Gender, DateOfBirth = existingPerson.DateOfBirth, PhoneNumber = existingPerson.PhoneNumber, BirthPlace = existingPerson.BirthPlace, IsGraduated = existingPerson.IsGraduated };
            _controller.ModelState.AddModelError("FirstName", "The FirstName field is required");
            // Act
            var result = _controller.Edit(personId, updatedPerson);
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Person>(viewResult.Model);
            Assert.Equal(updatedPerson, model);
        }
        [Fact]
        public void Delete_WithValidId_RedirectsToIndexWithMessage()
        {
            // Arrange
            var personId = 1;
            var expectedPerson = _persons.First(p => p.Id == personId);
            _mockRepository.Setup(repo => repo.GetById(personId)).Returns(expectedPerson);
            // Act
            var result = _controller.Delete(personId);
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.True(redirectToActionResult.RouteValues.ContainsKey("message"));
            Assert.Equal($"Person {personId} was removed from the list successfully!", redirectToActionResult.RouteValues["message"]);
        }

        [Fact]
        public void Delete_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var personId = 100;
            _mockRepository.Setup(repo => repo.GetById(personId)).Returns((Person)null);
            // Act
            var result = _controller.Delete(personId);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void ExportToExcel_ReturnsFileStreamResult()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);

            // Act
            var result = _controller.ExportToExcel();

            // Assert
            var fileStreamResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileStreamResult.ContentType);
            Assert.True(fileStreamResult.FileDownloadName.StartsWith("Persons_"));
            Assert.True(fileStreamResult.FileDownloadName.EndsWith(".xlsx"));
        }
    }
}