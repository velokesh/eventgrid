using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using AzureAssignment.Models;
using AzureAssignment.Repository;
using Google.Rpc;
using System.Linq;
using AzureAssignment.DTOs;
using AzureAssignment.Services.Implementation;

namespace AzureAssignment
{

    public class AppointmentController
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController( IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }


        [FunctionName("Appointment")]
        public async Task<IActionResult> GetAllAppointment(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "appointment")] HttpRequest req,
                ILogger log)
        {
            // Get all appointments from sql db 
            var allAppointments = await _appointmentService.GetAll();

            log.LogInformation("C# HTTP trigger function processed a request.");
     
            return new OkObjectResult(allAppointments);
        }

        [FunctionName("DeleteAppointment")]
        public async Task<IActionResult> DeleteAppointment(
               [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "appointment/{id}")] HttpRequest req, int id,
               ILogger log)
        {
            try
            {
                // Delete the appointment with the given Id 
                string message = await _appointmentService.DeleteAppointmentById(id);

                //log the success message
                log.LogInformation(message);
                return new OkObjectResult(message);
            }
            catch(ArgumentNullException ex)
            {

                //log the error message
                log.LogError(ex.Message);
                return new BadRequestObjectResult("The appointment with id = " + id + " doesn't exist");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }

        }

        [FunctionName("GetById")]
        public async Task<IActionResult> GetById(
               [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "appointment/{id}")] HttpRequest req, int id,
               ILogger log)
        {
            try
            {
                // Get the appointment with the given Id 
                Appointment appointment = await _appointmentService.GetAppointmentById(id);
                if (appointment == null)
                    return new BadRequestObjectResult("The appointment with id = " + id + " doesn't exist");

                //log the success message
                log.LogInformation("Appointment retrieved sucessfully");
                return new OkObjectResult(appointment);
            }
            catch (Exception ex)
            {
                //log the error message
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("CreateAppointment")]
        public async Task<IActionResult> CreateAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "appointment")] HttpRequest req,
            ILogger log)
        {
  
            try
            {
                var (msg, isErr) = await _appointmentService.CreateService(req);
                if (isErr)
                {
                    log.LogError(msg);
                    return new BadRequestObjectResult(msg);
                }

                log.LogInformation(msg);
                return new OkObjectResult(msg);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new BadRequestResult();
            }

        }

        [FunctionName("UpdateAppointment")]
        public async Task<IActionResult> UpdateAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "appointment/{id}")] HttpRequest req,
            ILogger log, int id)
        {
            try
            {
                var (msg, isErr) = await _appointmentService.UpdateAppointment(req, id);
                if (isErr)
                {
                    log.LogError(msg);
                    return new BadRequestObjectResult(msg);
                }

                log.LogInformation(msg);
                return new OkObjectResult(msg);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new BadRequestResult();
            }
        }
    }


}
