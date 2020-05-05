﻿using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
namespace Roxy.core
{
    public class QueryBase : MonoBehaviour
    {
        string urlRequest = "http://localhost/control-ezphera/ver-empleados.php";
        public string queryUrlAditional;
        public List<SingleQuery> queryVars = new List<SingleQuery>(0);
        protected virtual void Awake()
        {
            var queryConfig = Resources.Load<QueryBaseConfiguration>("QueryConfiguration");
            urlRequest = (queryConfig.isTest || string.IsNullOrEmpty(queryConfig.urlQuery) ? queryConfig.urlQueryTest : queryConfig.urlQuery) + queryUrlAditional;
        }
        protected virtual QueryResult Consultar()
        {
            QueryResult resultadoDigital = new QueryResult();
            HttpWebResponse response = null;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string mensaje = "{";
            for (var i = 0; i < queryVars.Count; i++)
            {

                if (!string.IsNullOrEmpty(queryVars[i].infoKey))
                {
                    mensaje += "\"" + queryVars[i].infoKey + "\":\"" + queryVars[i].inputField.text + "\"";
                }
                if (i < queryVars.Count - 1)
                {
                    mensaje += ",";
                }
            }
            mensaje += "}";
            //Debug.Log(mensaje);
            byte[] content = Encoding.ASCII.GetBytes(mensaje);
            response = CloudWebTools.DoWebRequest(urlRequest, "POST", "", content, headers, true, false);
            if (!CloudWebTools.IsErrorStatus(response))
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string newJson = reader.ReadToEnd();
                reader.Close();
                var jsonArr = JSON.Parse(newJson);
                Debug.Log(jsonArr);
                if (string.IsNullOrEmpty(jsonArr["error"].Value))
                {
                    resultadoDigital.message = "Muy bien registro exitoso";
                }
                else
                {
                    resultadoDigital.isError = true;
                    resultadoDigital.message = jsonArr["error"].Value;
                }
                resultadoDigital.result = jsonArr;
            }
            else
            {
                resultadoDigital.isError = true;
                resultadoDigital.message = "No se registro al usuario";
                // ProcessFaceError(response);
            }
            return resultadoDigital;
        }
    }
}