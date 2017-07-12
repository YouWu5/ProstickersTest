using ProStickers.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace ProStickers.API.Infrastructure
{
    public class ValidateFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                OperationResult result = new OperationResult();
                result.Result = Result.SDError;
                result.Message = GetErrorList(actionContext.ModelState);

                actionContext.Response =
                    actionContext.Request.CreateResponse<OperationResult>(HttpStatusCode.NotAcceptable, result);
            }
            else if (actionContext.ActionArguments.FirstOrDefault().Value == null)
            {
                actionContext.ModelState.AddModelError("Model Null", "Model is empty. Please enter some value");

                OperationResult result = new OperationResult();
                result.Result = Result.SDError;
                result.Message = "Model Null: Model is empty. Please enter some value";

                actionContext.Response =
                    actionContext.Request.CreateResponse<OperationResult>(HttpStatusCode.NotAcceptable, result);
            }
            base.OnActionExecuting(actionContext);
        }

        protected string GetErrorList(ModelStateDictionary errorDictionary)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var errors in errorDictionary.Values)
            {
                if (errors.Errors.Count == 1)
                {
                    sb.Append(errors.Errors[0].ErrorMessage + Environment.NewLine);
                }
                else
                {
                    if (!String.IsNullOrEmpty(errors.Errors[1].ErrorMessage))
                    {
                        sb.Append(errors.Errors[1].ErrorMessage + Environment.NewLine);
                    }
                }
            }
            return sb.ToString();
        }
    }
}