using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShield.Core
{
    public interface IApiKeyStore
    {
        Task<bool> ExistsAsync(string apiKey, CancellationToken ct);
    }
}
