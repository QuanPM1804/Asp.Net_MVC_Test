using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Asp_MVC1.Controllers;
using Asp_MVC1.Models;

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
        _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
        var result = _controller.Index();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Person>>(viewResult.Model);
        Assert.Equal(_persons.Count, model.Count());
    }

    [Fact]
    public void MaleMembers_ReturnsViewResultWithMalePersons()
    {
        _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
        var expectedMaleMembers = _persons.Where(p => p.Gender == "Male").ToList();
        var result = _controller.MaleMembers();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Person>>(viewResult.Model);
        Assert.Equal(expectedMaleMembers.Count, model.Count());
    }
    [Fact]
    public void OldestMember_ReturnsViewResultWithOldestPerson()
    {
        _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
        var expectedOldestMember = _persons.OrderBy(p => p.DateOfBirth).FirstOrDefault();
        var result = _controller.OldestMember();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Person>(viewResult.Model);
        Assert.Equal(expectedOldestMember, model);
    }

    [Fact]
    public void FullNames_ReturnsViewResultWithFullNames()
    {
        _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
        var expectedFullNames = _persons.Select(p => $"{p.LastName} {p.FirstName}").ToList();
        var result = _controller.FullNames();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Model);
        Assert.Equal(expectedFullNames, model);
    }

    [Fact]
    public void FilterByBirthYear_ReturnsViewResultWithFilteredPersons()
    {
        _mockRepository.Setup(repo => repo.GetAll()).Returns(_persons);
        var year = 2000;
        var expectedFilteredPersons = _persons.Where(p => p.DateOfBirth.Year == year).ToList();
        var result = _controller.FilterByBirthYear(year);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Person>>(viewResult.Model);
        Assert.Equal(expectedFilteredPersons.Count, model.Count());
    }

    [Fact]
    public void Create_ReturnsViewResult()
    {
        var result = _controller.Create();
        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public void Detail_ReturnsViewResultWithPerson()
    {
        var personId = 1;
        var expectedPerson = _persons.FirstOrDefault(p => p.Id == personId);
        _mockRepository.Setup(repo => repo.GetById(personId)).Returns(expectedPerson);
        var result = _controller.Detail(personId);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Person>(viewResult.Model);
        Assert.Equal(expectedPerson, model);
    }

    [Fact]
    public void Edit_WithValidId_ReturnsViewResultWithPerson()
    {
        var personId = 1;
        var expectedPerson = _persons.FirstOrDefault(p => p.Id == personId);
        _mockRepository.Setup(repo => repo.GetById(personId)).Returns(expectedPerson);
        var result = _controller.Edit(personId);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Person>(viewResult.Model);
        Assert.Equal(expectedPerson, model);
    }

    [Fact]
    public void EditPost_WithValidModel_RedirectsToIndex()
    {
        var personId = 1;
        var existingPerson = _persons.FirstOrDefault(p => p.Id == personId);
        var updatedPerson = new Person { Id = existingPerson.Id, FirstName = "Updated", LastName = existingPerson.LastName, Gender = existingPerson.Gender, DateOfBirth = existingPerson.DateOfBirth, PhoneNumber = existingPerson.PhoneNumber, BirthPlace = existingPerson.BirthPlace, IsGraduated = existingPerson.IsGraduated };
        var result = _controller.Edit(personId, updatedPerson);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }
    [Fact]
    public void EditPost_WithInvalidModel_ReturnsViewResult()
    {
        var personId = 1;
        var existingPerson = _persons.First(p => p.Id == personId);
        var updatedPerson = new Person { Id = existingPerson.Id, FirstName = null, LastName = existingPerson.LastName, Gender = existingPerson.Gender, DateOfBirth = existingPerson.DateOfBirth, PhoneNumber = existingPerson.PhoneNumber, BirthPlace = existingPerson.BirthPlace, IsGraduated = existingPerson.IsGraduated };
        _controller.ModelState.AddModelError("FirstName", "The FirstName field is required");
        var result = _controller.Edit(personId, updatedPerson);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Person>(viewResult.Model);
        Assert.Equal(updatedPerson, model);
    }
    [Fact]
    public void Delete_WithValidId_RedirectsToIndexWithMessage()
    {
        var personId = 1;
        var expectedPerson = _persons.First(p => p.Id == personId);
        _mockRepository.Setup(repo => repo.GetById(personId)).Returns(expectedPerson);
        var result = _controller.Delete(personId);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(redirectToActionResult.ControllerName);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.True(redirectToActionResult.RouteValues.ContainsKey("message"));
        Assert.Equal($"Person {personId} was removed from the list successfully!", redirectToActionResult.RouteValues["message"]);
    }

    [Fact]
    public void Delete_WithInvalidId_ReturnsNotFoundResult()
    {
        var personId = 100;
        _mockRepository.Setup(repo => repo.GetById(personId)).Returns((Person)null);
        var result = _controller.Delete(personId);
        Assert.IsType<NotFoundResult>(result);
    }
}
