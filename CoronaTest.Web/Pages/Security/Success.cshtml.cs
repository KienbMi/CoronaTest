using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoronaTest.Web.Pages.Security
{
    public class SuccessModel : PageModel
    {
        public void OnGet(Guid verificationIdentifier)
        {
            //TODO: Token verifizieren
        }
    }
}
