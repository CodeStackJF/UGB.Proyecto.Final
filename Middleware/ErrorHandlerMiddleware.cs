using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using UGB.Proyecto.Final.CustomExceptions;
using UGB.Proyecto.Final.DTO;
using UGB.Proyecto.Final.Wrapper;

namespace UGB.Proyecto.Final.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception no controlada en {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            WrapperResponse wrapperResponse = new WrapperResponse();
            switch(ex)
            {
                case SqliteException e:
                    wrapperResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    wrapperResponse.Message = "Ha ocurrido un error de conexión con Sqlite.";
                    wrapperResponse.StatusCode = 400;
                    break;
                case HttpRequestException e:
                    wrapperResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    wrapperResponse.Message = e.Message;
                    wrapperResponse.StatusCode = 400;
                    break;
                case CustomValidationException e:
                    wrapperResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    wrapperResponse.Message = e.Message;
                    wrapperResponse.StatusCode = 400;
                    wrapperResponse.ValidationErrors = e.Errors;
                    break;
                default:
                    wrapperResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    wrapperResponse.Message = "Ha ocurrido un error interno en el servidor.";
                    wrapperResponse.StatusCode = 500;
                    break;
            }

            JsonSerializerOptions json = new JsonSerializerOptions();
            json.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            var result = JsonSerializer.Serialize(wrapperResponse, json);
            return response.WriteAsync(result);
        }
    }
}