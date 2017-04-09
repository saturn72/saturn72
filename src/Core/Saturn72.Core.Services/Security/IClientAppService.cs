﻿using System.Collections.Generic;
using Saturn72.Core.Domain.Clients;

namespace Saturn72.Core.Services.Security
{
    public interface IClientAppService
    {
        /// <summary>
        /// gets client application 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientIpAddress"></param>
        /// <returns>App</returns>
        ClientAppDomainModel GetClientAppByClientId(string clientId, string clientIpAddress);

        /// <summary>
        /// Gets all clientapps
        /// </summary>
        /// <returns>IEnumerable of ClientApps</returns>
        IEnumerable<ClientAppDomainModel> GetAllClientApps();
    }
}