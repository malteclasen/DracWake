using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DracWake.Core
{
    public interface IWebClient
    {
        Task<string> Get(Uri uri);
        Task<string> Post(Uri uri, byte[] data);
    }
}
