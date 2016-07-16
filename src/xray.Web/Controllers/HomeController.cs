// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("")]
    public class HomeController : Controller
    {
        [Route("{section:regex(^cluster|dashboard$)?}")]
        public IActionResult Index(string section = null)
        {
            return this.View();
        }
    }
}