using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.DataTransferObjects
{
    public class TestCenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Postalcode { get; set; }
        public string Street { get; set; }
        public int SlotCapacity { get; set; }

        public TestCenterDto()
        {

        }
        public TestCenterDto(TestCenter testCenter)
        {
            Id = testCenter.Id;
            Name = testCenter.Name;
            City = testCenter.City;
            Postalcode = testCenter.Postalcode;
            Street = testCenter.Street;
            SlotCapacity = testCenter.SlotCapacity;
        }
    }
}
