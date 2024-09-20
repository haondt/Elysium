using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain.Services
{
    public interface IStoredDocumentFacadeFactory
    {
        public IStoredDocumentFacade Create(IDocumentService documentService);
    }
}
