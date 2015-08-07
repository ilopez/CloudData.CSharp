﻿using RestSharp;
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
        /// Runs a Execute/Update/Insert request without returning results.
        /// </summary>
        /// <param name="Query">UPDATE|INSERT|DELETE Query</param>
        /// <returns>True/False if the Request Works.</returns>
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

        /// <summary>
        /// Returns a basic QueryResponse object.
        /// </summary>
        /// <param name="Query">SELECT Query</param>
        /// <returns></returns>
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

        /// <summary>
        /// Query runs a SELECT statement but returns a list or object of type T.
        /// </summary>
        /// <typeparam name="T">The Return Object Type Parameter</typeparam>
        /// <param name="Query">SELECT Query</param>
        /// <returns></returns>
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
