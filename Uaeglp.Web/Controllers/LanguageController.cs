using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.ViewModels;
using Uaeglp.Web.Extensions;

namespace Uaeglp.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("/api/[controller]")]
    [ApiController]
    [Authorize]

    public class LanguageController : Controller
    {
        private readonly ILogger<LanguageController> _logger;
        private readonly Contracts.ILangService _service;

        public LanguageController(ILogger<LanguageController> logger, Contracts.ILangService service)
        {
            _logger = logger;
            _service = service;
        }
        //[HttpPost("get-labels", Name = "GetLabels")]
        //public async Task<IActionResult> GetLabelsAsync([FromBody] ViewModels.PageLabelReq view)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState.GetErrorMessages());

        //    var result = await _service.GetLabels(view);
        //    if (!result.Success)
        //        return BadRequest(result.Resource);

        //    return Ok(result.Resource);
        //}

        //[HttpPost("get-label", Name = "GetLabel")]
        //public async Task<IActionResult> GetLabelAsync([FromBody] ViewModels.PageLabelReq view)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState.GetErrorMessages());

        //    var result = await _service.GetLabel(view);
        //    if (!result.Success)
        //        return BadRequest(result.Resource);

        //    return Ok(result.Resource);
        //}

        [HttpGet("get-label-portal", Name = "GetLabelPortal")]
        public async Task<IActionResult> GetLabelPortalAsync()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var result = await _service.GetLabelPortal();

            return Ok(result);
        }

        //[HttpPost("get-label-ui", Name = "GetLabelUIAsync")]
        //public async Task<IActionResult> GetLabelUIAsync([FromBody] ViewModels.PageLabelReq view)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState.GetErrorMessages());

        //    var result = await _service.GetLabelsUI(view);
        //    if (!result.Success)
        //        return BadRequest(result.Resource);

        //    return Ok(result.Resource);
        //}

        //[HttpGet("get-all-page", Name = "GetPageAsync")]
        //public async Task<IActionResult> GetPageAsync()
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState.GetErrorMessages());

        //    var result = await _service.GetPageNamesUI();
        //    if (!result.Success)
        //        return BadRequest(result.Resource);

        //    return Ok(result.Resource);
        //}

        [HttpPost("save-label", Name = "SaveLabelAsync")]
        public async Task<IActionResult> SaveLabelAsync([FromBody] ViewModels.LabelNames view)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var result = await _service.SaveLabel(view);
            if (!result.Success)
                return BadRequest(result.Resource);

            return Ok(result.Resource);
        }

        [HttpPost("update-label", Name = "updatelabel")]
        public async Task<IActionResult> Updatelabel([FromBody] ViewModels.PageData model)
        {
            var result = await _service.UpdatelabelAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete-label/{labelId}", Name = "Deletelabel")]
        public async Task<IActionResult> Deletelabel(int labelId)
        {
            var result = await _service.DeletelabelAsync(labelId);
            return Ok(result);
        }


    }
}