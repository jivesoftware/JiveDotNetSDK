using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Net.Pokeshot.JiveSdk.Auth
{
    public class SignedRequest : ActionFilterAttribute
    {
        private static readonly string PARAM_ALGORITHM = "algorithm";
        private static readonly string PARAM_CLIENT_ID = "client_id";
        private static readonly string PARAM_JIVE_URL = "jive_url";
        private static readonly string PARAM_TENANT_ID = "tenant_id";
        private static readonly string PARAM_TIMESTAMP = "timestamp";
        private static readonly string PARAM_SIGNATURE = "signature";

        private static readonly string JIVE_EXTN = "JiveEXTN ";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.Request.Headers.Authorization != null)
                {

                    var authTemp = actionContext.Request.Headers.Authorization;

                    string authString = authTemp.Parameter;

                    //string authString = HttpContext.Current.Request.Headers["authorization"];
                    string userId = HttpContext.Current.Request.Headers["x-jive-user-id"];

                    string[] authStringArray = authString.Split('&');
                    string tenantId = null;
                    string jiveUrl = null;
                    foreach (string authElement in authStringArray)
                    {
                        string[] keyValue = authElement.Split('=');
                        if (keyValue[0].Equals("tenant_id"))
                        {
                            tenantId = keyValue[1];
                        }

                        if (keyValue[0].Equals("jive_url"))
                        {
                            jiveUrl = HttpUtility.UrlDecode(keyValue[1]);
                        }
                    }
                    string ownerId = userId + "@" + tenantId;

                    GenericIdentity MyIdentity = new GenericIdentity(ownerId);

                    String[] MyStringArray = { "User" };
                    GenericPrincipal MyPrincipal =
                        new GenericPrincipal(MyIdentity, MyStringArray);
                    Thread.CurrentPrincipal = MyPrincipal;
                }
                else
                {
                    throw new HttpRequestValidationException("Authorization header not formatted correctly");
                }
            }
            catch (Exception ex)
            {
                NewRelic.Api.Agent.NewRelic.NoticeError(ex);
                actionContext.Response = new System.Net.Http.HttpResponseMessage();
                actionContext.Response.Content = null;
                actionContext.Response.StatusCode = HttpStatusCode.InternalServerError;
            }
        }
    }
}
