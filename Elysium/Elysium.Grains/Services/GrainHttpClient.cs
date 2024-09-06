using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class GrainHttpClient<T>(HttpClient httpClient) : IGrainHttpClient<T> where T : IGrain
    {
        public HttpClient HttpClient => httpClient;
    }
}
