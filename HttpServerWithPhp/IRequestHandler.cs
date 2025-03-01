using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerWithPhp
{
    public interface IRequestHandler
    {
        public void HandleRequest(IAsyncResult result, CancellationToken cancellationToken);
    }
}
