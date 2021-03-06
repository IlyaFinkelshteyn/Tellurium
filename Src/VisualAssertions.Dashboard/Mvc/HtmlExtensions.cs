﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using Newtonsoft.Json;

namespace Tellurium.VisualAssertions.Dashboard.Mvc
{
    public static class HtmlExtensions
    {
        public static string ActionFor<TController>(this  UrlHelper urlHelper, Expression<Action<TController>> action) where TController:Controller
        {
            RouteValueDictionary valuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
            return urlHelper.RouteUrl(valuesFromExpression);
        }  
        
        public static IHtmlString ActionLink<TController>(this  AjaxHelper ajaxHelper, Expression<Action<TController>> action, string text, string updateTargetId, object htmlAttributes=null) where TController:Controller
        {
            RouteValueDictionary valuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);

            return ajaxHelper.ActionLink(text, action.Name, valuesFromExpression, new AjaxOptions() { HttpMethod = "GET", UpdateTargetId = updateTargetId }, new RouteValueDictionary(htmlAttributes));
        }
        
        public static IHtmlString ChangeActionLink<TController>(this  AjaxHelper ajaxHelper, Expression<Action<TController>> action, string text, string confirmMessage, string refreshElementId =null, object htmlAttributes=null) where TController:Controller
        {
            RouteValueDictionary valuesFromExpression = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);

            var ajaxOptions = new AjaxOptions() { HttpMethod = "POST", Confirm = confirmMessage,};
            if (string.IsNullOrWhiteSpace(refreshElementId) == false)
            {
                ajaxOptions.UpdateTargetId = refreshElementId;
                ajaxOptions.InsertionMode = InsertionMode.ReplaceWith;
            }
            return ajaxHelper.ActionLink(text, action.Name, valuesFromExpression, ajaxOptions, new RouteValueDictionary(htmlAttributes));
        }

        public static string GetCurrentActionName()
        {
            return HttpContext.Current.Request.RequestContext.RouteData.Values["action"] as string;
        }
        
        public static string GetCurrentControllerName()
        {
            return HttpContext.Current.Request.RequestContext.RouteData.Values["controller"] as string;
        }

        public static bool IsCurrentAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            if (GetCurrentControllerName() != MvcPages.WebPages.UrlHelper.GetControllerName(action))
            {
                return false;
            } 
            
            if (GetCurrentActionName() != MvcPages.WebPages.UrlHelper.GetActionName(action))
            {
                return false;
            }

            return true;
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static IHtmlString GetAppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return MvcHtmlString.Create(version);
        }

    }
}