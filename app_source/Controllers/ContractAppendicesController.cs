using App.BLL.Interfaces;
using FS.BaseAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ContractAppendicesController : BaseAPIController
    {
        private readonly IContractAppendixBizLogic _contractAppendixBizLogic;

        public ContractAppendicesController(IContractAppendixBizLogic contractAppendixBizLogic)
        {
            _contractAppendixBizLogic = contractAppendixBizLogic;
        }
    }
}
