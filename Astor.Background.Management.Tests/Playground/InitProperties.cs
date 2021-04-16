using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Astor.Background.Management.Scraper.Tests.Playground
{
    [TestClass]
    public class InitProperties
    {
        [TestMethod]
        public void CanBeDeserializedToViaNewtonsoft()
        {
            var json = "{ 'Name' : 'Egor' }";
            var person = JsonConvert.DeserializeObject<Person>(json);
            
            Assert.AreEqual("Egor", person.Name);
        }

        [TestMethod]
        public void CanBeSetViaReflection()
        {
            var person = new Person();
            
            Assert.IsNull(person.Name);
            
            person.GetType().GetProperty("Name").SetValue(person, "Alex");
            
            Assert.AreEqual("Alex", person.Name);
        }

        [TestMethod]
        public void WorkWithWithSyntax()
        {
            var person = new Person();
            
            Assert.IsNull(person.Name);

            person = person with { Name = "John"};
            
            Assert.AreEqual("John", person.Name);
        }

        [TestMethod]
        public void DoesDeepEquality()
        {
            var firstGuy = new Person
            {
                Name = "Tom"
            };

            var secondGuy = new Person
            {
                Name = "Tom"
            };
            
            writeTwoGuys(firstGuy, secondGuy);
            Assert.AreEqual(firstGuy, secondGuy);

            secondGuy = secondGuy with { City = new City
            {
                Name = "London"
            }};
            
            writeTwoGuys(firstGuy, secondGuy);
            Assert.AreNotEqual(firstGuy, secondGuy);

            firstGuy = firstGuy with { City = new City
            {
                Name = "London"
            }};
            
            writeTwoGuys(firstGuy, secondGuy);
            Assert.AreEqual(firstGuy, secondGuy);

            secondGuy = secondGuy with { City = new City
            {
                Name = "Paris"
            }};
            
            writeTwoGuys(firstGuy, secondGuy);
            Assert.AreNotEqual(firstGuy, secondGuy);
        }

        private void writeTwoGuys(Person firstGuy, Person secondGuy)
        {
            Console.WriteLine($" first guy: {firstGuy}, secondGuy: {secondGuy}");
        }
    }

    public record Person
    {
        public string Name { get; init; }
        
        public City City { get; init; }
    }

    public record City
    {
        public string Name { get; set; }
    }
}