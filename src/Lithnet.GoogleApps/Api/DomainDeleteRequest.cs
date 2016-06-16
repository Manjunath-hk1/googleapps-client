﻿using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Discovery;
using Google.Apis.Services;
using Google.Apis.Util;
using Lithnet.GoogleApps.ManagedObjects;

namespace Lithnet.GoogleApps.Api
{
    public class DomainDeleteRequest : DirectoryBaseServiceRequest<string>
    {
        public DomainDeleteRequest(IClientService service, string domainName, string customer)
            : base(service)
        {
            this.DomainName = domainName;
            this.Customer = customer;
            this.InitParameters();
        }

        [RequestParameter("domainName", RequestParameterType.Path)]
        public string DomainName { get; set; }

        [RequestParameter("customer", RequestParameterType.Path)]
        public string Customer { get; set; }

        protected override void InitParameters()
        {
            base.InitParameters();
            Parameter parameter = new Parameter
            {
                Name = "domainName",
                IsRequired = true,
                ParameterType = "path",
                DefaultValue = null,
                Pattern = null
            };
            base.RequestParameters.Add(parameter.Name, parameter);

            parameter = new Parameter
            {
                Name = "customer",
                IsRequired = true,
                ParameterType = "path",
                DefaultValue = null,
                Pattern = null
            };
            base.RequestParameters.Add(parameter.Name, parameter);
        }

        public override string HttpMethod
        {
            get
            {
                return "DELETE";
            }
        }

        public override string MethodName
        {
            get
            {
                return "delete";
            }
        }

        public override string RestPath
        {
            get
            {
                return "customer/{customer}/domains/{domainName}";
            }
        }
    }
}