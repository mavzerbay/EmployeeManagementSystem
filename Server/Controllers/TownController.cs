using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TownController(IGenericRepositoryInterface<Town> genericRepositoryInterface) : GenericController<Town>(genericRepositoryInterface)
    {

    }
}