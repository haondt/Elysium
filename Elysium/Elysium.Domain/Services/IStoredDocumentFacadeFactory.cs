namespace Elysium.Domain.Services
{
    public interface IStoredDocumentFacadeFactory
    {
        public IStoredDocumentFacade Create(IDocumentService documentService);
    }
}
