using App.BLL.Interfaces;
using FS.BaseAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class AppendixDocumentsController : BaseAPIController
    {
        private readonly IAppendixDocumentBizLogic _appendixDocumentBizLogic;
        private readonly IContractBizLogic _contractBizLogic;
        private readonly ILogger<AppendixDocumentsController> _logger;

        public AppendixDocumentsController(IAppendixDocumentBizLogic appendixDocumentBizLogic,
            IContractBizLogic contractBizLogic, ILogger<AppendixDocumentsController> logger)
        {
            _appendixDocumentBizLogic = appendixDocumentBizLogic;
            _contractBizLogic = contractBizLogic;
            _logger = logger;
        }
    }
}