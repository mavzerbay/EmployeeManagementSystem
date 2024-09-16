using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController(IGenericRepositoryInterface<Department> genericRepositoryInterface) : GenericController<Department>(genericRepositoryInterface)
    {

    }
}