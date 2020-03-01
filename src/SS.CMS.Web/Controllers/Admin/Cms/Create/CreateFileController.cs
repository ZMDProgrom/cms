﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Cms.Create
{
    [Route("admin/cms/create/createFile")]
    public partial class CreateFileController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreateFileController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateFiles))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var templates =
                await _templateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.FileTemplate);

            return new GetResult
            {
                Templates = templates
            };
        }
        

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateFiles))
            {
                return Unauthorized();
            }

            foreach (var templateId in request.TemplateIds)
            {
                await _createManager.CreateFileAsync(request.SiteId, templateId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}