using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController(IGenericRepositoryInterface<Country> genericRepositoryInterface) : GenericController<Country>(genericRepositoryInterface)
    {

    }
}