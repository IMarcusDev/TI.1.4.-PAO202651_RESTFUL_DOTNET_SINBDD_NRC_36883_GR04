using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_CON_UNI_DOTNET_RESTful.Models.HTTP
{
  internal class RESTfulClient
  {
    private string baseUrl = "http://localhost:5000/";
    private HttpClient http;

    public RESTfulClient()
    {
      this.http = new HttpClient();
    }

    public async Task<string> GetAsync(EndpointsEnum endpoint)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Get,
          baseUrl + endpoint
      );

      request.Headers.Add("Accept", "application/json");

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAsync(EndpointsEnum endpoint, string token)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Get,
          baseUrl + endpoint
      );

      request.Headers.Add("Accept", "application/json");
      request.Headers.Add("Authorization", token);

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAsync(string endpoint, string token)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Get,
          baseUrl + endpoint
      );

      request.Headers.Add("Accept", "application/json");
      request.Headers.Add("Authorization", token);

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(
        EndpointsEnum endpoint,
        string json,
        string token)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Post,
          baseUrl + endpoint
      );

      request.Headers.Add("Authorization", token);
      request.Headers.Add("Accept", "application/json");

      request.Content = new StringContent(
          json,
          Encoding.UTF8,
          "application/json"
      );

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(EndpointsEnum endpoint, string json)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Post,
          baseUrl + endpoint
      );

      request.Headers.Add("Accept", "application/json");

      request.Content = new StringContent(
          json,
          Encoding.UTF8,
          "application/json"
      );

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }
  }

}
