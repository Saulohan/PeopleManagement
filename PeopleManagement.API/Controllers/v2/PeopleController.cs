using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleManagement.API.Requests;
using PeopleManagement.Application.DTOs;
using PeopleManagement.Application.Interfaces;

namespace PeopleManagement.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize]
    public class PeopleController(IPeopleService peopleService) : V1.PeopleController(peopleService)
    {
        [HttpPost]
        public override async Task<IActionResult> AddAsync([FromBody] CreatePersonRequest request, [FromServices] IValidator<CreatePersonRequest> validator, IMapper mapper, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return BadRequest("Request cannot be null.");
            }

            ValidationResult validationResult = await validator.ValidateAsync(request, opt =>
            {
                opt.IncludeRuleSets("default", "v2");
            }, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }));
            }

            PersonDto personDto = mapper.Map<PersonDto>(request);

            await PeopleService.AddAsync(personDto, cancellationToken);
            return Ok();
        }
    }
}
