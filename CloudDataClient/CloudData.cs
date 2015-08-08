using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDataClient
{
    public class CloudData
    {
        private RestClient web;
        /// <summary>
        /// Creates a CloudData Client
        /// </summary>
        /// <param name="URLBase">The URL Base of the WebSite in Question.</param>
        /// <param name="APIKey">The API Key of the Script.</param>
        public CloudData(String URLBase, String APIKey)
        {
            this.web = new RestClient(URLBase);
            this.web.Authenticator = new HttpBasicAuthenticator(APIKey, APIKey);
        }

        /// <summary>
        /// Sets the Timeout for subsequent requests.
        /// </summary>
        /// <param name="Miliseconds"></param>
        public void SetTimeout(int Milliseconds)
        {
            this.web.Timeout = Milliseconds;
        }

        /// <summary>
        /// Runs a Execute/Update/Insert request without returning results.
        /// </summary>
        /// <param name="Query">UPDATE|INSERT|DELETE Query</param>
        /// <returns>True/False if the Request Works.</returns>
        public ExecuteResponse Execute(String Query)
        {
            var ers = new ExecuteResponse();
            var rq = GenerateRequest(Query);
            var rs = web.Execute(rq);
            PopulateQueryResponse(rs, ers);
            ers.Executed = (rs.Content == "true");
            ers.StopWatch.Stop();
            return ers;            
        }

        /// <summary>
        /// Returns a basic QueryResponse object.
        /// </summary>
        /// <param name="Query">SELECT Query</param>
        /// <returns></returns>
        public QueryResponse Query(String Query)
        {
            QueryResponse qrs = new QueryResponse();
            var rq = GenerateRequest(Query);
            var rs = this.web.Execute(rq);            
            PopulateQueryResponse(rs, qrs);
            qrs.StopWatch.Stop();
            return qrs;
        }

        private static void PopulateQueryResponse(IRestResponse rs, ResponseBase bs)
        {
            bs.HTTPCode = (int)rs.StatusCode;
            bs.RawContent = rs.Content;
            bs.ErrorException = rs.ErrorException;
            bs.ErrorMessage = rs.ErrorMessage;
        }

        private static RestRequest GenerateRequest(String Query)
        {
            var rq = new RestRequest("simplequery", Method.POST);
            rq.AddParameter("text/xml", Query, ParameterType.RequestBody);
            return rq;
        }

        /// <summary>
        /// Query runs a SELECT statement but returns a list or object of type T.
        /// </summary>
        /// <typeparam name="T">The Return Object Type Parameter</typeparam>
        /// <param name="Query">SELECT Query</param>
        /// <returns></returns>
        public QueryResponse<T> Query<T>(String Query) where T : new ()
        {
            var qrs = new QueryResponse<T>();
            var rq = GenerateRequest(Query);            
            var rs = this.web.Execute<T>(rq);
            PopulateQueryResponse(rs, qrs);
            if (rs.ErrorException == null)
            {
                qrs.Data = rs.Data;
            }
            qrs.StopWatch.Stop();
            return qrs;
        }

    }

    /// <summary>
    /// Public Response Base
    /// </summary>
    public class ResponseBase
    {

        public int HTTPCode { get; set; }
        public String RawContent { get; set; }
        public String ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
        public Stopwatch StopWatch { get; set; }
        public ResponseBase()
        {
            this.StopWatch = new Stopwatch();
            this.StopWatch.Start();
        }
    }

    /// <summary>
    /// Response when we just want the JSON or the Data Content
    /// </summary>
    public class QueryResponse : ResponseBase
    {        
        
    }

    /// <summary>
    /// Reponse when we want the JSON deserialized.
    /// </summary>
    /// <typeparam name="T">Target Object.</typeparam>
    public class QueryResponse<T> : ResponseBase
    {
        public T Data { get; set; }
    }

    /// <summary>
    /// Response for non Data updates
    /// </summary>
    public class ExecuteResponse : ResponseBase
    {
        public bool Executed {get;set;}
    }
}
