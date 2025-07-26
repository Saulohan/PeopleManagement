using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleManagement.API.Requests;
using PeopleManagement.Application.DTOs;
using PeopleManagement.Application.Interfaces;

namespace PeopleManagement.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
    public class PeopleController(IPeopleService peopleService) : ControllerBase
    {
        protected virtual IPeopleService PeopleService { get; set; } = peopleService;
        [HttpGet]
        public virtual async Task<IActionResult> SearchAsync([FromQuery] GetPeopleQueryRequest? request, [FromServices] IValidator<GetPeopleQueryRequest> validator, IMapper mapper, CancellationToken cancellationToken)
        {
            if (request == null)
                return BadRequest("Query cannot be null.");


            ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }));
            }

            FilterCriteriaDto filter = mapper.Map<FilterCriteriaDto>(request);

            List<PersonDto> people = await PeopleService.SearchAsync(filter, cancellationToken);

            return Ok(people);
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<IActionResult> AddAsync([FromBody] CreatePersonRequest request, [FromServices] IValidator<CreatePersonRequest> validator, IMapper mapper, CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest("Request cannot be null.");

            ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

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


        [HttpPut("{cpf}")]
        public virtual async Task<IActionResult> UpdateAsync([FromRoute] string cpf, [FromBody] UpdatePersonRequest request, [FromServices] IValidator<UpdatePersonRequest> validator, IMapper mapper, CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest("Request cannot be null.");

            ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }));
            }

            PersonDto personDto = mapper.Map<PersonDto>(request);


            PersonDto updated = await PeopleService.UpdateAsync(cpf, personDto, cancellationToken);

            return Ok(updated);
        }

        [HttpDelete("{cpf}")]
        public virtual async Task<IActionResult> DeleteAsync([FromRoute] string cpf, CancellationToken cancellationToken)
        {
            PersonDto deleted = await PeopleService.DeleteAsync(cpf, cancellationToken);
            return Ok(deleted);
        }
    }
}