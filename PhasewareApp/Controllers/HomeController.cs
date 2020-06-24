using Newtonsoft.Json;
using PagedList;
using PhasewareInterview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PhasewareApp.Controllers
{
    public class HomeController : Controller
    {
        private const string URL = "https://trackerbeyond3.phaseware.com:443/dat11/api/incident/";

        //Load IncidentID that contains 2 by default
        public ActionResult Incidents(int? Page, int? incidentID = 2, string deptName = "", string custName = "", string agentName = "")
        {
            int pageSize = 10;
            int pageIndex = Page != null ? Convert.ToInt32(Page) : 1;

            IEnumerable<IncidentModel> incidents = SearchForIncidents(incidentID, deptName, custName, agentName);

            return (incidents != null && incidents.Any()) ? View(incidents.ToPagedList(pageIndex, pageSize)) : View(incidents);
        }

        public JsonResult GetIncidentDetails(int IncidentId)
        {
            IncidentModel incident = SearchForIncidents(IncidentId, "", "", "", true, "Exact").First();

            return Json(incident, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<IncidentModel> SearchForIncidents(int? incidentID = null, string deptName = "", string custName = "", string agentName = "", bool details = false, string conditionType = "Contains")
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("APIUser2:pw741103"));
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + auth);

                    //The ConditionType was not specified in the instructions; so, I made the assumption you wanted to do a contains.
                    string searchURL = "search?searchConditions=[{\"ConditionType\":\"" + conditionType + "\", \"FieldName\":\"IncidentID\", \"SearchValue\":\"" + incidentID + "\"}" +
                        (string.IsNullOrWhiteSpace(deptName) ? "" : ",{\"ConditionType\":\"" + conditionType + "\", \"FieldName\":\"DepartmentID\", \"Lookup\": {\"LookupTableName\": \"Departments\", \"LookupFieldName\": \"Name\"}, \"SearchValue\":\"" + deptName + "\"}") +
                        (string.IsNullOrWhiteSpace(custName) ? "" : ",{\"ConditionType\":\"" + conditionType + "\", \"FieldName\":\"CustomerID\", \"Lookup\": {\"LookupTableName\": \"Customer\", \"LookupFieldName\": \"Name\"}, \"SearchValue\":\"" + custName + "\"}") +
                        (string.IsNullOrWhiteSpace(agentName) ? "" : ",{\"ConditionType\":\"" + conditionType + "\", \"FieldName\":\"AgentAssigned\", \"Lookup\": {\"LookupTableName\": \"AppUsers\", \"LookupFieldName\": \"FullName\"}, \"SearchValue\":\"" + agentName + "\"}") +
                        "]&fields=[\"IncidentID\",\"DepartmentName\",\"Description\"" +
                        (details ? ",\"CustomerName\",\"AgentFullName\"" : "") +
                        "]";

                    var response = client.GetAsync(searchURL);
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var IncidentResponse = result.Content.ReadAsStringAsync().Result;
                        var jsonIncidents = JsonConvert.DeserializeObject<JsonIncidentModel>(IncidentResponse);

                        return jsonIncidents.Results;
                    }

                    else
                    {
                        //Log Error

                        return null;
                    }
                }
            }

            else
            {
                return null;
            }
        }
    }
}