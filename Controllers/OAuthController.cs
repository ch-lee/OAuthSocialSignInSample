
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace OAuthSocialSignInSample.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        [Route("/oauth/github")]
        public IActionResult GithubLogin()
        {
            return Challenge(new AuthenticationProperties{ RedirectUri = "/Account" }, "Github");
        }

        [HttpGet]
        [Route("/oauth/slack")]
        public IActionResult SlackLogin()
        {
            return Challenge(new AuthenticationProperties{ RedirectUri = "Account" }, "Slack");
        }
        
        [HttpGet]
        [Route("/oauth/google")]
        public IActionResult GoogleLogin()
        {
            return Challenge(new AuthenticationProperties{ RedirectUri = "Account" }, "Google");
        }

    }
}