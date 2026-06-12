using OwnerService.DTO;
using OwnerService.Models;
using ServiceCenterService.DTO;

namespace OwnerService.Services
{
    public interface IOwnerService
    {
        IEnumerable<Owner> GetAllOwners();
        Owner? GetOwnerById(string id);
        string RegisterOwner(OwnerDTO owner);
        string Login(LoginModel login, out string token);
        string UpdateOwner(string id, OwnerDTO updatedOwner);
        Task<string> DeleteOwner(string id);
        string AddServiceCenter(AddServiceCenterDTO payload);
        string RemoveServiceCenter(AddServiceCenterDTO payload);
    }
}
