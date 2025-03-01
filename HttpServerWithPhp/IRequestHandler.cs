using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerWithPhp
{
    public interface IRequestHandler
    {
        public void HandleRequest(HttpListenerContext request, CancellationToken cancellationToken);
    }
}
