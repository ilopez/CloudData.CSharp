using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDataClient
{
    public class CloudData
    {
        private RestClient web;
        public CloudData(String URLBase, String APIKey)
        {
            this.web = new RestClient(URLBase);
            this.web.Authenticator = new HttpBasicAuthenticator(APIKey, APIKey);
        }

        public bool Execute(String Query)
        {
            var rq = new RestRequest("update", Method.POST);
            rq.AddParameter("text/xml", Query, ParameterType.RequestBody);
            var rs = web.Execute(rq);
            if (rs.Content == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public QueryResponse Query(String Query)
        {
            var rq = GenerateRequest(Query);

            var rs = this.web.Execute(rq);
            int i = 0;
            QueryResponse qrs = new QueryResponse();
            qrs.HTTPCode = (int)rs.StatusCode;
            qrs.RawContent = rs.Content;
            return qrs;
        }

        private static RestRequest GenerateRequest(String Query)
        {
            var rq = new RestRequest("simplequery", Method.POST);
            rq.AddParameter("text/xml", Query, ParameterType.RequestBody);
            return rq;
        }

        public T Query<T>(String Query) where T : new ()
        {
            var rq = GenerateRequest(Query);
            var rs = this.web.Execute<T>(rq);
            if (rs.ErrorException != null)
            {
                throw new Exception("Query Exception: " + rs.ErrorException);
            }
            return rs.Data;
        }
    }
    public class QueryResponse
    {
        public int HTTPCode { get; set; }
        public String RawContent { get; set; }       
    }
}
