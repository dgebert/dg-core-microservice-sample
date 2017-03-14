using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using dg.api.controllers;
using dg.dataservice;


namespace dg.unittest.controller
{
    public class PeopleControllerTest
    {
        IPeopleService _mockPeopleService;

        public PeopleControllerTest()
        {
            _mockPeopleService = Substitute.For<IPeopleService>();
        }

        [Fact]
		public void Get()
        {
            //PeopleController = 
            //_mockPeopleService.GetAll().Returns
            var result = new PeopleController(new MockPeopleService());
        }
    }
}
