using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.api.Services;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManagementOfMossadAgentsAPI.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GeneralViewController : ControllerBase
    {
        private readonly ManagementOfMossadAgentsDbContext _context;
        ServiceGeneralView _serviceGeneralView;

        public GeneralViewController(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
            _serviceGeneralView = new ServiceGeneralView(_context);
        }

        [HttpGet]
        public async Task<ActionResult<GeneralView>> GetGeneralView()
        {
            var generalView = await _serviceGeneralView.GetGeneralView();

            return Ok(generalView);
        }
    }
}
