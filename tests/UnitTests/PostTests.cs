﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Mandrill.Models.Payloads;
using NUnit.Framework;

namespace Mandrill.Tests.UnitTests
{
  [TestFixture]
  public class PostTests
  {
    private HttpTest httpClient;

    [SetUp]
    public void CreateHttpTest() {
      httpClient = new HttpTest();
    }

    [TearDown]
    public void DisposeHttpTest() {
      httpClient.Dispose();
    }

    [Test]
    public async Task Should_Throw_TimeOut_Exception_When_Timing_Out() {
      httpClient.SimulateTimeout();

      var api = new MandrillApi("");
      Assert.Throws<TimeoutException>(async () => await api.Post<object>("", new SendMessagePayload()));

    }

    [Test]
    public async Task Should_Throw_Mandrill_Exception_When_Server_Error() {
      httpClient.RespondWith(500, "");

      var api = new MandrillApi("");
      Assert.Throws<MandrillException>(async () => await api.Post<object>("", new SendMessagePayload()));

    }



    [Test]
    public async Task Should_Serialize_Response_When_Json_Content_Is_Recieved() {
      var responseString = @"{
	      ""Name"": ""Shawn"",
	      ""Id"": 1
      }";
      httpClient.RespondWith(200, responseString);

      var api = new MandrillApi("");
      var response = await api.Post<SampleObject>("", new SendMessagePayload());

      Assert.AreEqual("Shawn", response.Name);
      Assert.AreEqual(1, response.Id);
    }

    private class SampleObject
    {
      public string Name { get; set; }
      public int Id { get; set; }
    }

  }
}